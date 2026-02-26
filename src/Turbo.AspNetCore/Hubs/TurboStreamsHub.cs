using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Turbo.AspNetCore.Hubs
{
    /// <summary>
    /// SignalR Hub for broadcasting Turbo Stream updates to connected clients.
    /// Provides channel-based subscription model similar to Rails ActionCable.
    /// </summary>
    public class TurboStreamsHub : Hub
    {
        /// <summary>
        /// Subscribes the current connection to a specific channel.
        /// Clients in the same channel will receive broadcast messages sent to that channel.
        /// </summary>
        /// <param name="channel">Channel name to subscribe to (e.g., "notifications", "chat:123")</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task SubscribeToChannel(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel name cannot be null or empty", nameof(channel));
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, channel);
        }

        /// <summary>
        /// Unsubscribes the current connection from a specific channel.
        /// </summary>
        /// <param name="channel">Channel name to unsubscribe from</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task UnsubscribeFromChannel(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel name cannot be null or empty", nameof(channel));
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);
        }

        /// <summary>
        /// Called when a client connects to the hub.
        /// Override to add custom connection logic.
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// Override to add custom disconnection logic.
        /// </summary>
        /// <param name="exception">Exception that caused the disconnection, if any</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
