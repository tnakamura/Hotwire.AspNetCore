using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Turbo.AspNetCore.Hubs;

namespace Turbo.AspNetCore.Test;

public class TurboStreamBroadcasterTest
{
    [Fact]
    public async Task BroadcastAsync_WithValidArguments_SendsToGroup()
    {
        var fixture = CreateFixture();

        await fixture.Broadcaster.BroadcastAsync("notifications", "<turbo-stream>ok</turbo-stream>");

        await fixture.GroupProxy.Received(1).SendCoreAsync(
            "ReceiveTurboStream",
            Arg.Is<object?[]>(x => x.Length == 1 && x[0] as string == "<turbo-stream>ok</turbo-stream>"),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BroadcastAsync_WithInvalidChannel_ThrowsArgumentException(string? channel)
    {
        var fixture = CreateFixture();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Broadcaster.BroadcastAsync(channel!, "<turbo-stream>ok</turbo-stream>"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BroadcastAsync_WithInvalidHtml_ThrowsArgumentException(string? html)
    {
        var fixture = CreateFixture();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Broadcaster.BroadcastAsync("notifications", html!));
    }

    [Fact]
    public async Task BroadcastToConnectionAsync_WithValidArguments_SendsToConnection()
    {
        var fixture = CreateFixture();

        await fixture.Broadcaster.BroadcastToConnectionAsync("connection-1", "<turbo-stream>ok</turbo-stream>");

        await fixture.ConnectionProxy.Received(1).SendCoreAsync(
            "ReceiveTurboStream",
            Arg.Is<object?[]>(x => x.Length == 1 && x[0] as string == "<turbo-stream>ok</turbo-stream>"),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BroadcastToConnectionAsync_WithInvalidConnectionId_ThrowsArgumentException(string? connectionId)
    {
        var fixture = CreateFixture();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Broadcaster.BroadcastToConnectionAsync(connectionId!, "<turbo-stream>ok</turbo-stream>"));
    }

    [Fact]
    public async Task BroadcastToAllAsync_WithValidArguments_SendsToAllClients()
    {
        var fixture = CreateFixture();

        await fixture.Broadcaster.BroadcastToAllAsync("<turbo-stream>ok</turbo-stream>");

        await fixture.AllProxy.Received(1).SendCoreAsync(
            "ReceiveTurboStream",
            Arg.Is<object?[]>(x => x.Length == 1 && x[0] as string == "<turbo-stream>ok</turbo-stream>"),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BroadcastToAllAsync_WithInvalidHtml_ThrowsArgumentException(string? html)
    {
        var fixture = CreateFixture();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Broadcaster.BroadcastToAllAsync(html!));
    }

    [Fact]
    public async Task BroadcastViewAsync_WhenViewFound_RendersAndBroadcasts()
    {
        var fixture = CreateFixture();
        fixture.ViewEngine
            .FindView(Arg.Any<Microsoft.AspNetCore.Mvc.ActionContext>(), "_Message", false)
            .Returns(ViewEngineResult.Found("_Message", fixture.View));
        fixture.View
            .RenderAsync(Arg.Any<ViewContext>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ViewContext>().Writer.Write("<turbo-stream>rendered</turbo-stream>");
                return Task.CompletedTask;
            });

        await fixture.Broadcaster.BroadcastViewAsync("notifications", "_Message", new { Id = 1 });

        await fixture.GroupProxy.Received(1).SendCoreAsync(
            "ReceiveTurboStream",
            Arg.Is<object?[]>(x => x.Length == 1 && x[0] as string == "<turbo-stream>rendered</turbo-stream>"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BroadcastViewAsync_WhenHttpContextExists_UsesItAndBroadcasts()
    {
        var httpContext = new DefaultHttpContext();
        var fixture = CreateFixture(httpContext);
        fixture.ViewEngine
            .FindView(Arg.Any<Microsoft.AspNetCore.Mvc.ActionContext>(), "_Message", false)
            .Returns(ViewEngineResult.Found("_Message", fixture.View));
        fixture.View
            .RenderAsync(Arg.Any<ViewContext>())
            .Returns(callInfo =>
            {
                callInfo.Arg<ViewContext>().Writer.Write("<turbo-stream>rendered-ctx</turbo-stream>");
                return Task.CompletedTask;
            });

        await fixture.Broadcaster.BroadcastViewAsync("notifications", "_Message", null);

        await fixture.GroupProxy.Received(1).SendCoreAsync(
            "ReceiveTurboStream",
            Arg.Is<object?[]>(x => x.Length == 1 && x[0] as string == "<turbo-stream>rendered-ctx</turbo-stream>"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BroadcastViewAsync_WhenViewNotFound_ThrowsInvalidOperationException()
    {
        var fixture = CreateFixture();
        fixture.ViewEngine
            .FindView(Arg.Any<Microsoft.AspNetCore.Mvc.ActionContext>(), "missing", false)
            .Returns(ViewEngineResult.NotFound("missing", ["/Views/Missing.cshtml"]));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Broadcaster.BroadcastViewAsync("notifications", "missing", null));

        Assert.Contains("Could not find view 'missing'.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BroadcastViewAsync_WithInvalidChannel_ThrowsArgumentException(string? channel)
    {
        var fixture = CreateFixture();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Broadcaster.BroadcastViewAsync(channel!, "_Message", null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BroadcastViewAsync_WithInvalidViewName_ThrowsArgumentException(string? viewName)
    {
        var fixture = CreateFixture();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            fixture.Broadcaster.BroadcastViewAsync("notifications", viewName!, null));
    }

    [Fact]
    public void Constructor_WithNullHubContext_ThrowsArgumentNullException()
    {
        var viewEngine = Substitute.For<IRazorViewEngine>();
        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        Assert.Throws<ArgumentNullException>(() =>
            new TurboStreamBroadcaster(null!, viewEngine, tempDataProvider, serviceProvider, httpContextAccessor));
    }

    [Fact]
    public void Constructor_WithNullViewEngine_ThrowsArgumentNullException()
    {
        var hubContext = Substitute.For<IHubContext<TurboStreamsHub>>();
        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        Assert.Throws<ArgumentNullException>(() =>
            new TurboStreamBroadcaster(hubContext, null!, tempDataProvider, serviceProvider, httpContextAccessor));
    }

    [Fact]
    public void Constructor_WithNullTempDataProvider_ThrowsArgumentNullException()
    {
        var hubContext = Substitute.For<IHubContext<TurboStreamsHub>>();
        var viewEngine = Substitute.For<IRazorViewEngine>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        Assert.Throws<ArgumentNullException>(() =>
            new TurboStreamBroadcaster(hubContext, viewEngine, null!, serviceProvider, httpContextAccessor));
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        var hubContext = Substitute.For<IHubContext<TurboStreamsHub>>();
        var viewEngine = Substitute.For<IRazorViewEngine>();
        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        Assert.Throws<ArgumentNullException>(() =>
            new TurboStreamBroadcaster(hubContext, viewEngine, tempDataProvider, null!, httpContextAccessor));
    }

    [Fact]
    public void Constructor_WithNullHttpContextAccessor_ThrowsArgumentNullException()
    {
        var hubContext = Substitute.For<IHubContext<TurboStreamsHub>>();
        var viewEngine = Substitute.For<IRazorViewEngine>();
        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var serviceProvider = Substitute.For<IServiceProvider>();

        Assert.Throws<ArgumentNullException>(() =>
            new TurboStreamBroadcaster(hubContext, viewEngine, tempDataProvider, serviceProvider, null!));
    }

    private static (TurboStreamBroadcaster Broadcaster, IClientProxy GroupProxy, ISingleClientProxy ConnectionProxy, IClientProxy AllProxy, IRazorViewEngine ViewEngine, IView View) CreateFixture(HttpContext? httpContext = null)
    {
        var hubContext = Substitute.For<IHubContext<TurboStreamsHub>>();
        var clients = Substitute.For<IHubClients>();
        var groupProxy = Substitute.For<IClientProxy>();
        var connectionProxy = Substitute.For<ISingleClientProxy>();
        var allProxy = Substitute.For<IClientProxy>();
        var viewEngine = Substitute.For<IRazorViewEngine>();
        var tempDataProvider = Substitute.For<ITempDataProvider>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var view = Substitute.For<IView>();

        clients.Group(Arg.Any<string>()).Returns(groupProxy);
        clients.Client(Arg.Any<string>()).Returns(connectionProxy);
        clients.All.Returns(allProxy);
        hubContext.Clients.Returns(clients);

        if (httpContext != null)
        {
            httpContext.RequestServices = serviceProvider;
        }

        httpContextAccessor.HttpContext.Returns(httpContext);

        var broadcaster = new TurboStreamBroadcaster(
            hubContext,
            viewEngine,
            tempDataProvider,
            serviceProvider,
            httpContextAccessor);

        return (broadcaster, groupProxy, connectionProxy, allProxy, viewEngine, view);
    }
}
