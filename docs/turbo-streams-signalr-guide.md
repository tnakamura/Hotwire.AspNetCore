# Turbo Streams + SignalR 使用ガイド

## 概要

このガイドでは、ASP.NET Core で SignalR を使用してリアルタイム Turbo Streams を実装する方法を説明します。

## インストール

### NuGet パッケージ

```bash
dotnet add package Turbo.AspNetCore
```

SignalR は ASP.NET Core に組み込まれているため、追加のパッケージは不要です。

## 基本セットアップ

### 1. Program.cs の設定

```csharp
using Turbo.AspNetCore;
using Turbo.AspNetCore.Hubs;

var builder = WebApplication.CreateBuilder(args);

// MVC サービスの追加
builder.Services.AddControllersWithViews();

// SignalR サービスの追加
builder.Services.AddSignalR();

// Turbo Stream Broadcaster サービスの追加
builder.Services.AddScoped<ITurboStreamBroadcaster, TurboStreamBroadcaster>();

var app = builder.Build();

// SignalR Hub のマッピング
app.MapHub<TurboStreamsHub>("/hubs/turbo-streams");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### 2. _Layout.cshtml の設定

```html
<head>
    <!-- SignalR クライアントライブラリ -->
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
    
    <!-- Turbo -->
    <script src="https://cdn.jsdelivr.net/npm/@hotwired/turbo@latest"></script>
    
    <!-- Turbo SignalR 統合 -->
    <script src="~/js/turbo-signalr.js"></script>
</head>
<body>
    @RenderBody()
    
    <script>
        // SignalR 接続の初期化
        const turboSignalR = new TurboSignalR();
        turboSignalR.start();
    </script>
    
    @RenderSection("Scripts", required: false)
</body>
```

### 3. JavaScript ファイルのコピー

`src/Turbo.AspNetCore/wwwroot/js/turbo-signalr.js` をアプリケーションの `wwwroot/js/` ディレクトリにコピーします。

## 基本的な使い方

### Controller でブロードキャストする

```csharp
using Microsoft.AspNetCore.Mvc;
using Turbo.AspNetCore;
using YourApp.Models;

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
        // データベースに保存など...
        await SaveNotificationAsync(notification);

        // チャンネルにブロードキャスト
        await _broadcaster.BroadcastViewAsync(
            "notifications",      // チャンネル名
            "_Notification",      // 部分ビュー名
            notification          // モデル
        );

        // Turbo リクエストの場合は Turbo Stream を返す
        if (Request.IsTurboRequest())
        {
            return this.TurboStream("_Notification", notification);
        }

        return RedirectToAction("Index");
    }
}
```

### Turbo Stream 部分ビュー

**Views/Notifications/_Notification.cshtml**:
```html
@model Notification

<turbo-stream action="append" target="notifications">
    <template>
        <div class="notification" id="notification-@Model.Id">
            <h4>@Model.Title</h4>
            <p>@Model.Message</p>
            <small>@Model.CreatedAt.ToString("HH:mm:ss")</small>
        </div>
    </template>
</turbo-stream>
```

### クライアントサイドの購読

**Views/Notifications/Index.cshtml**:
```html
<h1>通知</h1>

<div id="notifications">
    <!-- ここに通知が追加される -->
</div>

@section Scripts {
<script>
    // SignalR 接続を開始し、チャンネルに購読
    const turboSignalR = new TurboSignalR();
    turboSignalR.start().then(() => {
        turboSignalR.subscribe('notifications');
    });
</script>
}
```

## 高度な使い方

### 複数チャンネルの購読

```javascript
const turboSignalR = new TurboSignalR();
await turboSignalR.start();

// 複数のチャンネルに購読
await turboSignalR.subscribe('notifications');
await turboSignalR.subscribe('chat');
await turboSignalR.subscribe('dashboard');
```

### 特定の接続にのみブロードキャスト

```csharp
// 特定のユーザーにのみ送信
await _broadcaster.BroadcastToConnectionAsync(
    connectionId,
    turboStreamHtml
);
```

### 全クライアントにブロードキャスト

```csharp
// すべての接続クライアントに送信
await _broadcaster.BroadcastToAllAsync(turboStreamHtml);
```

### 生の HTML をブロードキャスト

```csharp
var html = @"
<turbo-stream action=""append"" target=""messages"">
    <template>
        <div>新しいメッセージ</div>
    </template>
</turbo-stream>";

await _broadcaster.BroadcastAsync("chat", html);
```

## イベントハンドリング

### 接続イベントのリスニング

```javascript
// 接続成功
document.addEventListener('turbo-signalr:connected', () => {
    console.log('SignalR に接続しました');
});

// 再接続中
document.addEventListener('turbo-signalr:reconnecting', (event) => {
    console.log('再接続中...', event.detail.error);
});

// 再接続成功
document.addEventListener('turbo-signalr:reconnected', () => {
    console.log('再接続しました');
});

// 接続切断
document.addEventListener('turbo-signalr:closed', (event) => {
    console.error('接続が切断されました', event.detail.error);
});

