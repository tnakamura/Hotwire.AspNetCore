using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Turbo.AspNetCore.Test;

public class TurboSignalRExtensionsTest
{
    [Fact]
    public async Task BroadcastTurboStreamAsync_CallsBroadcastViewAsync()
    {
        var controller = new TestController();
        var broadcaster = Substitute.For<ITurboStreamBroadcaster>();
        var model = new { Id = 1 };

        await controller.BroadcastTurboStreamAsync(broadcaster, "news", "_Message", model);

        await broadcaster.Received(1).BroadcastViewAsync("news", "_Message", model);
    }

    [Fact]
    public async Task BroadcastTurboStreamAsync_WithNullBroadcaster_ThrowsArgumentNullException()
    {
        var controller = new TestController();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            controller.BroadcastTurboStreamAsync(null!, "news", "_Message", new { Id = 1 }));
    }

    [Fact]
    public async Task BroadcastTurboStreamHtmlAsync_CallsBroadcastAsync()
    {
        var controller = new TestController();
        var broadcaster = Substitute.For<ITurboStreamBroadcaster>();

        await controller.BroadcastTurboStreamHtmlAsync(broadcaster, "news", "<turbo-stream></turbo-stream>");

        await broadcaster.Received(1).BroadcastAsync("news", "<turbo-stream></turbo-stream>");
    }

    [Fact]
    public async Task BroadcastTurboStreamHtmlAsync_WithNullBroadcaster_ThrowsArgumentNullException()
    {
        var controller = new TestController();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            controller.BroadcastTurboStreamHtmlAsync(null!, "news", "<turbo-stream></turbo-stream>"));
    }

    [Fact]
    public async Task BroadcastTurboStreamToAllAsync_CallsBroadcastToAllAsync()
    {
        var controller = new TestController();
        var broadcaster = Substitute.For<ITurboStreamBroadcaster>();

        await controller.BroadcastTurboStreamToAllAsync(broadcaster, "<turbo-stream></turbo-stream>");

        await broadcaster.Received(1).BroadcastToAllAsync("<turbo-stream></turbo-stream>");
    }

    [Fact]
    public async Task BroadcastTurboStreamToAllAsync_WithNullBroadcaster_ThrowsArgumentNullException()
    {
        var controller = new TestController();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            controller.BroadcastTurboStreamToAllAsync(null!, "<turbo-stream></turbo-stream>"));
    }

    private sealed class TestController : Controller;
}
