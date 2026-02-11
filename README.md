# Hotwire.AspNetCore

ASP.NET Core 向けの [Hotwire](https://hotwired.dev/) 実装ライブラリです。

## 概要

Hotwire は、JavaScript を最小限に抑えながら、高速でモダンな Web アプリケーションを構築するためのアプローチです。このライブラリは、ASP.NET Core アプリケーションで Hotwire を簡単に使用できるようにします。

## 機能

### ✅ Turbo Drive
- **高速なページ遷移**: リンクとフォーム送信を AJAX 化し、ページ全体の再読み込みを防ぐ
- **プログレッシブエンハンスメント**: JavaScript 無効時でも動作
- **永続的な要素**: ページ遷移しても状態を保持する要素（音楽プレーヤーなど）
- **Tag Helper サポート**: `<turbo-drive-meta>` や `<turbo-permanent>` など

### ✅ Turbo Frames
- **部分的なページ更新**: ページの特定部分のみを更新
- **遅延読み込み**: 必要に応じてコンテンツを読み込む

### ✅ Turbo Streams
- **リアルタイム更新**: WebSocket や SSE を使用してページを動的に更新
- **7つの基本アクション**: append, prepend, replace, update, remove, before, after

### ✅ Stimulus (別パッケージ)
- **軽量な JavaScript フレームワーク**: HTML を操作するための最小限の JavaScript

## インストール

```bash
dotnet add package Hotwire.AspNetCore
```

または、個別のパッケージ:

```bash
dotnet add package Turbo.AspNetCore
dotnet add package Stimulus.AspNetCore
```

## クイックスタート

### Turbo Drive の使用

1. **_ViewImports.cshtml に Tag Helper を追加**:

```csharp
@addTagHelper *, Turbo.AspNetCore
```

2. **_Layout.cshtml でメタタグを設定**:

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"]</title>
    
    @* Turbo Drive を有効化 *@
    <turbo-drive-meta enabled="true" transition="fade" />
</head>
<body>
    @RenderBody()
    
    @* Turbo.js を読み込み *@
    <script type="module">
        import * as Turbo from 'https://cdn.jsdelivr.net/npm/@hotwired/turbo@8.0.12/+esm';
    </script>
</body>
```

3. **永続的な要素を作成** (オプション):

```html
<turbo-permanent id="music-player">
    <audio controls>
        <source src="/audio/music.mp3" type="audio/mpeg">
    </audio>
</turbo-permanent>
```

### Turbo Frames の使用

```html
<turbo-frame id="messages">
    <h2>メッセージ</h2>
    <p>ここに最新のメッセージが表示されます</p>
    <a href="/messages/1">メッセージを読む</a>
</turbo-frame>
```

### Turbo Streams の使用

**コントローラー**:

```csharp
using Turbo.AspNetCore;

public class MessagesController : Controller
{
    public IActionResult Create(MessageViewModel model)
    {
        // バリデーション...
        
        if (Request.IsTurboStreamRequest())
        {
            return TurboStream(model);
        }
        
        return RedirectToAction("Index");
    }
}
```

**ビュー (Create.cshtml)**:

```html
<turbo-stream action="append" target="messages">
    <template>
        <div class="message">
            <p>@Model.Content</p>
        </div>
    </template>
</turbo-stream>
```

## サンプルアプリケーション

### WireDrive (Turbo Drive デモ)
高速なページ遷移と永続的な要素のデモ。

```bash
cd examples/WireDrive
dotnet run
```

詳細は [WireDrive README](examples/WireDrive/README.md) を参照。

### WireFrame (Turbo Frames デモ)
部分的なページ更新のデモ。

```bash
cd examples/WireFrame
dotnet run
```

### WireStream (Turbo Streams デモ)
リアルタイム更新のデモ。

```bash
cd examples/WireStream
dotnet run
```

## ドキュメント

- [Turbo Drive 実装プラン](docs/turbo-drive-implementation-plan.md)
- [Hotwire 調査レポート](docs/hotwire-investigation-report.md)

## 要件

- .NET Standard 2.0+ (ライブラリ)
- .NET 6.0+ (サンプルアプリ)

## ビルド

```bash
dotnet build
```

## テスト

```bash
dotnet test
```

## ライセンス

MIT License

## 参考資料

- [Hotwire 公式サイト](https://hotwired.dev/)
- [Turbo Handbook](https://turbo.hotwired.dev/handbook/introduction)
- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
