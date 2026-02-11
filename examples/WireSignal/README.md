# WireSignal - Real-time Turbo Streams with SignalR

WireSignal demonstrates how to use **SignalR** with **Turbo Streams** to create real-time, interactive web applications without writing complex JavaScript.

## Features

### 🔴 Real-time Notifications
- Broadcast notifications to all connected clients instantly
- Different notification types (info, success, warning, error)
- Automatic UI updates via Turbo Streams

### 💬 Live Chat
- Real-time chat messages using WebSocket
- No page refreshes required
- Automatic scroll to new messages

### 🔌 SignalR Integration
- Automatic reconnection on connection loss
- Channel-based subscriptions
- Exponential backoff retry logic

## Architecture

```
┌─────────────────┐
│   Controller    │ 1. BroadcastViewAsync()
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│ Broadcaster     │ 2. Render View → SendAsync()
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│ TurboStreamsHub │ 3. Group broadcast via SignalR
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│ JavaScript      │ 4. Turbo.renderStreamMessage()
│ (Browser)       │
└─────────────────┘
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Modern browser with WebSocket support

### Running the Application

1. Navigate to the WireSignal directory:
```bash
cd examples/WireSignal
```

2. Run the application:
```bash
dotnet run
```

3. Open multiple browser windows at `https://localhost:5001` to see real-time updates

### Testing Real-time Features

#### Notifications
1. Navigate to `/Notifications`
2. Open the same page in multiple browser tabs
3. Send a notification from one tab
4. Watch it appear instantly in all tabs

#### Chat
1. Navigate to `/Chat`
2. Open the same page in multiple browser tabs
3. Send a message from one tab
4. Watch it appear instantly in all tabs

## Code Structure

### Backend (C#)

**Program.cs** - SignalR and Turbo configuration:
```csharp
// Add SignalR
builder.Services.AddSignalR();

// Add Turbo Stream Broadcaster
builder.Services.AddScoped<ITurboStreamBroadcaster, TurboStreamBroadcaster>();

// Map SignalR Hub
app.MapHub<TurboStreamsHub>("/hubs/turbo-streams");
```

**Controller** - Broadcasting updates:
```csharp
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
        // Broadcast to all subscribers
        await _broadcaster.BroadcastViewAsync(
            "notifications",     // Channel name
            "_Notification",     // Partial view
            notification         // Model
        );

        return this.TurboStream("_Notification", notification);
    }
}
```

**Turbo Stream Partial View** - _Notification.cshtml:
```html
@model Notification

<turbo-stream action="append" target="notifications">
    <template>
        <div class="notification">
            <strong>@Model.Title</strong>
            <p>@Model.Message</p>
        </div>
    </template>
</turbo-stream>
```

### Frontend (JavaScript)

**Client-side connection** - In view:
```javascript
// Initialize connection
const turboSignalR = new TurboSignalR();
await turboSignalR.start();

// Subscribe to channel
await turboSignalR.subscribe('notifications');

// Listen to events
document.addEventListener('turbo-signalr:streamReceived', (event) => {
    console.log('Update received!');
});
```

## Key Components

| Component | Purpose |
|-----------|---------|
| `TurboStreamsHub` | SignalR Hub for managing connections |
| `ITurboStreamBroadcaster` | Service for broadcasting Turbo Streams |
| `TurboStreamBroadcaster` | Implementation of broadcaster service |
| `turbo-signalr.js` | Client-side JavaScript library |
| `TurboSignalRExtensions` | Controller extension methods |

## API Reference

### ITurboStreamBroadcaster

```csharp
// Broadcast to a channel
Task BroadcastAsync(string channel, string turboStreamHtml);

// Broadcast view to a channel
Task BroadcastViewAsync(string channel, string viewName, object model = null);

// Broadcast to specific connection
Task BroadcastToConnectionAsync(string connectionId, string turboStreamHtml);

// Broadcast to all clients
Task BroadcastToAllAsync(string turboStreamHtml);
```

### TurboSignalRConnection (JavaScript)

```javascript
// Start connection
await turboSignalR.start();

// Subscribe to channel
await turboSignalR.subscribe('channel-name');

// Unsubscribe from channel
await turboSignalR.unsubscribe('channel-name');

// Stop connection
await turboSignalR.stop();

// Check connection state
const isConnected = turboSignalR.isConnected();
```

## Events

The JavaScript client emits custom events:

- `turbo-signalr:connected` - Connection established
- `turbo-signalr:reconnecting` - Attempting to reconnect
- `turbo-signalr:reconnected` - Reconnection successful
- `turbo-signalr:disconnected` - Connection closed
- `turbo-signalr:subscribed` - Subscribed to channel
- `turbo-signalr:unsubscribed` - Unsubscribed from channel
- `turbo-signalr:streamReceived` - Turbo Stream update received
- `turbo-signalr:error` - Error occurred

## Comparison with Rails ActionCable

| Feature | Rails ActionCable | Hotwire.AspNetCore SignalR |
|---------|------------------|----------------------------|
| Transport | WebSocket, fallback to polling | WebSocket, SSE, Long Polling |
| Channel Subscription | ✅ | ✅ |
| Broadcasting | `ActionCable.server.broadcast` | `ITurboStreamBroadcaster.BroadcastAsync` |
| Auto-reconnect | ✅ | ✅ |
| Turbo Streams | ✅ | ✅ |
| Server-side rendering | ✅ | ✅ |

## Production Considerations

### Scaling to Multiple Servers

For production deployments across multiple servers, you need a SignalR backplane:

**Option 1: Azure SignalR Service**
```csharp
builder.Services.AddSignalR()
    .AddAzureSignalR("connection-string");
```

**Option 2: Redis Backplane**
```csharp
builder.Services.AddSignalR()
    .AddStackExchangeRedis("redis-connection-string");
```

**Option 3: SQL Server Backplane**
```csharp
builder.Services.AddSignalR()
    .AddSqlServer("sql-connection-string");
```

### Security

**Authentication**:
```csharp
public class TurboStreamsHub : Hub
{
    [Authorize]
    public async Task SubscribeToChannel(string channel)
    {
        // Only authenticated users can subscribe
        await Groups.AddToGroupAsync(Context.ConnectionId, channel);
    }
}
```

**Authorization**:
```csharp
public async Task SubscribeToChannel(string channel)
{
    // Check if user has permission to subscribe to this channel
    if (!await _authService.CanAccessChannel(Context.User, channel))
    {
        throw new UnauthorizedAccessException();
    }
    
    await Groups.AddToGroupAsync(Context.ConnectionId, channel);
}
```

## Performance Tips

1. **Use channels wisely** - Don't broadcast to all clients if you can target specific channels
2. **Keep messages small** - Only send necessary HTML in Turbo Streams
3. **Use caching** - Cache rendered views when possible
4. **Monitor connections** - Track active connections and clean up stale ones
5. **Configure connection limits** - Set appropriate limits for your server capacity

## Troubleshooting

### Connection Issues
- Check browser console for errors
- Verify SignalR Hub is mapped in Program.cs
- Ensure firewall allows WebSocket connections

### Updates Not Appearing
- Verify channel name matches between subscribe and broadcast
- Check that Turbo is loaded before turbo-signalr.js
- Confirm the target element ID exists in the DOM

### Reconnection Problems
- Check network stability
- Increase reconnection delay if needed
- Verify server is still running

## Learn More

- [Hotwire Documentation](https://hotwired.dev/)
- [Turbo Streams Reference](https://turbo.hotwired.dev/handbook/streams)
- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Implementation Plan](../../docs/turbo-streams-signalr-plan.md)

## License

See the repository root for license information.
