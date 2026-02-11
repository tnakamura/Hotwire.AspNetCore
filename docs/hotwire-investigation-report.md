# Hotwire.AspNetCore 調査レポート

**作成日**: 2026年2月11日  
**調査対象**: ASP.NET Core 用 Hotwire 実装ライブラリ  
**調査目的**: 現状の実装状況把握、Ruby on Rails 版との比較、.NET 10 対応検証

---

## エグゼクティブサマリー

Hotwire.AspNetCore は、ASP.NET Core で Hotwire フレームワークを利用するための軽量なサーバーサイドライブラリです。Turbo Frames と Turbo Streams の基本機能を Tag Helpers として提供し、JavaScript を最小限にしてモダンなインタラクティブ Web アプリケーションの構築を可能にします。

**主な発見**:
- ✅ Turbo Frames と Turbo Streams の基本的な実装が完了
- ✅ .NET 10 環境で正常にビルド・テスト実行可能
- ⚠️ Turbo Drive は未実装（JavaScript ライブラリに依存）
- ⚠️ Stimulus.js の統合は未実装
- ⚠️ Rails 版と比較して、いくつかの高度な機能が未対応

---

## 1. リポジトリ構成の完全把握

### 1.1 プロジェクト構成

```
Hotwire.AspNetCore/
├── src/
│   ├── Hotwire.AspNetCore/         # 傘下パッケージ（Turbo + Stimulus を統合）
│   ├── Turbo.AspNetCore/           # Turbo Frame/Stream の実装
│   └── Stimulus.AspNetCore/        # Stimulus 統合（未実装）
├── examples/
│   ├── WireFrame/                  # Turbo Frames のデモアプリ
│   └── WireStream/                 # Turbo Streams のデモアプリ
└── test/
    └── Turbo.AspNetCore.Test/      # 単体テスト
```

### 1.2 ターゲットフレームワーク

| プロジェクト | ターゲットフレームワーク | 備考 |
|------------|---------------------|------|
| Turbo.AspNetCore | netstandard2.0 | 幅広い .NET バージョンに対応 |
| Hotwire.AspNetCore | netstandard2.0 | 同上 |
| Stimulus.AspNetCore | netstandard2.0 | 空プロジェクト |
| WireFrame | net6.0 | サンプルアプリ（.NET 6 は EOL 警告あり） |
| WireStream | net6.0 | 同上 |
| Turbo.AspNetCore.Test | net9.0 | テストプロジェクト |

**検証結果**: .NET 10 SDK 環境でビルド成功。全 4 テストがパス。

---

## 2. 実装済み機能の詳細

### 2.1 Turbo.AspNetCore の実装内容

#### **A. Controller 拡張メソッド** (`TurboControllerExtensions.cs`)

```csharp
public static IActionResult TurboStream(this Controller controller)
public static IActionResult TurboStream(this Controller controller, object model)
public static IActionResult TurboStream(this Controller controller, string viewName)
public static IActionResult TurboStream(this Controller controller, string viewName, object model)
```

**機能**: Turbo Stream レスポンスを返す際の Content-Type (`text/vnd.turbo-stream.html`) を自動設定。

**使用例**:
```csharp
[HttpPost]
public IActionResult Subscribe()
{
    return this.TurboStream(); // Subscribe.cshtml を Turbo Stream として返す
}
```

#### **B. Request 検出拡張メソッド** (`TurboHttpRequestExtensions.cs`)

```csharp
public static bool IsTurboFrameRequest(this HttpRequest request)
public static bool IsTurboStreamRequest(this HttpRequest request)
```

**機能**:
- `IsTurboFrameRequest()`: `turbo-frame` ヘッダーの有無を確認
- `IsTurboStreamRequest()`: Accept ヘッダーで `text/vnd.turbo-stream.html` を確認

**使用例**:
```csharp
if (Request.IsTurboFrameRequest())
{
    return PartialView("_FrameContent");
}
return View();
```

#### **C. Tag Helpers** (`TagHelpers/`)

##### Turbo Frame Tag Helper

```csharp
public class TurboFrameTagHelper : TagHelper
```

**機能**: `<turbo-frame>` タグを生成。

