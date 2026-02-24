# Hotwire.AspNetCore

ASP.NET Core 向けの [Hotwire](https://hotwired.dev/) 実装ライブラリです。

[![Tests](https://img.shields.io/badge/tests-56%20passing-brightgreen)](test/)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0%2B-blue)](https://dotnet.microsoft.com/)

## 概要

Hotwire は、JavaScript を最小限に抑えながら、高速でモダンな Web アプリケーションを構築するためのアプローチです。このライブラリは、ASP.NET Core アプリケーションで Hotwire を簡単に使用できるようにします。

**Rails パリティ達成**: このライブラリは、Ruby on Rails の turbo-rails、ActionCable、stimulus-rails と同等の機能を提供します。

## 機能

### ✅ Turbo Drive
- **高速なページ遷移**: リンクとフォーム送信を AJAX 化し、ページ全体の再読み込みを防ぐ
- **プログレッシブエンハンスメント**: JavaScript 無効時でも動作
- **永続的な要素**: ページ遷移しても状態を保持する要素（音楽プレーヤーなど）
- **Tag Helper サポート**: `<turbo-drive-meta>` や `<turbo-permanent>` など

### ✅ Turbo Frames
- **部分的なページ更新**: ページの特定部分のみを更新
- **遅延読み込み**: 必要に応じてコンテンツを読み込む
- **Tag Helper サポート**: `<turbo-frame>` でフレームを簡単に定義

### ✅ Turbo Streams
- **リアルタイム更新**: WebSocket や SSE を使用してページを動的に更新
- **16の標準アクション**: append, prepend, replace, update, remove, before, after, append_all, prepend_all, replace_all, update_all, remove_all, before_all, after_all, morph, refresh
- **カスタムアクション**: 独自の DOM 操作ロジックを定義可能（Rails パリティ達成）
- **Tag Helper サポート**: `<turbo-stream>` と `<turbo-stream-custom>` で簡単に Turbo Streams を生成
- **SignalR 統合**: リアルタイムブロードキャスト機能（下記参照）

### ✅ Stimulus
完全な Stimulus サポート（別パッケージ `Stimulus.AspNetCore`）:
- **5つの Tag Helper**: Controller, Action, Target, Value, Class
- **9つの HTML 拡張メソッド**: プログラムから Stimulus 属性を生成
- **軽量な JavaScript フレームワーク**: HTML を操作するための最小限の JavaScript
- **20のテスト**: すべてパス
- **サンプルアプリ**: WireStimulus で 5 つのコントローラー例を提供

### ✅ SignalR 統合
SignalR を使用したリアルタイム Turbo Streams:
- **TurboStreamsHub**: WebSocket 接続を管理する SignalR Hub
- **ITurboStreamBroadcaster**: ビューをリアルタイムでブロードキャストするサービス
- **チャネルベースの購読**: 特定のチャネルにのみブロードキャスト
- **自動再接続**: 接続断時の自動リトライ
- **turbo-signalr.js**: クライアント側 JavaScript ライブラリ
- **サンプルアプリ**: WireSignal で通知とチャットのデモを提供

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

### Turbo カスタムアクションの使用

**JavaScript (カスタムアクションの定義)**:

```javascript
// wwwroot/js/custom-actions.js
Turbo.StreamActions.notify = function() {
  const message = this.getAttribute("message");
  const type = this.getAttribute("type") || "info";
  alert(`[${type}] ${message}`);
}
```

**ビュー (Tag Helper)**:

```html
<turbo-stream-custom action="notify" message="保存しました！" type="success"></turbo-stream-custom>
```

**または HTML 拡張メソッド**:

```csharp
@Html.TurboStreamCustom("notify", new { message = "保存しました！", type = "success" })
```

詳細は [Turbo カスタムアクションガイド](docs/turbo-custom-actions-guide.md) を参照。

### Stimulus の使用

**_ViewImports.cshtml に Tag Helper を追加**:

```csharp
@addTagHelper *, Stimulus.AspNetCore
```

**ビュー (Dropdown の例)**:

```html
<div stimulus-controller="dropdown" 
     stimulus-value-dropdown-open="false"
     stimulus-class-dropdown-active="show">
    
    <button stimulus-action="click->dropdown#toggle" 
            class="btn btn-primary">
        ドロップダウンを開く
    </button>
    
    <div stimulus-target="dropdown.menu" 
         class="dropdown-menu">
        <a class="dropdown-item" href="#">アクション</a>
        <a class="dropdown-item" href="#">別のアクション</a>
    </div>
</div>
```

**JavaScript (Stimulus コントローラー)**:

```javascript
// wwwroot/js/controllers/dropdown_controller.js
import { Controller } from "@hotwired/stimulus"

export default class extends Controller {
  static targets = ["menu"]
  static classes = ["active"]
  static values = { open: Boolean }

  toggle(event) {
    event.preventDefault()
    this.openValue = !this.openValue
  }

  openValueChanged() {
    if (this.openValue) {
      this.menuTarget.classList.add(this.activeClass)
    } else {
      this.menuTarget.classList.remove(this.activeClass)
    }
  }
}
```

詳細は [Stimulus.AspNetCore README](src/Stimulus.AspNetCore/README.md) を参照。

### SignalR によるリアルタイム Turbo Streams

**Program.cs での設定**:

```csharp
using Turbo.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// SignalR を追加
builder.Services.AddSignalR();

// Turbo Stream Broadcaster を追加
builder.Services.AddTurboStreamBroadcaster();

var app = builder.Build();

// SignalR Hub をマップ
app.MapHub<TurboStreamsHub>("/hubs/turbo-streams");

app.Run();
```

**コントローラーでのブロードキャスト**:

```csharp
using Turbo.AspNetCore;

public class NotificationsController : Controller
{
    private readonly ITurboStreamBroadcaster _broadcaster;

    public NotificationsController(ITurboStreamBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Notification notification)
    {
        // すべての購読者にブロードキャスト
        await _broadcaster.BroadcastViewAsync(
            "notifications",     // チャネル名
            "_Notification",     // パーシャルビュー
            notification         // モデル
        );

        return this.TurboStream("_Notification", notification);
    }
}
```

**クライアント側の接続 (JavaScript)**:

```javascript
// turbo-signalr.js を読み込み後
const turboSignalR = new TurboSignalR();
await turboSignalR.start();
await turboSignalR.subscribe('notifications');

// イベントリスナー
document.addEventListener('turbo-signalr:streamReceived', (event) => {
    console.log('リアルタイム更新を受信！');
});
```

詳細は [SignalR 統合ガイド](docs/turbo-streams-signalr-guide.md) と [WireSignal サンプル](examples/WireSignal/README.md) を参照。

## サンプルアプリケーション

このリポジトリには、Hotwire の各機能を実演する 5 つのサンプルアプリケーションが含まれています。

### 1. WireDrive - Turbo Drive デモ
高速なページ遷移と永続的な要素のデモ。

```bash
cd examples/WireDrive
dotnet run
```

**主な機能**:
- 高速なページ遷移（リロード不要）
- 永続的な要素（音楽プレーヤーなど）
- プログレッシブエンハンスメント

詳細は [WireDrive README](examples/WireDrive/README.md) を参照。

### 2. WireFrame - Turbo Frames デモ
部分的なページ更新のデモ。

```bash
cd examples/WireFrame
dotnet run
```

**主な機能**:
- 部分的なページ更新
- 遅延読み込み
- ネストされたフレーム

### 3. WireStream - Turbo Streams デモ
リアルタイム更新とカスタムアクションのデモ。

```bash
cd examples/WireStream
dotnet run
# http://localhost:5000/CustomActions にアクセス
```

**主な機能**:
- 16 の標準 Turbo Stream アクション
- 5 つのカスタムアクション（set_title, notify, slide_in, highlight, console_log）
- DOM 操作のデモ

### 4. WireStimulus - Stimulus デモ
Stimulus コントローラーの包括的なデモ。

```bash
cd examples/WireStimulus
dotnet run
```

**主な機能**:
- 5 つの実用的な Stimulus コントローラー
  - **Dropdown**: トグル＋自動クローズ
  - **Clipboard**: クリップボードコピー＋フィードバック
  - **Counter**: インクリメント/デクリメント
  - **Form**: リアルタイムバリデーション
  - **Slideshow**: 画像カルーセル＋オートプレイ

詳細は [WireStimulus README](examples/WireStimulus/README.md) を参照。

### 5. WireSignal - SignalR 統合デモ
SignalR を使用したリアルタイム Turbo Streams のデモ。

```bash
cd examples/WireSignal
dotnet run
```

**主な機能**:
- リアルタイム通知システム
- ライブチャット
- SignalR による WebSocket 接続
- チャネルベースの購読

詳細は [WireSignal README](examples/WireSignal/README.md) を参照。

## ドキュメント

### ガイド
- [Turbo Drive ガイド](docs/turbo-drive-guide.md) - Turbo Drive の使い方
- [Turbo カスタムアクションガイド](docs/turbo-custom-actions-guide.md) - カスタムアクションの実装方法
- [SignalR 統合ガイド](docs/turbo-streams-signalr-guide.md) - SignalR によるリアルタイム Turbo Streams

### 実装ドキュメント
- [Hotwire 調査レポート](docs/hotwire-investigation-report.md) - 詳細な実装状況と Rails パリティの評価
- [Turbo カスタムアクション実装プラン](docs/turbo-custom-actions-plan.md) - カスタムアクションの設計と実装計画
- [Stimulus.AspNetCore README](src/Stimulus.AspNetCore/README.md) - Stimulus Tag Helper の完全なドキュメント

### サンプルアプリケーション
- [WireDrive README](examples/WireDrive/README.md) - Turbo Drive の例
- [WireStimulus README](examples/WireStimulus/README.md) - Stimulus の包括的な例（5 つのコントローラー）
- [WireSignal README](examples/WireSignal/README.md) - SignalR 統合の例

## 要件

- .NET Standard 2.0+ (ライブラリ)
- .NET 6.0+ (サンプルアプリ)

## パッケージ

このリポジトリには 3 つのパッケージが含まれています:

### 1. Hotwire.AspNetCore
すべての機能を含む統合パッケージ（Turbo + Stimulus）

```bash
dotnet add package Hotwire.AspNetCore
```

### 2. Turbo.AspNetCore
Turbo Drive/Frames/Streams の実装

```bash
dotnet add package Turbo.AspNetCore
```

### 3. Stimulus.AspNetCore
Stimulus Tag Helper と HTML 拡張

```bash
dotnet add package Stimulus.AspNetCore
```

## ビルド

```bash
dotnet build
```

## テスト

```bash
dotnet test
```

**テスト結果**: 56 テスト（36 Turbo + 20 Stimulus）すべてパス ✅

## プロジェクト構成

```
Hotwire.AspNetCore/
├── src/
│   ├── Hotwire.AspNetCore/      # 統合パッケージ
│   ├── Turbo.AspNetCore/         # Turbo 実装
│   │   ├── TagHelpers/           # Turbo Tag Helper (6個)
│   │   ├── Hubs/                 # SignalR Hub
│   │   └── wwwroot/js/           # turbo-signalr.js
│   └── Stimulus.AspNetCore/      # Stimulus 実装
│       └── TagHelpers/           # Stimulus Tag Helper (5個)
├── test/
│   ├── Turbo.AspNetCore.Test/    # Turbo テスト (36)
│   └── Stimulus.AspNetCore.Test/ # Stimulus テスト (20)
├── examples/
│   ├── WireDrive/                # Turbo Drive デモ
│   ├── WireFrame/                # Turbo Frames デモ
│   ├── WireStream/               # Turbo Streams デモ
│   ├── WireStimulus/             # Stimulus デモ
│   └── WireSignal/               # SignalR デモ
└── docs/                         # ドキュメント
```

## 機能比較

| 機能 | Rails (turbo-rails) | Hotwire.AspNetCore | 状態 |
|------|---------------------|-------------------|------|
| Turbo Drive | ✅ | ✅ | 完全実装 |
| Turbo Frames | ✅ | ✅ | 完全実装 |
| Turbo Streams (標準アクション) | ✅ 16アクション | ✅ 16アクション | Rails パリティ |
| Turbo Streams (カスタムアクション) | ✅ turbo_stream.action() | ✅ TurboStreamCustom | Rails パリティ |
| Turbo 8 (morph/refresh) | ✅ | ✅ | 完全実装 |
| リアルタイムストリーム | ✅ ActionCable | ✅ SignalR | Rails パリティ |
| Stimulus | ✅ stimulus-rails | ✅ Stimulus.AspNetCore | Rails パリティ |
| Tag Helpers | ✅ Rails Helpers | ✅ ASP.NET Tag Helpers | ASP.NET 最適化 |

## ライセンス

MIT License

## 参考資料

- [Hotwire 公式サイト](https://hotwired.dev/)
- [Turbo Handbook](https://turbo.hotwired.dev/handbook/introduction)
- [Stimulus Handbook](https://stimulus.hotwired.dev/handbook/introduction)
