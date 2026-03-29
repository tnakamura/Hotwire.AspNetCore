# ASP.NET Core にちょうどいいリッチさを。Hotwire.AspNetCore を作った理由と使い方

こんにちは。この記事では、`Hotwire.AspNetCore` を開発した背景、ライブラリの特徴、実際の使い方、そして実装時に工夫したポイントをまとめます。

## 開発の動機

ASP.NET Core で「少しリッチな Web アプリ」を作りたいとき、選択肢が極端だと感じていました。

- MVC / Razor Pages のままでは、画面の一部更新やリアルタイム更新を行うのに手作業が増える
- かといって Blazor に寄せると、構成や開発体験が大きく変わる

この「MVC/Razor Pages と Blazor の間」を埋めるアプローチとして、Rails の Hotwire がちょうど良さそうだと感じ、ASP.NET Core で扱いやすい形で実装してみたのが `Hotwire.AspNetCore` です。

---

## Hotwire.AspNetCore の特徴

### 1. Turbo Drive

ページ遷移を高速化するための機能です。リンククリックやフォーム送信を Turbo が扱い、フルリロードを減らします。

- `turbo-drive-meta` Tag Helper で有効化・遷移設定
- `turbo-permanent` Tag Helper で永続要素を定義
- `Request.IsTurboDriveRequest()` でサーバー側判定

関連実装:

- `/src/Turbo.AspNetCore/TagHelpers/TurboDriveMetaTagHelper.cs`
- `/src/Turbo.AspNetCore/TagHelpers/TurboPermanentTagHelper.cs`
- `/src/Turbo.AspNetCore/TurboHttpRequestExtensions.cs`

### 2. Turbo Frames

ページの一部だけを差し替える機能です。Razor Pages / MVC でも自然に段階導入できます。

- `turbo-frame` Tag Helper 対応
- `Request.IsTurboFrameRequest()` で判定可能

関連実装:

- `/src/Turbo.AspNetCore/TagHelpers/TurboFrameTagHelper.cs`
- `/src/Turbo.AspNetCore/TurboHttpRequestExtensions.cs`

### 3. Turbo Streams（標準 + カスタム）

サーバーから HTML 片を返して DOM 更新を指示します。

- 主要アクション（append / prepend / replace / update / remove / before / after）
- 複数要素向け `*_all` 系
- Turbo 8 系の `morph` / `refresh`
- カスタムアクション定義（`turbo-stream-custom`、`Html.TurboStreamCustom(...)`）

関連実装:

- `/src/Turbo.AspNetCore/TagHelpers/TurboStreamTagHelper.cs`
- `/src/Turbo.AspNetCore/TagHelpers/TurboStreamCustomActionTagHelper.cs`
- `/src/Turbo.AspNetCore/TurboStreamCustomHtmlExtensions.cs`
- `/src/Turbo.AspNetCore/TurboControllerExtensions.cs`

### 4. SignalR 連携（リアルタイム配信）

Turbo Stream を SignalR で push できるので、通知・チャット・ダッシュボード更新などをサーバー主導で実現できます。

- `TurboStreamsHub` でチャネル購読モデルを提供
- `ITurboStreamBroadcaster` で channel / connection / all 宛て配信
- `turbo-signalr.js` でクライアント接続・再接続・再購読

関連実装:

- `/src/Turbo.AspNetCore/Hubs/TurboStreamsHub.cs`
- `/src/Turbo.AspNetCore/TurboStreamBroadcaster.cs`
- `/src/Turbo.AspNetCore/TurboEndpointRouteBuilderExtensions.cs`
- `/src/Turbo.AspNetCore/wwwroot/js/turbo-signalr.js`

### 5. Stimulus 連携（別パッケージ）

`Stimulus.AspNetCore` で、`stimulus-*` 属性を Tag Helper / Html Helper で扱いやすくしています。

関連実装:

- `/src/Stimulus.AspNetCore/TagHelpers/*`
- `/src/Stimulus.AspNetCore/StimulusHtmlExtensions.cs`

---

## 使い方（最小構成）

### 1) パッケージ導入

```bash
dotnet add package Hotwire.AspNetCore
```

または個別導入:

```bash
dotnet add package Turbo.AspNetCore
dotnet add package Stimulus.AspNetCore
```

### 2) Tag Helper を登録

`Views/_ViewImports.cshtml`

```csharp
@addTagHelper *, Turbo.AspNetCore
@addTagHelper *, Stimulus.AspNetCore
```

### 3) Turbo.js を読み込み、Turbo Drive を有効化

`Views/Shared/_Layout.cshtml`

```html
<script type="module" src="https://cdn.jsdelivr.net/npm/@hotwired/turbo@latest/dist/turbo.es2017-esm.min.js"></script>
<turbo-drive-meta enabled="true" transition="fade" />
```

### 4) Controller で Turbo Stream 判定して返す

```csharp
using Turbo.AspNetCore;

public IActionResult Create(MessageViewModel model)
{
    if (Request.IsTurboStreamRequest())
    {
        return this.TurboStream("_Message", model);
    }

    return RedirectToAction("Index");
}
```

