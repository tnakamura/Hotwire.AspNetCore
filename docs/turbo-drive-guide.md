# Turbo Drive ガイド

Turbo Drive は、リンクとフォーム送信を AJAX 化することで、ページ全体の再読み込みを防ぎ、Web アプリケーションを高速化するライブラリです。

## 目次

1. [概要](#概要)
2. [セットアップ](#セットアップ)
3. [Tag Helper リファレンス](#tag-helper-リファレンス)
4. [拡張メソッド](#拡張メソッド)
5. [ベストプラクティス](#ベストプラクティス)
6. [トラブルシューティング](#トラブルシューティング)

## 概要

### Turbo Drive の仕組み

Turbo Drive は以下のように動作します：

1. ユーザーがリンクをクリック
2. Turbo Drive が AJAX リクエストでページを取得
3. `<body>` の内容のみを置換
4. `<head>` の CSS や JavaScript は再読み込みしない
5. ブラウザの履歴を更新

これにより、SPA のような高速なページ遷移を実現しながら、サーバーサイドレンダリングの利点を維持できます。

### メリット

- **高速なページ遷移**: CSS/JS の再読み込みが不要
- **ネットワーク帯域幅の削減**: `<head>` の重複送信を防ぐ
- **簡単な統合**: 既存の ASP.NET Core アプリに最小限の変更で追加可能
- **プログレッシブエンハンスメント**: JavaScript 無効時でも動作

## セットアップ

### 1. パッケージのインストール

```bash
dotnet add package Turbo.AspNetCore
```

### 2. Tag Helper の登録

`Views/_ViewImports.cshtml` に以下を追加：

```csharp
@addTagHelper *, Turbo.AspNetCore
```

### 3. レイアウトファイルの設定

`Views/Shared/_Layout.cshtml` を更新：

```html
<!DOCTYPE html>
<html lang="ja">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"]</title>
    
    <!-- CSS ファイル -->
    <link rel="stylesheet" href="~/css/site.css" />
    
    <!-- Turbo Drive メタタグ -->
    <turbo-drive-meta enabled="true" transition="fade" />
</head>
<body>
    @RenderBody()
    
    <!-- Turbo.js の読み込み -->
    <script type="module">
        import * as Turbo from 'https://cdn.jsdelivr.net/npm/@hotwired/turbo@8.0.12/+esm';
    </script>
    
    <!-- その他の JavaScript ファイル -->
    <script src="~/js/site.js"></script>
</body>
</html>
```

### 4. 動作確認

開発者ツールのネットワークタブを開いて、ページ遷移時に CSS や JavaScript ファイルが再リクエストされていないことを確認してください。

## Tag Helper リファレンス

### turbo-drive-meta

Turbo Drive の動作を制御するメタタグを生成します。

#### 属性

| 属性 | 型 | デフォルト | 説明 |
|------|-----|-----------|------|
| `enabled` | `bool` | `true` | Turbo Drive を有効/無効にする |
| `transition` | `string` | `""` | ページ遷移時のアニメーション (`"fade"`, `"slide"`, `"none"`) |

#### 使用例

```html
<!-- 基本的な使用 -->
<turbo-drive-meta enabled="true" />

<!-- アニメーション付き -->
<turbo-drive-meta enabled="true" transition="fade" />

<!-- Turbo Drive を無効化 -->
<turbo-drive-meta enabled="false" />
```

#### 生成される HTML

```html
<meta name="turbo-visit-control" content="advance">
<meta name="turbo-refresh-method" content="morph">
<meta name="turbo-refresh-scroll" content="preserve">
<meta name="turbo-transition" content="fade">
```

### turbo-permanent

ページ遷移時に状態を保持する永続的な要素を定義します。

#### 属性

| 属性 | 型 | 必須 | 説明 |
|------|-----|------|------|
| `id` | `string` | ✅ | 要素の一意な ID |

#### 使用例

```html
<!-- 音楽プレーヤー -->
<turbo-permanent id="music-player">
    <audio controls>
        <source src="/audio/music.mp3" type="audio/mpeg">
    </audio>
</turbo-permanent>

<!-- ビデオプレーヤー -->
<turbo-permanent id="video-player">
    <video src="/video/demo.mp4" controls></video>
</turbo-permanent>

<!-- チャットウィジェット -->
<turbo-permanent id="chat-widget">
    <div class="chat-container">
        <!-- チャットの内容 -->
    </div>
</turbo-permanent>
```

#### 生成される HTML

```html
<div id="music-player" data-turbo-permanent="">
    <audio controls>
        <source src="/audio/music.mp3" type="audio/mpeg">
    </audio>
</div>
```

#### 注意事項

- `id` 属性は必須で、アプリケーション全体で一意である必要があります
- 永続的な要素の内容はページ遷移しても保持されますが、DOM 構造が変更されると置換されます
- JavaScript のイベントリスナーは保持されます

## 拡張メソッド

### IsTurboDriveRequest()

Turbo Drive によるリクエストかどうかを判定します。

```csharp
public bool IsTurboDriveRequest(this HttpRequest request)
```

#### 使用例

```csharp
public IActionResult Index()
{
    if (Request.IsTurboDriveRequest())
    {
        // Turbo Drive リクエストの処理
        ViewBag.IsTurboDrive = true;
    }
    
    return View();
}
```

### IsTurboRequest()

Turbo によるリクエスト（Drive/Frame/Stream のいずれか）かどうかを判定します。

```csharp
public bool IsTurboRequest(this HttpRequest request)
```

#### 使用例

```csharp
public IActionResult Index()
{
    if (Request.IsTurboRequest())
    {
        // Turbo を使用している場合の処理
        Response.Headers.Add("X-Turbo-Enabled", "true");
    }
    
    return View();
}
```

### 判定ロジック

- **IsTurboDriveRequest**: `Turbo-Frame` ヘッダーが存在せず、`Accept` ヘッダーに `text/html` が含まれる
- **IsTurboFrameRequest**: `Turbo-Frame` ヘッダーが存在する
- **IsTurboStreamRequest**: `Accept` ヘッダーに `text/vnd.turbo-stream.html` が含まれる
- **IsTurboRequest**: 上記のいずれか

## ベストプラクティス

### 1. JavaScript の初期化

Turbo Drive では、ページ遷移時に `<body>` のみが置換されるため、JavaScript の初期化コードを適切に配置する必要があります。

#### 悪い例

```javascript
// ページ読み込み時に一度だけ実行される
window.onload = function() {
    setupEventListeners();
};
```

#### 良い例

```javascript
// Turbo Drive のイベントを使用
document.addEventListener('turbo:load', function() {
    setupEventListeners();
});

// または、turbo:frame-load イベント
document.addEventListener('turbo:frame-load', function() {
    setupFrameListeners();
});
```

### 2. フォーム送信

Turbo Drive は POST リクエストも自動的に処理しますが、リダイレクトを適切に行う必要があります。

```csharp
[HttpPost]
public IActionResult Create(ProductViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View("New", model);
    }
    
    // 製品を保存
    _productService.Create(model);
    
    // Turbo Drive はリダイレクトを自動的に処理
    return RedirectToAction("Show", new { id = model.Id });
}
```

### 3. エラーハンドリング

エラーページでも Turbo Drive が動作するように設定します。

```csharp
// Startup.cs または Program.cs
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html";
        
        await context.Response.WriteAsync("An error occurred");
    });
});
```

### 4. 認証とリダイレクト

認証が必要なページへのリダイレクトも適切に処理されます。

```csharp
[Authorize]
public IActionResult SecurePage()
{
    // Turbo Drive は自動的にログインページにリダイレクト
    return View();
}
```

### 5. CSS と JavaScript のキャッシュ

Turbo Drive は `<head>` を再読み込みしないため、CSS や JavaScript の変更時にはキャッシュバスティングを使用します。

```html
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
<script src="~/js/site.js" asp-append-version="true"></script>
```

## トラブルシューティング

### 問題: ページ遷移しても JavaScript が動作しない

**原因**: JavaScript が `window.onload` や `DOMContentLoaded` イベントを使用している

**解決策**: Turbo のイベントを使用する

```javascript
document.addEventListener('turbo:load', function() {
    // 初期化コード
});
```

### 問題: フォーム送信後にページが更新されない

**原因**: サーバーが適切なレスポンスを返していない

**解決策**: リダイレクトまたはビューを返す

```csharp
[HttpPost]
public IActionResult Create(Model model)
{
    // 処理...
    
    return RedirectToAction("Index"); // OK
    // または
    return View("Success", model); // OK
}
```

### 問題: 特定のリンクで Turbo Drive を無効にしたい

**解決策**: `data-turbo="false"` 属性を追加

```html
<a href="/external-page" data-turbo="false">外部ページ</a>
```

### 問題: 永続的な要素が保持されない

**原因**: 要素の ID が一意でないか、DOM 構造が変更されている

**解決策**: 
1. ID が一意であることを確認
2. 永続的な要素の DOM 構造を変更しない
3. `data-turbo-permanent` 属性が正しく設定されているか確認

### 問題: ページ遷移が遅い

**原因**: ネットワークが遅い、またはサーバーの応答が遅い

**解決策**:
1. サーバーのパフォーマンスを最適化
2. キャッシュを活用
3. プリフェッチを有効化

```html
<a href="/products" data-turbo-prefetch>製品一覧</a>
```

## Turbo 8 の新機能

### Page Refresh (Morph)

Turbo 8 では、ページ全体を効率的にリフレッシュする新機能が追加されました。

```html
<meta name="turbo-refresh-method" content="morph">
<meta name="turbo-refresh-scroll" content="preserve">
```

これにより、フォーム送信後や定期的なポーリングでページ全体をリフレッシュする際に、スクロール位置やフォームの状態が保持されます。

### Turbo Streams over HTTP

Turbo 8 では、通常の HTTP レスポンスとして Turbo Streams を返すことができます。

```csharp
[HttpPost]
public IActionResult Update(int id, Model model)
{
    // 更新処理...
    
    if (Request.IsTurboStreamRequest())
    {
        return TurboStream("Update", model);
    }
    
    return RedirectToAction("Show", new { id });
}
```

## 参考資料

- [Turbo Drive Handbook](https://turbo.hotwired.dev/handbook/drive)
- [Turbo 8 リリースノート](https://turbo.hotwired.dev/handbook/installing)
- [WireDrive サンプルアプリ](../examples/WireDrive/README.md)
- [実装プラン](turbo-drive-implementation-plan.md)
