using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Extension methods for registering Turbo.AspNetCore services.
    /// </summary>
    public static class TurboServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for SignalR-based Turbo Stream broadcasting.
        /// Registers <see cref="IHttpContextAccessor"/> and <see cref="ITurboStreamBroadcaster"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection for chaining.</returns>
        public static IServiceCollection AddTurboStreamBroadcaster(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddHttpContextAccessor();
            services.TryAddScoped<ITurboStreamBroadcaster, TurboStreamBroadcaster>();

            return services;
        }
    }
}