// エラー
document.addEventListener('turbo-signalr:error', (event) => {
    console.error('エラーが発生しました', event.detail.error);
});
```

### ストリーム受信イベント

```javascript
document.addEventListener('turbo-signalr:streamReceived', (event) => {
    console.log('Turbo Stream を受信しました', event.detail.html);
    
    // カスタム処理（例：サウンド再生、通知バッジ更新など）
    playNotificationSound();
    updateBadgeCount();
});
```

### チャンネルイベント

```javascript
// チャンネル購読完了
document.addEventListener('turbo-signalr:subscribed', (event) => {
    console.log('チャンネルに購読しました:', event.detail.channel);
});

// チャンネル購読解除完了
document.addEventListener('turbo-signalr:unsubscribed', (event) => {
    console.log('チャンネルから購読解除しました:', event.detail.channel);
});
```

## Turbo Stream アクション

サポートされている Turbo Stream アクション:

| アクション | 説明 | 使用例 |
|----------|------|--------|
| `append` | ターゲット要素の末尾に追加 | チャットメッセージ、通知 |
| `prepend` | ターゲット要素の先頭に追加 | 新着情報 |
| `replace` | ターゲット要素を置換 | 更新されたコンテンツ |
| `update` | ターゲット要素の内容を更新 | カウンター、ステータス |
| `remove` | ターゲット要素を削除 | 削除された項目 |
| `before` | ターゲット要素の前に挿入 | - |
| `after` | ターゲット要素の後に挿入 | - |

## ベストプラクティス

### 1. チャンネル命名規則

```csharp
// 良い例
await turboSignalR.subscribe('notifications');
await turboSignalR.subscribe('chat:room-123');
await turboSignalR.subscribe('user:456');

// 避けるべき例
await turboSignalR.subscribe('general');
await turboSignalR.subscribe('all');
```

### 2. エラーハンドリング

```csharp
public async Task<IActionResult> Create(Notification notification)
{
    try
    {
        await _broadcaster.BroadcastViewAsync("notifications", "_Notification", notification);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "ブロードキャストに失敗しました");
        // エラー処理...
    }
    
    return this.TurboStream("_Notification", notification);
}
```

### 3. パフォーマンス最適化

```csharp
// ビューのキャッシュ
private readonly IMemoryCache _cache;

public async Task BroadcastCachedView(string channel, Notification notification)
{
    var cacheKey = $"notification-view-{notification.Type}";
    
    if (!_cache.TryGetValue(cacheKey, out string html))
    {
        html = await RenderViewAsync("_Notification", notification);
        _cache.Set(cacheKey, html, TimeSpan.FromMinutes(5));
    }
    
    await _broadcaster.BroadcastAsync(channel, html);
}
```

### 4. セキュリティ

```csharp
// チャンネルアクセスの認証
public class SecureTurboStreamsHub : TurboStreamsHub
{
    private readonly IAuthorizationService _authService;

    public SecureTurboStreamsHub(IAuthorizationService authService)
    {
        _authService = authService;
    }

    public override async Task SubscribeToChannel(string channel)
    {
        var user = Context.User;
        
        if (!await _authService.CanAccessChannelAsync(user, channel))
        {
            throw new UnauthorizedAccessException($"チャンネル '{channel}' にアクセスできません");
        }
        
        await base.SubscribeToChannel(channel);
    }
}
```

## トラブルシューティング

### 接続できない

1. SignalR Hub が正しくマッピングされているか確認:
```csharp
app.MapHub<TurboStreamsHub>("/hubs/turbo-streams");
```

2. JavaScript ライブラリが正しく読み込まれているか確認:
```html
<script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
```

3. ブラウザの開発者コンソールでエラーを確認

### 更新が表示されない

1. チャンネル名が一致しているか確認
2. ターゲット要素の ID が存在するか確認
3. Turbo Stream の構文が正しいか確認

### 再接続の問題

```javascript
// 再接続遅延をカスタマイズ
const turboSignalR = new TurboSignalR('/hubs/turbo-streams', {
    reconnectDelay: 10000  // 10秒
});
```

## 本番環境での考慮事項

### スケールアウト（複数サーバー）

Azure SignalR Service を使用:
```csharp
builder.Services.AddSignalR()
    .AddAzureSignalR(Configuration["Azure:SignalR:ConnectionString"]);
```

Redis バックプレーンを使用:
```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis(Configuration["Redis:ConnectionString"]);
```

### 接続数の制限

```csharp
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 32 * 1024; // 32 KB
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
```

## サンプルアプリケーション

完全なサンプルは `examples/WireSignal` を参照してください:

```bash
cd examples/WireSignal
dotnet run
```

ブラウザで `https://localhost:5001` を開き、以下の機能を試してください:
- リアルタイム通知
- リアルタイムチャット

## 参考資料

- [SignalR ドキュメント](https://learn.microsoft.com/ja-jp/aspnet/core/signalr/)
- [Turbo Streams リファレンス](https://turbo.hotwired.dev/handbook/streams)
- [実装計画](turbo-streams-signalr-plan.md)
- [WireSignal README](../examples/WireSignal/README.md)