**使用例**:
```html
<turbo-frame id="gallery">
    <img src="~/images/ocean.jpg" alt="Ocean">
    <a asp-controller="Gallery" asp-action="Forest">Next</a>
</turbo-frame>
```

##### Turbo Stream Tag Helpers（14 種類）

**基底クラス**:
- `TurboStreamTagHelper`: `<turbo-stream>` の基底。`<template>` タグを自動挿入。
- `TurboStreamActionTagHelper`: 単一ターゲット用（`target` 属性）
- `TurboStreamActionAllTagHelper`: 複数ターゲット用（`targets` 属性）

**実装されているアクション**:

| Tag Helper | アクション | 対象 | 説明 |
|-----------|---------|-----|------|
| `TurboStreamAppendTagHelper` | `append` | 単一 | ターゲット要素の末尾に追加 |
| `TurboStreamPrependTagHelper` | `prepend` | 単一 | ターゲット要素の先頭に追加 |
| `TurboStreamReplaceTagHelper` | `replace` | 単一 | ターゲット要素を置換（要素自体も含む） |
| `TurboStreamUpdateTagHelper` | `update` | 単一 | ターゲット要素の内部 HTML を更新 |
| `TurboStreamRemoveTagHelper` | `remove` | 単一 | ターゲット要素を削除 |
| `TurboStreamBeforeTagHelper` | `before` | 単一 | ターゲット要素の前に挿入 |
| `TurboStreamAfterTagHelper` | `after` | 単一 | ターゲット要素の後に挿入 |
| `TurboStreamAppendAllTagHelper` | `append` | 複数 | 複数のターゲット要素の末尾に追加 |
| `TurboStreamPrependAllTagHelper` | `prepend` | 複数 | 複数のターゲット要素の先頭に追加 |
| `TurboStreamReplaceAllTagHelper` | `replace` | 複数 | 複数のターゲット要素を置換 |
| `TurboStreamUpdateAllTagHelper` | `update` | 複数 | 複数のターゲット要素の内部 HTML を更新 |
| `TurboStreamRemoveAllTagHelper` | `remove` | 複数 | 複数のターゲット要素を削除 |
| `TurboStreamBeforeAllTagHelper` | `before` | 複数 | 複数のターゲット要素の前に挿入 |
| `TurboStreamAfterAllTagHelper` | `after` | 複数 | 複数のターゲット要素の後に挿入 |

**使用例**:
```html
<turbo-stream-replace target="subscriber-count">
    <p id="subscriber-count">
        You have @Random.Shared.Next(100) subscribers.
    </p>
</turbo-stream-replace>

<turbo-stream-append target="subscriber-list">
    <li>@Context.Request.Form["name"]</li>
</turbo-stream-append>
```

### 2.2 サンプルアプリケーション

#### **WireFrame** - Turbo Frames デモ

**機能**: ギャラリーナビゲーション（Forest, Mountains, Ocean）をページ全体の再読み込みなしで実現。

**実装ポイント**:
- `<turbo-frame id="gallery">` でコンテンツをラップ
- フレーム内のリンククリックで、該当フレームのみが更新される
- JavaScript は Turbo.js の CDN 読み込みのみ

**コード例**:
```html
<script type="module">
    import * as Turbo from 'https://cdn.skypack.dev/@hotwired/turbo';
</script>

<turbo-frame id="gallery">
    <img src="~/images/ocean.jpg" alt="Ocean" width="500" height="400">
    <div>
        <a asp-controller="Gallery" asp-action="Forest">Next (forest)</a>
    </div>
</turbo-frame>
```

#### **WireStream** - Turbo Streams デモ

**機能**: ニュースレター購読フォームで、複数の DOM 要素を同時更新。

**実装ポイント**:
- POST リクエストが `TurboStream()` アクションを返す
- View ファイル（`Subscribe.cshtml`）が複数の `<turbo-stream-*>` タグを含む
- 購読者数の表示更新と、リストへの新規追加を同時実行

**コード例**:
```csharp
// Controller
[HttpPost]
public IActionResult Subscribe()
{
    return this.TurboStream();
}
```

