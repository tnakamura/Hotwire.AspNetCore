# Turbo Streams + SignalR 実装計画

**作成日**: 2026年2月11日  
**目的**: ASP.NET Core SignalR を使用したリアルタイム Turbo Streams の実装  
**ベースドキュメント**: docs/hotwire-investigation-report.md Section 5.3.A

---

## 1. エグゼクティブサマリー

本ドキュメントは、Rails の ActionCable に相当する SignalR 統合を通じて、Hotwire.AspNetCore にリアルタイム更新機能を実装する詳細な計画を提供します。

### 主な目標

- ✅ SignalR Hub による WebSocket ベースのリアルタイム通信
- ✅ Turbo Streams フォーマットでの DOM 更新のブロードキャスト
- ✅ チャンネル/グループによる対象クライアントの制御
- ✅ 開発者フレンドリーな API（Controller 拡張メソッド）
- ✅ 自動再接続とエラーハンドリング
- ✅ 実践的なサンプルアプリケーション

---

## 2. アーキテクチャ設計

### 2.1 全体構成

```
┌─────────────────┐
│   Blazor/MVC    │
│   Controller    │
└────────┬────────┘
         │ 1. BroadcastTurboStream()
         ↓
┌─────────────────┐
│ TurboStreamsHub │ ←→ SignalR Connection
└────────┬────────┘
         │ 2. SendAsync("ReceiveTurboStream", html)
         ↓
┌─────────────────┐
│  Browser        │
│  (JavaScript)   │ 3. Turbo.renderStreamMessage()
└─────────────────┘
```

### 2.2 コンポーネント

| コンポーネント | 役割 | 実装ファイル |
|------------|------|------------|
| `TurboStreamsHub` | SignalR Hub - リアルタイム通信の中核 | `src/Turbo.AspNetCore/Hubs/TurboStreamsHub.cs` |
| `TurboSignalRExtensions` | Controller/Service から Hub への簡単なアクセス | `src/Turbo.AspNetCore/TurboSignalRExtensions.cs` |
| `ITurboStreamBroadcaster` | ブロードキャスト用のサービスインターフェース | `src/Turbo.AspNetCore/ITurboStreamBroadcaster.cs` |
| `TurboStreamBroadcaster` | ブロードキャストサービスの実装 | `src/Turbo.AspNetCore/TurboStreamBroadcaster.cs` |
| `turbo-signalr.js` | クライアントサイド JavaScript | `src/Turbo.AspNetCore/wwwroot/js/turbo-signalr.js` |

---

## 3. 実装詳細

### 3.1 SignalR Hub (TurboStreamsHub)

**ファイル**: `src/Turbo.AspNetCore/Hubs/TurboStreamsHub.cs`

```csharp
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Turbo.AspNetCore.Hubs
{
    /// <summary>
    /// SignalR Hub for broadcasting Turbo Stream updates to connected clients.
    /// </summary>
    public class TurboStreamsHub : Hub
    {
        /// <summary>
        /// Subscribes the connection to a specific channel.
        /// </summary>
        /// <param name="channel">Channel name to subscribe to</param>
        public async Task SubscribeToChannel(string channel)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channel);
        }

        /// <summary>
        /// Unsubscribes the connection from a specific channel.
        /// </summary>
        /// <param name="channel">Channel name to unsubscribe from</param>
        public async Task UnsubscribeFromChannel(string channel)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);
        }

        /// <summary>
        /// Called when a client connects to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
```

### 3.2 Broadcaster Service (ITurboStreamBroadcaster)

**ファイル**: `src/Turbo.AspNetCore/ITurboStreamBroadcaster.cs`

```csharp
using System.Threading.Tasks;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Service for broadcasting Turbo Stream updates via SignalR.
    /// </summary>
    public interface ITurboStreamBroadcaster
    {
        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to all clients in a channel.
        /// </summary>
        /// <param name="channel">Channel name to broadcast to</param>
        /// <param name="turboStreamHtml">Turbo Stream HTML content</param>
        Task BroadcastAsync(string channel, string turboStreamHtml);

        /// <summary>
        /// Broadcasts a Turbo Stream by rendering a view to all clients in a channel.
        /// </summary>
        /// <param name="channel">Channel name to broadcast to</param>
        /// <param name="viewName">View name to render</param>
        /// <param name="model">Model for the view</param>
        Task BroadcastViewAsync(string channel, string viewName, object model = null);
    }
}
```

### 3.3 Controller 拡張メソッド

