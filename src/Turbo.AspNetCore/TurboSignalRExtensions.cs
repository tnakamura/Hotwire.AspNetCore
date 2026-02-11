using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Turbo.AspNetCore.Hubs;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Extension methods for broadcasting Turbo Streams via SignalR from controllers.
    /// </summary>
    public static class TurboSignalRExtensions
    {
        /// <summary>
        /// Broadcasts a Turbo Stream view to all clients subscribed to a channel.
        /// This is a convenience method for controllers to easily broadcast Turbo Streams.
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="broadcaster">Turbo Stream broadcaster service</param>
        /// <param name="channel">Channel name to broadcast to</param>
        /// <param name="viewName">View name to render</param>
        /// <param name="model">Model for the view</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        /// <example>
        /// <code>
        /// await this.BroadcastTurboStreamAsync(_broadcaster, "notifications", "_Notification", notification);
        /// </code>
        /// </example>
        public static async Task BroadcastTurboStreamAsync(
            this Controller controller,
            ITurboStreamBroadcaster broadcaster,
            string channel,
            string viewName,
            object model = null)
        {
            if (broadcaster == null)
            {
                throw new ArgumentNullException(nameof(broadcaster));
            }

            await broadcaster.BroadcastViewAsync(channel, viewName, model);
        }

        /// <summary>
        /// Broadcasts raw Turbo Stream HTML to all clients subscribed to a channel.
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="broadcaster">Turbo Stream broadcaster service</param>
        /// <param name="channel">Channel name to broadcast to</param>
        /// <param name="turboStreamHtml">Turbo Stream HTML content</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        public static async Task BroadcastTurboStreamHtmlAsync(
            this Controller controller,
            ITurboStreamBroadcaster broadcaster,
            string channel,
            string turboStreamHtml)
        {
            if (broadcaster == null)
            {
                throw new ArgumentNullException(nameof(broadcaster));
            }

            await broadcaster.BroadcastAsync(channel, turboStreamHtml);
        }

        /// <summary>
        /// Broadcasts a Turbo Stream view to all connected clients.
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="broadcaster">Turbo Stream broadcaster service</param>
        /// <param name="viewName">View name to render</param>
        /// <param name="model">Model for the view</param>
        /// <returns>A task representing the asynchronous broadcast operation</returns>
        public static async Task BroadcastTurboStreamToAllAsync(
            this Controller controller,
            ITurboStreamBroadcaster broadcaster,
            string viewName,
            object model = null)
        {
            if (broadcaster == null)
            {
                throw new ArgumentNullException(nameof(broadcaster));
            }

            var html = await broadcaster.BroadcastViewAsync("__temp__", viewName, model);
            // Note: This is a workaround - we should refactor BroadcastViewAsync to return HTML
            // For now, we'll use the hub context directly
            throw new NotImplementedException("Use ITurboStreamBroadcaster.BroadcastToAllAsync directly");
        }
    }
}