```html
<!-- Subscribe.cshtml -->
<turbo-stream-replace target="subscriber-count">
    <p id="subscriber-count" style="padding: 1em; background-color: lightyellow; text-align: center;">
        You have @Random.Shared.Next(100) subscribers.
    </p>
</turbo-stream-replace>

<turbo-stream-append target="subscriber-list">
    <li>@Context.Request.Form["name"]</li>
</turbo-stream-append>
```

### 2.3 テスト

**テストファイル**: `Turbo.AspNetCore.Test/TurboHttpRequestExtensionsTest.cs`

**テスト内容**:
- `IsTurboFrameRequest()` の動作検証（`turbo-frame` ヘッダーの有無）
- `IsTurboStreamRequest()` の動作検証（Accept ヘッダーのメディアタイプ確認）

**テスト結果**: 全 4 テストがパス（.NET 9/10 で検証済み）

---

## 3. 本家 Hotwire (Ruby on Rails) との比較

### 3.1 Hotwire の 3 大機能

| 機能 | 説明 | Rails 実装 | ASP.NET Core 実装 |
|-----|------|----------|----------------|
| **Turbo Drive** | リンク・フォーム送信を AJAX 化し、ページ全体の再読み込みを防ぐ。`<body>` を置換し `<head>` をマージ。 | ✅ デフォルトで有効 | ⚠️ JavaScript (Turbo.js) に依存。サーバーサイド実装なし |
| **Turbo Frames** | `<turbo-frame>` でページを独立したセグメントに分割。フレーム内のナビゲーションはフレームのみを更新。 | ✅ `turbo_frame_tag` ヘルパー提供 | ✅ `TurboFrameTagHelper` 実装済み |
| **Turbo Streams** | HTTP レスポンス、WebSocket、SSE 経由で DOM の一部をリアルタイム更新。`append`, `replace` などのアクションを提供。 | ✅ `turbo_stream` ヘルパー、ActionCable 統合 | ✅ 基本アクション実装済み（14 種類の Tag Helper） |

### 3.2 Rails 版の主要ヘルパー vs ASP.NET Core 実装

#### Rails 版のヘルパー

```ruby
# Turbo Frame
<%= turbo_frame_tag "comments" do %>
  <%= render @comments %>
<% end %>

# Turbo Stream (Controller)
def create
  @comment = @post.comments.create(comment_params)
  respond_to do |format|
    format.turbo_stream # create.turbo_stream.erb を自動レンダリング
    format.html { redirect_to @post }
  end
end

# Turbo Stream (View)
<%= turbo_stream.append "comments" do %>
  <%= render @comment %>
<% end %>

<%= turbo_stream.replace "new_comment" do %>
  <%= render "form", post: @post, comment: Comment.new %>
<% end %>

# WebSocket/SSE 購読
<%= turbo_stream_from "post_#{@post.id}_comments" %>
```

#### ASP.NET Core 版の実装

```csharp
// Controller
[HttpPost]
public IActionResult Subscribe()
{
    return this.TurboStream(); // Subscribe.cshtml をレンダリング
}
```

```html
<!-- View: Turbo Frame -->
<turbo-frame id="comments">
    @foreach (var comment in Model)
    {
        <div>@comment.Text</div>
    }
</turbo-frame>

<!-- View: Turbo Stream -->
<turbo-stream-append target="comments">
    <div>@Model.Text</div>
</turbo-stream-append>

<turbo-stream-replace target="new_comment">
    <form asp-action="Create">
        <!-- フォーム内容 -->
    </form>
</turbo-stream-replace>
```

**類似点**:
- Tag Helper ベースのアプローチは Rails の ERB ヘルパーと直感的に対応
- Controller での Content-Type 設定が簡潔
- 基本的な Turbo Stream アクションをすべてカバー

**相違点**:
- Rails: `format.turbo_stream` で自動的にビューを選択
- ASP.NET Core: 明示的に `TurboStream()` を呼び出し、ビュー名は通常の規約に従う
- Rails: ActionCable との統合が標準装備（`turbo_stream_from`）
- ASP.NET Core: WebSocket/SSE の統合は未実装（SignalR との統合が今後の拡張ポイント）

### 3.3 機能の実装状況比較

