using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Turbo.AspNetCore.Test;

public class TurboServiceCollectionExtensionsTest
{
    [Fact]
    public void AddTurboStreamBroadcaster_RegistersRequiredServices()
    {
        var services = new ServiceCollection();

        services.AddTurboStreamBroadcaster();

        var hasHttpContextAccessor = services.Any(x =>
            x.ServiceType == typeof(IHttpContextAccessor));

        var broadcasterDescriptor = services.SingleOrDefault(x =>
            x.ServiceType == typeof(ITurboStreamBroadcaster));

        Assert.True(hasHttpContextAccessor);
        Assert.NotNull(broadcasterDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, broadcasterDescriptor!.Lifetime);
        Assert.Equal(typeof(TurboStreamBroadcaster), broadcasterDescriptor.ImplementationType);
    }

    [Fact]
    public void AddTurboStreamBroadcaster_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection? services = null;

        var action = () => services!.AddTurboStreamBroadcaster();

        Assert.Throws<ArgumentNullException>(action);
    }
}