**ファイル**: `src/Turbo.AspNetCore/TurboSignalRExtensions.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Turbo.AspNetCore.Hubs;
using System.Threading.Tasks;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Extension methods for broadcasting Turbo Streams via SignalR.
    /// </summary>
    public static class TurboSignalRExtensions
    {
        /// <summary>
        /// Broadcasts a Turbo Stream view to all clients in a channel.
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="hubContext">SignalR hub context</param>
        /// <param name="channel">Channel name to broadcast to</param>
        /// <param name="viewName">View name to render</param>
        /// <param name="model">Model for the view</param>
        public static async Task BroadcastTurboStreamAsync(
            this Controller controller,
            IHubContext<TurboStreamsHub> hubContext,
            string channel,
            string viewName,
            object model = null)
        {
            // View rendering logic will be implemented
            var html = await RenderViewToStringAsync(controller, viewName, model);
            await hubContext.Clients.Group(channel).SendAsync("ReceiveTurboStream", html);
        }

        // Helper method to render view to string
        private static async Task<string> RenderViewToStringAsync(
            Controller controller,
            string viewName,
            object model)
        {
            // Implementation details...
            throw new System.NotImplementedException();
        }
    }
}
```

### 3.4 クライアントサイド JavaScript

**ファイル**: `src/Turbo.AspNetCore/wwwroot/js/turbo-signalr.js`

```javascript
/**
 * Turbo Streams + SignalR Integration
 * 
 * Connects to the SignalR Hub and receives Turbo Stream updates.
 */

class TurboSignalRConnection {
    constructor(hubUrl = '/hubs/turbo-streams') {
        this.hubUrl = hubUrl;
        this.connection = null;
        this.channels = new Set();
    }

    /**
     * Starts the SignalR connection
     */
    async start() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .withAutomaticReconnect()
            .build();

        this.connection.on('ReceiveTurboStream', (html) => {
            this.handleTurboStream(html);
        });

        try {
            await this.connection.start();
            console.log('Turbo SignalR connected');
        } catch (err) {
            console.error('Error starting Turbo SignalR:', err);
            setTimeout(() => this.start(), 5000); // Retry after 5 seconds
        }
    }

    /**
     * Subscribes to a channel
     */
    async subscribe(channel) {
        if (!this.channels.has(channel)) {
            await this.connection.invoke('SubscribeToChannel', channel);
            this.channels.add(channel);
            console.log(`Subscribed to channel: ${channel}`);
        }
    }

    /**
     * Unsubscribes from a channel
     */
    async unsubscribe(channel) {
        if (this.channels.has(channel)) {
            await this.connection.invoke('UnsubscribeFromChannel', channel);
            this.channels.delete(channel);
            console.log(`Unsubscribed from channel: ${channel}`);
        }
    }

    /**
     * Handles incoming Turbo Stream HTML
     */
    handleTurboStream(html) {
        if (window.Turbo) {
            Turbo.renderStreamMessage(html);
        } else {
            console.error('Turbo is not available');
        }
    }

    /**
     * Stops the connection
     */
    async stop() {
        if (this.connection) {
            await this.connection.stop();
            console.log('Turbo SignalR disconnected');
        }
    }
}

// Export for use in applications
window.TurboSignalR = TurboSignalRConnection;
```

---

## 4. 使用例

### 4.1 Program.cs での設定

```csharp
using Turbo.AspNetCore.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR
builder.Services.AddSignalR();

// Add Turbo Stream Broadcaster service
builder.Services.AddScoped<ITurboStreamBroadcaster, TurboStreamBroadcaster>();

var app = builder.Build();

// Map SignalR Hub
app.MapHub<TurboStreamsHub>("/hubs/turbo-streams");

app.Run();
```

### 4.2 _Layout.cshtml でのクライアント設定

```html
<head>
    <!-- SignalR Client -->
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
    
    <!-- Turbo -->
    <script src="https://cdn.jsdelivr.net/npm/@hotwired/turbo@latest"></script>
    
    <!-- Turbo SignalR Integration -->
    <script src="~/js/turbo-signalr.js"></script>
</head>
<body>
    @RenderBody()
    
    <script>
        // Initialize Turbo SignalR connection
        const turboSignalR = new TurboSignalR();
        turboSignalR.start();
        
        // Subscribe to channels as needed
        turboSignalR.subscribe('notifications');
    </script>
</body>
```

### 4.3 Controller での使用

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Turbo.AspNetCore.Hubs;

public class NotificationsController : Controller
{
    private readonly IHubContext<TurboStreamsHub> _hubContext;