| 機能 | Rails (turbo-rails) | ASP.NET Core (Hotwire.AspNetCore) | 実装状況 |
|-----|-------------------|--------------------------------|---------|
| **Turbo Drive** | | | |
| - リンク/フォームの AJAX 化 | ✅ | ⚠️ JS のみ | 未対応（サーバー側不要） |
| **Turbo Frames** | | | |
| - `<turbo-frame>` タグ生成 | ✅ | ✅ | **実装済み** |
| - `src` 属性による遅延読み込み | ✅ | ✅ | **実装済み**（JS 側） |
| - `target="_top"` などのナビゲーション制御 | ✅ | ✅ | **実装済み**（JS 側） |
| **Turbo Streams** | | | |
| - 基本アクション（7 種） | ✅ | ✅ | **実装済み** |
| - 複数ターゲットアクション（7 種） | ✅ | ✅ | **実装済み** |
| - `morph` アクション（Turbo 8+） | ✅ | ❌ | **未対応** |
| - `refresh` アクション（Turbo 8+） | ✅ | ❌ | **未対応** |
| - ActionCable/WebSocket 統合 | ✅ | ❌ | **未対応** |
| - SSE 統合 | ✅ | ❌ | **未対応** |
| - カスタムアクション | ✅ | ❌ | **未対応** |
| **Stimulus** | | | |
| - Stimulus.js 統合 | ✅ | ❌ | **未対応**（空プロジェクト） |
| **その他** | | | |
| - テストヘルパー | ✅ | ⚠️ | 最小限のみ |
| - ドキュメント | ✅ 充実 | ❌ | 未整備 |

### 3.4 Turbo 8 の新機能（2023年〜）

Turbo 8 では以下の新機能が追加されていますが、Hotwire.AspNetCore では未対応です：

#### **`morph` アクション**
- DOM の状態（入力値、フォーカス、スクロール位置など）を保持しながら、変更された部分のみを更新
- フォーム入力中のユーザー体験を損なわずに更新可能

**Rails での使用例**:
```erb
<%= turbo_stream.morph "form" do %>
  <%= render "form", record: @record %>
<% end %>
```

**メタタグでページ全体の morph を有効化**:
```html
<meta name="turbo-refresh-method" content="morph">
<meta name="turbo-refresh-scroll" content="preserve">
```

#### **`refresh` アクション**
- ページ全体のリフレッシュをトリガー
- morph と組み合わせることで、変更部分のみを更新
- WebSocket 経由で全クライアントにブロードキャスト可能

**Rails での使用例**:
```erb
<%= turbo_stream.refresh %>
```

**ASP.NET Core での実装に必要な要素**:
- `TurboStreamMorphTagHelper` と `TurboStreamRefreshTagHelper` の追加
- クライアント側で Turbo 8+ の使用
- ドキュメントとサンプルの整備

---

## 4. 未実装機能と今後の拡張ポイント

### 4.1 優先度: 高

#### **A. SignalR との統合（リアルタイム Turbo Streams）**

**説明**: Rails の ActionCable に相当する、ASP.NET Core の SignalR を使ったリアルタイム更新機能。

**実装イメージ**:
```csharp
// Hub
public class TurboStreamsHub : Hub
{
    public async Task BroadcastTurboStream(string channel, string html)
    {
        await Clients.Group(channel).SendAsync("ReceiveTurboStream", html);
    }
}

// Controller 拡張
public static async Task BroadcastTurboStream(
    this IHubContext<TurboStreamsHub> hubContext,
    string channel,
    string viewName,
    object model)
{
    // View を HTML としてレンダリングし、SignalR でブロードキャスト
}
```

**ユースケース**:
- チャットアプリ
- リアルタイムダッシュボード
- 多人数同時編集
- 通知システム

#### **B. Turbo 8 新機能のサポート**

1. **`morph` アクション**
   - `TurboStreamMorphTagHelper` の実装
   - ドキュメントとサンプルの追加

2. **`refresh` アクション**
   - `TurboStreamRefreshTagHelper` の実装
   - SignalR と組み合わせた全体リフレッシュのサンプル

#### **C. カスタム Turbo Stream アクション**

**説明**: ユーザー定義のカスタムアクションをサポート。

**Rails での例**:
```javascript
// JavaScript 側でカスタムアクションを定義
Turbo.StreamActions.set_title = function() {
  document.title = this.getAttribute("title");
}
```

