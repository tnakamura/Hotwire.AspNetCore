using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using System;
using System.IO;
using System.Threading.Tasks;
using Turbo.AspNetCore.Hubs;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Service for broadcasting Turbo Stream updates via SignalR.
    /// </summary>
    public class TurboStreamBroadcaster : ITurboStreamBroadcaster
    {
        private readonly IHubContext<TurboStreamsHub> _hubContext;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TurboStreamBroadcaster"/> class.
        /// </summary>
        /// <param name="hubContext">SignalR hub context for TurboStreamsHub</param>
        /// <param name="razorViewEngine">Razor view engine for rendering views</param>
        /// <param name="tempDataProvider">Temp data provider</param>
        /// <param name="serviceProvider">Service provider</param>
        public TurboStreamBroadcaster(
            IHubContext<TurboStreamsHub> hubContext,
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _razorViewEngine = razorViewEngine ?? throw new ArgumentNullException(nameof(razorViewEngine));
            _tempDataProvider = tempDataProvider ?? throw new ArgumentNullException(nameof(tempDataProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to all clients subscribed to a channel.
        /// </summary>
        public async Task BroadcastAsync(string channel, string turboStreamHtml)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel name cannot be null or empty", nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(turboStreamHtml))
            {
                throw new ArgumentException("Turbo Stream HTML cannot be null or empty", nameof(turboStreamHtml));
            }

            await _hubContext.Clients.Group(channel).SendAsync("ReceiveTurboStream", turboStreamHtml);
        }

        /// <summary>
        /// Broadcasts a Turbo Stream by rendering a partial view to all clients in a channel.
        /// </summary>
        public async Task BroadcastViewAsync(string channel, string viewName, object model = null)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel name cannot be null or empty", nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("View name cannot be null or empty", nameof(viewName));
            }

            var html = await RenderViewToStringAsync(viewName, model);
            await BroadcastAsync(channel, html);
        }

        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to a specific connection.
        /// </summary>
        public async Task BroadcastToConnectionAsync(string connectionId, string turboStreamHtml)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            if (string.IsNullOrWhiteSpace(turboStreamHtml))
            {
                throw new ArgumentException("Turbo Stream HTML cannot be null or empty", nameof(turboStreamHtml));
            }

            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveTurboStream", turboStreamHtml);
        }

        /// <summary>
        /// Broadcasts a Turbo Stream HTML fragment to all connected clients.
        /// </summary>
        public async Task BroadcastToAllAsync(string turboStreamHtml)
        {
            if (string.IsNullOrWhiteSpace(turboStreamHtml))
            {
                throw new ArgumentException("Turbo Stream HTML cannot be null or empty", nameof(turboStreamHtml));
            }

            await _hubContext.Clients.All.SendAsync("ReceiveTurboStream", turboStreamHtml);
        }

        /// <summary>
        /// Renders a Razor view to a string.
        /// </summary>
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);

            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Could not find view '{viewName}'. Searched locations: {string.Join(", ", viewResult.SearchedLocations)}");
            }

            await using var sw = new StringWriter();
            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}
