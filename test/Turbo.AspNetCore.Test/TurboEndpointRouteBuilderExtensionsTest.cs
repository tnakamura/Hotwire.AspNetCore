using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Turbo.AspNetCore.Hubs;

namespace Turbo.AspNetCore.Test;

public class TurboEndpointRouteBuilderExtensionsTest
{
    [Fact]
    public void MapTurboStreamsHub_WithoutPattern_MapsDefaultHubEndpoint()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSignalR();
        var app = builder.Build();

        app.MapTurboStreamsHub();

        var endpoint = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .FirstOrDefault(e => e.RoutePattern.RawText == "/hubs/turbo-streams");

        Assert.NotNull(endpoint);
        Assert.Contains(endpoint!.Metadata, metadata =>
            metadata is HubMetadata hubMetadata &&
            hubMetadata.HubType == typeof(TurboStreamsHub));
    }

    [Fact]
    public void MapTurboStreamsHub_WithPattern_MapsHubEndpointToSpecifiedPath()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSignalR();
        var app = builder.Build();

        app.MapTurboStreamsHub("/custom/turbo");

        var endpoint = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .FirstOrDefault(e => e.RoutePattern.RawText == "/custom/turbo");

        Assert.NotNull(endpoint);
        Assert.Contains(endpoint!.Metadata, metadata =>
            metadata is HubMetadata hubMetadata &&
            hubMetadata.HubType == typeof(TurboStreamsHub));
    }

    [Fact]
    public void MapTurboStreamsHub_NullEndpoints_ThrowsArgumentNullException()
    {
        IEndpointRouteBuilder? endpoints = null;

        var action = () => endpoints!.MapTurboStreamsHub();

        Assert.Throws<ArgumentNullException>(action);
    }
}