```erb
<%= turbo_stream.action(:set_title, title: "New Title") %>
```

**ASP.NET Core での実装案**:
```csharp
public class TurboStreamCustomActionTagHelper : TurboStreamTagHelper
{
    public string Action { get; set; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("action", Action);
        base.Process(context, output);
    }
}
```

### 4.2 優先度: 中

#### **A. Stimulus.js の統合**

**現状**: `Stimulus.AspNetCore` プロジェクトは空のまま。

**実装案**:
- Stimulus コントローラーを Razor ページから簡単に参照できる Tag Helper
- データ属性の自動設定ヘルパー

**使用例イメージ**:
```html
<div stimulus-controller="dropdown">
    <button stimulus-action="click->dropdown#toggle">Toggle</button>
    <div stimulus-target="dropdown.menu">Menu</div>
</div>
```

#### **B. テストヘルパーの拡充**

**現状**: 最小限の単体テストのみ。

**追加すべきテスト**:
- Tag Helper の出力検証（各アクション）
- 統合テスト（実際のリクエスト/レスポンスのシミュレーション）
- SignalR 統合後のリアルタイム更新テスト

#### **C. ドキュメントの整備**

**必要なドキュメント**:
- README.md の充実（クイックスタート、API リファレンス）
- 各機能のチュートリアル
- Rails からの移行ガイド
- ベストプラクティス集

### 4.3 優先度: 低

#### **A. サーバーサイドレンダリングの最適化**

- Turbo Stream の HTML 生成を効率化
- View のキャッシング機能

#### **B. IDE サポートの強化**

- Tag Helper の IntelliSense 改善
- スニペット集の提供

---

## 5. .NET 10 対応状況と将来性

### 5.1 現在の対応状況

**検証環境**:
- .NET SDK: 10.0.102
- ビルド: ✅ 成功（警告あり）
- テスト: ✅ 全 4 テストがパス

**警告内容**:
```
warning NETSDK1138: The target framework 'net6.0' is out of support and will not receive security updates in the future.
```

**対象**: サンプルアプリ（WireFrame, WireStream）が net6.0 をターゲット。

### 5.2 推奨される改善

#### **A. サンプルアプリのターゲットフレームワーク更新**

**現状**: net6.0（EOL: 2024年11月）

**推奨**: net8.0 または net9.0 に更新

**変更箇所**:
```xml
<!-- WireFrame.csproj, WireStream.csproj -->
<PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <!-- または <TargetFramework>net8.0</TargetFramework> -->
</PropertyGroup>
```

**理由**:
- .NET 8 は LTS（Long Term Support、2026年11月までサポート）
- .NET 9 は最新の機能を活用可能（2024年11月リリース、2026年5月までサポート）

#### **B. ライブラリ自体の互換性**

**現状**: netstandard2.0（非常に広範囲の .NET バージョンに対応）

**考慮事項**:
- netstandard2.0 は .NET Framework 4.7.2+ および .NET Core 2.0+ をサポート
- 現時点では変更不要
- 将来的に .NET 特有の機能（例: Source Generator）を活用する場合は、net8.0+ をターゲットに追加

### 5.3 将来的な拡張性

#### **A. クライアント側フレームワークとの統合**

**可能性**:
- Blazor との併用（Turbo Frames 内に Blazor コンポーネント）
- HTMX などの他の HTML-over-the-wire ライブラリとの比較・統合

**検討ポイント**:
- Turbo.js と Blazor の JavaScript インタープリターの競合回避
- SPA フレームワーク（React, Vue, Angular）との併用シナリオ

#### **B. Native AOT (Ahead-of-Time Compilation) 対応**

.NET 8+ では Native AOT が強化されています。Hotwire.AspNetCore が Native AOT をサポートすることで、以下のメリットが得られます：

- 起動時間の短縮
- メモリ使用量の削減
- コンテナイメージのサイズ削減

**必要な作業**:
- Reflection の使用を最小化
- AOT 互換性のテスト

#### **C. Minimal APIs との親和性**

ASP.NET Core の Minimal APIs スタイルとの統合を検討：

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTurbo();

var app = builder.Build();