    public NotificationsController(IHubContext<TurboStreamsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Notification notification)
    {
        // Save notification to database
        // ...

        // Broadcast to all subscribed clients
        await this.BroadcastTurboStreamAsync(
            _hubContext,
            "notifications",
            "_Notification",
            notification
        );

        return RedirectToAction("Index");
    }
}
```

### 4.4 Turbo Stream View (_Notification.cshtml)

```html
<turbo-stream action="append" target="notifications">
    <template>
        <div class="notification">
            <h4>@Model.Title</h4>
            <p>@Model.Message</p>
        </div>
    </template>
</turbo-stream>
```

---

## 5. サンプルアプリケーション (WireSignal)

### 5.1 構成

```
examples/WireSignal/
├── Controllers/
│   ├── HomeController.cs
│   ├── ChatController.cs
│   └── NotificationsController.cs
├── Views/
│   ├── Home/
│   ├── Chat/
│   └── Notifications/
├── wwwroot/
│   └── css/
├── Program.cs
└── README.md
```

### 5.2 実装するデモ機能

1. **リアルタイムチャット**
   - メッセージ投稿のリアルタイムブロードキャスト
   - ユーザー参加/退室の通知
   - 未読カウントの更新

2. **リアルタイム通知システム**
   - システム通知の配信
   - ユーザー固有の通知
   - 通知バッジの更新

3. **リアルタイムダッシュボード**
   - 統計データの自動更新
   - アクティブユーザー数の表示
   - グラフのリアルタイム更新

---

## 6. テスト戦略

### 6.1 単体テスト

**ファイル**: `test/Turbo.AspNetCore.Test/TurboStreamsHubTest.cs`

```csharp
using Xunit;
using Microsoft.AspNetCore.SignalR;
using Turbo.AspNetCore.Hubs;

public class TurboStreamsHubTest
{
    [Fact]
    public async Task SubscribeToChannel_AddsConnectionToGroup()
    {
        // Arrange
        var hub = new TurboStreamsHub();
        
        // Act & Assert
        // Test implementation
    }
}
```

### 6.2 統合テスト

- SignalR 接続のエンドツーエンドテスト
- チャンネル購読/解除のテスト
- ブロードキャストメッセージの受信テスト

---

## 7. マイルストーン

### Phase 1: 基本実装 (完了目標: 即日)
- [x] 実装計画ドキュメント作成
- [ ] TurboStreamsHub 実装
- [ ] ITurboStreamBroadcaster インターフェース定義
- [ ] 基本的な拡張メソッド実装

### Phase 2: クライアント統合 (完了目標: 即日)
- [ ] JavaScript クライアントライブラリ作成
- [ ] 自動再接続機能
- [ ] エラーハンドリング

### Phase 3: サンプルアプリ (完了目標: 即日)
- [ ] WireSignal プロジェクト作成
- [ ] チャットデモ実装
- [ ] 通知システムデモ実装
- [ ] README とドキュメント作成

### Phase 4: テストと検証 (完了目標: 即日)
- [ ] 単体テスト追加
- [ ] 統合テスト追加
- [ ] ビルドとテスト実行
- [ ] ドキュメント最終レビュー

---

## 8. 技術的考慮事項

### 8.1 SignalR トランスポート

SignalR は以下のトランスポートを自動ネゴシエーション:
1. **WebSocket** (推奨) - 双方向、低レイテンシ
2. **Server-Sent Events (SSE)** - サーバー→クライアント
3. **Long Polling** - フォールバック

### 8.2 スケーラビリティ

複数サーバー環境では、SignalR バックプレーンが必要:
- **Azure SignalR Service**
- **Redis Backplane**
- **SQL Server Backplane**

### 8.3 セキュリティ

- チャンネル購読の認証/認可
- CORS 設定
- 接続トークンの検証

---

## 9. 参考資料

- [Hotwire 公式ドキュメント](https://hotwired.dev/)
- [SignalR 公式ドキュメント](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Rails ActionCable ガイド](https://guides.rubyonrails.org/action_cable_overview.html)
- [Turbo Streams リファレンス](https://turbo.hotwired.dev/handbook/streams)

---

## 10. まとめ

この実装により、Hotwire.AspNetCore は Rails の ActionCable に匹敵するリアルタイム機能を獲得し、以下のユースケースに対応可能になります:

✅ リアルタイムチャット  
✅ 通知システム  
✅ 協業編集ツール  
✅ ライブダッシュボード  
✅ ライブフィード更新  

**次のステップ**: Phase 1 の実装を開始します。
