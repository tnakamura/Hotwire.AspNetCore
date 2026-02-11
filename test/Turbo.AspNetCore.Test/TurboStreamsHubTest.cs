using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Turbo.AspNetCore.Hubs;

namespace Turbo.AspNetCore.Test;

public class TurboStreamsHubTest
{
    [Fact]
    public async Task SubscribeToChannel_AddsConnectionToGroup()
    {
        // Arrange
        var mockClients = Substitute.For<IHubCallerClients>();
        var mockGroups = Substitute.For<IGroupManager>();
        var mockContext = Substitute.For<HubCallerContext>();
        
        mockContext.ConnectionId.Returns("test-connection-id");
        
        var hub = new TurboStreamsHub
        {
            Clients = mockClients,
            Groups = mockGroups,
            Context = mockContext
        };

        // Act
        await hub.SubscribeToChannel("test-channel");

        // Assert
        await mockGroups.Received(1).AddToGroupAsync("test-connection-id", "test-channel", default);
    }

    [Fact]
    public async Task UnsubscribeFromChannel_RemovesConnectionFromGroup()
    {
        // Arrange
        var mockClients = Substitute.For<IHubCallerClients>();
        var mockGroups = Substitute.For<IGroupManager>();
        var mockContext = Substitute.For<HubCallerContext>();
        
        mockContext.ConnectionId.Returns("test-connection-id");
        
        var hub = new TurboStreamsHub
        {
            Clients = mockClients,
            Groups = mockGroups,
            Context = mockContext
        };

        // Act
        await hub.UnsubscribeFromChannel("test-channel");

        // Assert
        await mockGroups.Received(1).RemoveFromGroupAsync("test-connection-id", "test-channel", default);
    }

    [Fact]
    public async Task SubscribeToChannel_WithNullChannel_ThrowsArgumentException()
    {
        // Arrange
        var mockClients = Substitute.For<IHubCallerClients>();
        var mockGroups = Substitute.For<IGroupManager>();
        var mockContext = Substitute.For<HubCallerContext>();
        
        var hub = new TurboStreamsHub
        {
            Clients = mockClients,
            Groups = mockGroups,
            Context = mockContext
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => hub.SubscribeToChannel(null!));
    }

    [Fact]
    public async Task SubscribeToChannel_WithEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var mockClients = Substitute.For<IHubCallerClients>();
        var mockGroups = Substitute.For<IGroupManager>();
        var mockContext = Substitute.For<HubCallerContext>();
        
        var hub = new TurboStreamsHub
        {
            Clients = mockClients,
            Groups = mockGroups,
            Context = mockContext
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => hub.SubscribeToChannel(""));
    }

    [Fact]
    public async Task UnsubscribeFromChannel_WithNullChannel_ThrowsArgumentException()
    {
        // Arrange
        var mockClients = Substitute.For<IHubCallerClients>();
        var mockGroups = Substitute.For<IGroupManager>();
        var mockContext = Substitute.For<HubCallerContext>();
        
        var hub = new TurboStreamsHub
        {
            Clients = mockClients,
            Groups = mockGroups,
            Context = mockContext
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => hub.UnsubscribeFromChannel(null!));
    }
}
