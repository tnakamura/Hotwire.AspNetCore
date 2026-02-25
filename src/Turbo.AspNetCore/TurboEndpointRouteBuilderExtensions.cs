using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Turbo.AspNetCore.Hubs;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Extension methods for mapping Turbo.AspNetCore endpoints.
    /// </summary>
    public static class TurboEndpointRouteBuilderExtensions
    {
        private const string DefaultTurboStreamsHubPattern = "/hubs/turbo-streams";

        /// <summary>
        /// Maps <see cref="TurboStreamsHub"/> to the default path <c>/hubs/turbo-streams</c>.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>A convention builder for the mapped hub endpoint.</returns>
        public static HubEndpointConventionBuilder MapTurboStreamsHub(this IEndpointRouteBuilder endpoints)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            return endpoints.MapTurboStreamsHub(DefaultTurboStreamsHubPattern);
        }

        /// <summary>
        /// Maps <see cref="TurboStreamsHub"/> to the specified path.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <param name="pattern">The path pattern for the Turbo Streams hub endpoint.</param>
        /// <returns>A convention builder for the mapped hub endpoint.</returns>
        public static HubEndpointConventionBuilder MapTurboStreamsHub(this IEndpointRouteBuilder endpoints, string pattern)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            return endpoints.MapHub<TurboStreamsHub>(pattern);
        }
    }
}
