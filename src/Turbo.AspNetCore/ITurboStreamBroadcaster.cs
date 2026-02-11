using System.Threading.Tasks;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Service for broadcasting Turbo Stream updates via SignalR.
    /// Provides a high-level API for sending real-time updates to clients.
    /// </summary>
    public interface ITurboStreamBroadcaster
    {
        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to all clients subscribed to a channel.
        /// </summary>
        /// <param name="channel">Channel name to broadcast to (e.g., "notifications", "chat:123")</param>
        /// <param name="turboStreamHtml">Turbo Stream HTML content (e.g., &lt;turbo-stream action="append"...&gt;)</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        Task BroadcastAsync(string channel, string turboStreamHtml);

        /// <summary>
        /// Broadcasts a Turbo Stream by rendering a partial view to all clients in a channel.
        /// The view should contain turbo-stream elements.
        /// </summary>
        /// <param name="channel">Channel name to broadcast to</param>
        /// <param name="viewName">Partial view name to render (e.g., "_Notification")</param>
        /// <param name="model">Model to pass to the view</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        Task BroadcastViewAsync(string channel, string viewName, object model = null);

        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to a specific connection.
        /// </summary>
        /// <param name="connectionId">SignalR connection ID</param>
        /// <param name="turboStreamHtml">Turbo Stream HTML content</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        Task BroadcastToConnectionAsync(string connectionId, string turboStreamHtml);

        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to all connected clients.
        /// </summary>
        /// <param name="turboStreamHtml">Turbo Stream HTML content</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        Task BroadcastToAllAsync(string turboStreamHtml);
    }
}