app.MapPost("/subscribe", (HttpContext context) =>
{
    // Turbo Stream を返す簡潔な記法
    return Results.TurboStream("Subscribe", model);
});

app.Run();
```

---

## 6. ベストプラクティスと推奨事項

### 6.1 現在のライブラリの使用推奨事項

#### **適している用途**
- ✅ フォーム送信後の部分更新
- ✅ ページ内の独立したウィジェット（コメント欄、サイドバーなど）
- ✅ JavaScript を極力書きたくないプロジェクト
- ✅ サーバーサイドレンダリングを重視するプロジェクト

#### **現時点で不向きな用途**
- ❌ リアルタイム性が重要なアプリ（チャット、協業ツールなど）→ SignalR 統合後は可能
- ❌ 複雑なクライアント側の状態管理が必要なアプリ → Blazor や SPA フレームワークの方が適切
- ❌ オフライン対応が必要なアプリ

### 6.2 実装時のベストプラクティス

#### **A. Turbo Frame の適切な粒度**

**推奨**: 独立して更新できる UI の単位でフレームを分割。

```html
<!-- 良い例: コメント欄を独立したフレームに -->
<turbo-frame id="comments">
    @foreach (var comment in Model.Comments)
    {
        <div>@comment.Text</div>
    }
</turbo-frame>

<!-- 避けるべき例: ページ全体を 1 つのフレームに -->
<turbo-frame id="everything">
    <!-- ページ全体 -->
</turbo-frame>
```

#### **B. Turbo Stream のターゲット ID 設計**

**推奨**: 一意で予測可能な ID を使用。

```html
<!-- 良い例 -->
<div id="subscriber-count">...</div>
<ul id="subscriber-list">...</ul>

<!-- 避けるべき例: 動的に変わる ID -->
<div id="@Guid.NewGuid()">...</div>
```

#### **C. エラーハンドリング**

Turbo Stream リクエストが失敗した場合の対処:

```csharp
[HttpPost]
public IActionResult Subscribe()
{
    try
    {
        // 処理
        return this.TurboStream();
    }
    catch (Exception ex)
    {
        if (Request.IsTurboStreamRequest())
        {
            return this.TurboStream("Error", new { Message = ex.Message });
        }
        return View("Error");
    }
}
```

#### **D. プログレッシブエンハンスメント**

JavaScript が無効な環境でも基本機能が動作するように設計:

```html
<form method="post" action="/subscribe">
    <!-- Turbo が有効な場合は Turbo Stream レスポンス -->
    <!-- 無効な場合は通常のフォーム送信 -->
    <input type="text" name="email" />
    <button type="submit">Subscribe</button>
</form>
```

### 6.3 パフォーマンス最適化

#### **A. Turbo Frame の遅延読み込み**

```html
<turbo-frame id="expensive-content" src="/load-expensive-content" loading="lazy">
    Loading...
</turbo-frame>
```

#### **B. Turbo Stream レスポンスの最小化**

```csharp
// 必要な部分のみを更新
return this.TurboStream("_PartialUpdate", model);
```

#### **C. CDN の活用**

Turbo.js は CDN から読み込むことで、キャッシュを活用:

```html
<script type="module">
    import * as Turbo from 'https://cdn.skypack.dev/@hotwired/turbo';
