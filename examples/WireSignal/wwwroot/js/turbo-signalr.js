/**
 * Turbo Streams + SignalR Integration
 * 
 * Connects to the SignalR TurboStreamsHub and receives real-time Turbo Stream updates.
 * Provides automatic reconnection and channel subscription management.
 * 
 * @version 1.0.0
 * @requires @microsoft/signalr
 * @requires @hotwired/turbo
 */

(() => {
    if (typeof window !== 'undefined' && window.TurboSignalR) {
        return;
    }

class TurboSignalRConnection {
    /**
     * Creates a new Turbo SignalR connection
     * @param {string} hubUrl - The URL of the SignalR hub (default: '/hubs/turbo-streams')
     * @param {Object} options - Additional configuration options
     * @param {number} options.reconnectDelay - Delay in milliseconds before reconnecting (default: 5000)
     * @param {boolean} options.autoStart - Whether to automatically start the connection (default: false)
     */
    constructor(hubUrl = '/hubs/turbo-streams', options = {}) {
        this.hubUrl = hubUrl;
        this.connection = null;
        this.channels = new Set();
        this.reconnectDelay = options.reconnectDelay || 5000;
        this.isStarted = false;

        if (options.autoStart) {
            this.start();
        }
    }

    /**
     * Starts the SignalR connection
     * @returns {Promise<void>}
     */
    async start() {
        if (this.isStarted) {
            console.warn('Turbo SignalR: Connection already started');
            return;
        }

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: (retryContext) => {
                    // Exponential backoff: 0s, 2s, 10s, 30s, then 60s
                    if (retryContext.previousRetryCount === 0) return 0;
                    if (retryContext.previousRetryCount === 1) return 2000;
                    if (retryContext.previousRetryCount === 2) return 10000;
                    if (retryContext.previousRetryCount === 3) return 30000;
                    return 60000;
                }
            })
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Set up event handlers
        this.connection.on('ReceiveTurboStream', (html) => {
            this.handleTurboStream(html);
        });

        this.connection.onreconnecting((error) => {
            console.log('Turbo SignalR: Reconnecting...', error);
            this.dispatchEvent('reconnecting', { error });
        });

        this.connection.onreconnected((connectionId) => {
            console.log('Turbo SignalR: Reconnected', connectionId);
            this.dispatchEvent('reconnected', { connectionId });
            this.resubscribeChannels();
        });

        this.connection.onclose((error) => {
            console.error('Turbo SignalR: Connection closed', error);
            this.isStarted = false;
            this.dispatchEvent('closed', { error });
            
            // Attempt to reconnect
            setTimeout(() => {
                if (!this.isStarted) {
                    this.start();
                }
            }, this.reconnectDelay);
        });

        try {
            await this.connection.start();
            this.isStarted = true;
            console.log('Turbo SignalR: Connected');
            this.dispatchEvent('connected');
        } catch (err) {
            console.error('Turbo SignalR: Error starting connection:', err);
            this.isStarted = false;
            this.dispatchEvent('error', { error: err });
            
            // Retry after delay
            setTimeout(() => this.start(), this.reconnectDelay);
        }
    }

    /**
     * Subscribes to a channel
     * @param {string} channel - Channel name to subscribe to
     * @returns {Promise<void>}
     */
    async subscribe(channel) {
        if (!channel) {
            throw new Error('Channel name is required');
        }

        if (this.channels.has(channel)) {
            console.warn(`Turbo SignalR: Already subscribed to channel: ${channel}`);
            return;
        }

        if (!this.isStarted) {
            throw new Error('Connection is not started. Call start() first.');
        }

        try {
            await this.connection.invoke('SubscribeToChannel', channel);
            this.channels.add(channel);
            console.log(`Turbo SignalR: Subscribed to channel: ${channel}`);
            this.dispatchEvent('subscribed', { channel });
        } catch (err) {
            console.error(`Turbo SignalR: Error subscribing to channel ${channel}:`, err);
            throw err;
        }
    }

    /**
     * Unsubscribes from a channel
     * @param {string} channel - Channel name to unsubscribe from
     * @returns {Promise<void>}
     */
    async unsubscribe(channel) {
        if (!channel) {
            throw new Error('Channel name is required');
        }

        if (!this.channels.has(channel)) {
            console.warn(`Turbo SignalR: Not subscribed to channel: ${channel}`);
            return;
        }

        if (!this.isStarted) {
            throw new Error('Connection is not started.');
        }

        try {
            await this.connection.invoke('UnsubscribeFromChannel', channel);
            this.channels.delete(channel);
            console.log(`Turbo SignalR: Unsubscribed from channel: ${channel}`);
            this.dispatchEvent('unsubscribed', { channel });
        } catch (err) {
            console.error(`Turbo SignalR: Error unsubscribing from channel ${channel}:`, err);
            throw err;
        }
    }

    /**
     * Resubscribes to all channels after reconnection
     * @private
     */
    async resubscribeChannels() {
        const channelsToResubscribe = Array.from(this.channels);
        this.channels.clear();
        
        for (const channel of channelsToResubscribe) {
            try {
                await this.subscribe(channel);
            } catch (err) {
                console.error(`Turbo SignalR: Failed to resubscribe to channel ${channel}:`, err);
            }
        }
    }

    /**
     * Handles incoming Turbo Stream HTML
     * @private
     * @param {string} html - Turbo Stream HTML content
     */
    handleTurboStream(html) {
        if (!html) {
            console.warn('Turbo SignalR: Received empty HTML');
            return;
        }

        if (window.Turbo && window.Turbo.renderStreamMessage) {
            try {
                Turbo.renderStreamMessage(html);
                this.dispatchEvent('streamReceived', { html });
            } catch (err) {
                console.error('Turbo SignalR: Error rendering stream:', err);
                this.dispatchEvent('error', { error: err, html });
            }
        } else {
            console.error('Turbo SignalR: Turbo.renderStreamMessage is not available. Make sure @hotwired/turbo is loaded.');
        }
    }

    /**
     * Dispatches a custom event
     * @private
     * @param {string} eventName - Event name
     * @param {Object} detail - Event detail
     */
    dispatchEvent(eventName, detail = {}) {
        const event = new CustomEvent(`turbo-signalr:${eventName}`, {
            detail: { ...detail, connection: this },
            bubbles: true
        });
        document.dispatchEvent(event);
    }

    /**
     * Stops the connection
     * @returns {Promise<void>}
     */
    async stop() {
        if (!this.connection) {
            console.warn('Turbo SignalR: No connection to stop');
            return;
        }

        try {
            this.isStarted = false;
            await this.connection.stop();
            console.log('Turbo SignalR: Disconnected');
            this.dispatchEvent('disconnected');
        } catch (err) {
            console.error('Turbo SignalR: Error stopping connection:', err);
            throw err;
        }
    }

    /**
     * Gets the current connection state
     * @returns {string} Connection state
     */
    getState() {
        if (!this.connection) {
            return 'Disconnected';
        }
        return signalR.HubConnectionState[this.connection.state];
    }

    /**
     * Checks if the connection is active
     * @returns {boolean}
     */
    isConnected() {
        return this.connection && this.connection.state === signalR.HubConnectionState.Connected;
    }
}

// Export for use in applications
if (typeof window !== 'undefined') {
    window.TurboSignalR = TurboSignalRConnection;
}

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = TurboSignalRConnection;
}

})();