### 5) View で Turbo Stream を返す

```html
<turbo-stream-append target="messages">
  <div class="message">@Model.Content</div>
</turbo-stream-append>
```

### 6) SignalR でリアルタイム配信（必要な場合）

`Program.cs`

```csharp
builder.Services.AddSignalR();
builder.Services.AddTurboStreamBroadcaster();

var app = builder.Build();
app.MapTurboStreamsHub();
```

サーバー側で配信:

```csharp
await _broadcaster.BroadcastViewAsync("notifications", "_Notification", model);
```

クライアント側では `turbo-signalr.js` を読み込み、チャネル購読します。

---

## ソースコードを読んでわかった「実装時の工夫ポイント」

### 1) リクエスト判定を拡張メソッドに集約

`TurboHttpRequestExtensions` に判定ロジックを寄せることで、Controller 側は `if (Request.IsTurboStreamRequest())` のように短く書けます。

特に `IsTurboDriveRequest()` は、`turbo-frame` ヘッダー有無と `Accept: text/html` の組み合わせで判定しており、Turbo の振る舞いに沿った実装になっています。

- `/src/Turbo.AspNetCore/TurboHttpRequestExtensions.cs`

### 2) Turbo Stream Tag Helper の継承設計で重複を削減

`TurboStreamTagHelper`（基底）→ `TurboStreamActionTagHelper` / `TurboStreamActionAllTagHelper`（中間）→ 各アクション（具象）という構造です。

- 共通処理（`<turbo-stream>` への変換、`<template>` 自動ラップ）を基底に集約
- 各アクションは `Action` プロパティを返すだけ

これにより、アクション追加時の差分を小さく保ちつつ、挙動の一貫性を維持しやすくしています。

- `/src/Turbo.AspNetCore/TagHelpers/TurboStreamTagHelper.cs`

### 3) カスタムアクションで「任意属性」を受け取れる設計

`TurboStreamCustomActionTagHelper` では `DictionaryAttributePrefix = ""` を使い、`action` 以外の属性を柔軟に受け取って出力へ渡しています。

これにより、JS 側の `Turbo.StreamActions.xxx` と組み合わせるだけで、用途別のカスタムストリームを追加できます。

- `/src/Turbo.AspNetCore/TagHelpers/TurboStreamCustomActionTagHelper.cs`
- `/src/Turbo.AspNetCore/TurboStreamCustomHtmlExtensions.cs`

### 4) SignalR Broadcaster で「View を文字列化して配信」

`TurboStreamBroadcaster` は `IRazorViewEngine` を使ってビューをレンダリングし、その HTML を SignalR で配信します。

さらに `IHttpContextAccessor` から現在の `HttpContext` が取れない場合に備え、`DefaultHttpContext` を補う実装になっており、実行コンテキストの違いに耐える作りです。

- `/src/Turbo.AspNetCore/TurboStreamBroadcaster.cs`

### 5) クライアント接続の復旧戦略

`turbo-signalr.js` では自動再接続と、再接続後のチャネル再購読を実装しています。運用で起きがちな瞬断に強いのは実務的に重要です。

- `/src/Turbo.AspNetCore/wwwroot/js/turbo-signalr.js`

### 6) API の導入障壁を下げる拡張メソッド

- `AddTurboStreamBroadcaster()`（DI 登録）
- `MapTurboStreamsHub()`（エンドポイントマッピング）
- `TurboStream(...)`（Controller 返却）

といった拡張メソッドで、利用側コードの定型を短くできるようにしています。

- `/src/Turbo.AspNetCore/TurboServiceCollectionExtensions.cs`
- `/src/Turbo.AspNetCore/TurboEndpointRouteBuilderExtensions.cs`
- `/src/Turbo.AspNetCore/TurboControllerExtensions.cs`

---

## どんなときに向いているか

- 既存の MVC / Razor Pages を活かしつつ、画面体験を一段リッチにしたい
- SPA に全面移行するほどではないが、部分更新やリアルタイム性は欲しい
- サーバーサイドレンダリング中心で開発したい

## 補足（正直な注意点）

- Turbo / Stimulus は JavaScript ランタイム前提
- SignalR を複数インスタンス運用する場合は、バックプレーン構成を別途検討する必要あり
- カスタムアクションは Tag Helper 側だけで完結せず、クライアント JS 側実装が必要

---

## まとめ

`Hotwire.AspNetCore` は、ASP.NET Core において「MVC/Razor Pages の延長で、ちょうどいいリッチさ」を実現するための選択肢です。

Blazor ほど大きくアーキテクチャを振らず、かつ jQuery 的な局所ハックにも寄りすぎない。そんな中間地点を、Turbo Drive / Frames / Streams と SignalR 連携で埋められるように設計しました。

まずは Turbo Drive と Turbo Streams の小さな導入から始めて、必要に応じて SignalR や Stimulus を足していくのがおすすめです。