</script>
```

---

## 7. 結論と次のステップ

### 7.1 主要な発見のまとめ

1. **実装状況**:
   - Turbo Frames と Turbo Streams の基本機能は実装済み
   - Tag Helper ベースの直感的な API
   - .NET 10 環境で問題なく動作

2. **Rails 版との比較**:
   - 基本的なアプローチは Rails 版と同等
   - WebSocket/SSE 統合、Turbo 8 の新機能は未対応
   - Stimulus.js の統合は空のまま

3. **将来性**:
   - SignalR との統合で Rails 並みのリアルタイム機能が実現可能
   - .NET の最新機能（Native AOT、Minimal APIs）との親和性あり
   - 拡張の余地は大きい

### 7.2 推奨される次のステップ

#### **短期（1〜3ヶ月）**

1. **サンプルアプリのモダナイゼーション**
   - net6.0 → net8.0 または net9.0 への更新
   - 追加のサンプルシナリオ（CRUD 操作、検索など）

2. **ドキュメントの整備**
   - README.md の充実
   - API リファレンスの作成
   - チュートリアルの追加

3. **テストの拡充**
   - Tag Helper の出力検証
   - 統合テストの追加

#### **中期（3〜6ヶ月）**

1. **SignalR 統合の実装**
   - `TurboStreamsHub` の実装
   - リアルタイム更新のサンプル
   - ドキュメント化

2. **Turbo 8 新機能のサポート**
   - `morph` と `refresh` アクションの実装
   - クライアント側のライブラリ更新

3. **カスタムアクションのサポート**
   - 拡張可能な Tag Helper 基盤
   - サンプルとドキュメント

#### **長期（6ヶ月〜）**

1. **Stimulus.js の統合**
   - Tag Helper の実装
   - サンプルアプリ
   - Rails 版との機能パリティ

2. **エコシステムの拡大**
   - Blazor との統合ガイド
   - コミュニティからの貢献受け入れ体制
   - NuGet パッケージの継続的なメンテナンス

### 7.3 評価とメンテナンス推奨度

**総合評価**: ⭐⭐⭐⭐ (5 段階中 4)

**理由**:
- ✅ 基本機能は堅牢で実用的
- ✅ コードベースは読みやすく拡張しやすい
- ✅ Rails 版の設計思想を ASP.NET Core に適切に移植
- ⚠️ リアルタイム機能の欠如が現時点での制約
- ⚠️ ドキュメント不足が採用障壁

**メンテナンス推奨度**: **高**

このライブラリは ASP.NET Core エコシステムにおいて貴重な位置を占めています。JavaScript を最小限にしてモダンなインタラクティブ Web アプリを構築できる選択肢として、今後の拡張と継続的なメンテナンスを強く推奨します。

---

## 8. 付録

### 8.1 参考リソース

#### **公式ドキュメント**
- [Hotwire 公式サイト](https://hotwired.dev/)
- [Turbo Handbook](https://turbo.hotwired.dev/handbook/introduction)
- [turbo-rails GitHub](https://github.com/hotwired/turbo-rails)

#### **ASP.NET Core 関連**
- [ASP.NET Core Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro)
- [SignalR 公式ドキュメント](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [.NET 10 リリースノート](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)

#### **コミュニティリソース**
- [Hotwire & Turbo in Rails: Complete Guide 2025](https://www.railscarma.com/blog/hotwire-and-turbo-in-rails-complete-guide/)
- [Turbo 8 Page Refreshes (+ Morphing) Explained](https://jonsully.net/blog/turbo-8-page-refreshes-morphing-explained-at-length/)
- [Hotwire Cheatsheet for Ruby on Rails Developers](https://cheatsheetshero.com/user/igor-kasyanchuk/930-hotwire-cheatsheet-for-ruby-on-rails-developers)

### 8.2 用語集

| 用語 | 説明 |
|-----|------|
| **Hotwire** | HTML Over The Wire の略。JavaScript を最小限にしてインタラクティブな Web アプリを構築するフレームワーク。 |
| **Turbo** | Hotwire のコア機能。Drive, Frames, Streams の 3 つの機能を提供。 |
| **Turbo Drive** | リンクとフォーム送信を AJAX 化し、ページ全体の再読み込みを防ぐ。 |
| **Turbo Frames** | ページを独立したセグメントに分割し、部分的な更新を可能にする。 |
| **Turbo Streams** | DOM の一部をリアルタイムで更新するための機能。HTTP、WebSocket、SSE をサポート。 |
| **Stimulus** | Hotwire の補完的な JavaScript フレームワーク。最小限の JavaScript で DOM 操作を実現。 |
| **Tag Helper** | ASP.NET Core の Razor ビューで使用できるサーバーサイドコンポーネント。 |
| **SignalR** | ASP.NET Core のリアルタイム通信ライブラリ。WebSocket、SSE、ロングポーリングをサポート。 |
| **ActionCable** | Rails のリアルタイム通信フレームワーク。SignalR に相当。 |

---

**調査担当**: GitHub Copilot Agent  
**レポートバージョン**: 1.0  
**最終更新**: 2026年2月11日
