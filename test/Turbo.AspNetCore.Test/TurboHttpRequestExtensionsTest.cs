using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
namespace Turbo.AspNetCore.Test;

public class TurboHttpRequestExtensionsTest
{
    [Theory]
    [InlineData("turbo-frame", true)]
    [InlineData("", false)]
    public void IsTurboFrameRequestTest(string key, bool expected)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[key] = "";

        Assert.Equal(expected, context.Request.IsTurboFrameRequest());
    }

    [Theory]
    [InlineData("text/vnd.turbo-stream.html", true)]
    [InlineData("text/html", false)]
    public void IsTurboStreamRequestTest(string mediaType, bool expected)
    {
        var header = new MediaTypeHeaderValue(mediaType);
        var context = new DefaultHttpContext();
        context.Request.GetTypedHeaders().Accept = [header];

        Assert.Equal(expected, context.Request.IsTurboStreamRequest());
    }

    [Fact]
    public void IsTurboDriveRequest_WithoutTurboFrameHeader_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var header = new MediaTypeHeaderValue("text/html");
        context.Request.GetTypedHeaders().Accept = [header];

        // Act
        var result = context.Request.IsTurboDriveRequest();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTurboDriveRequest_WithTurboFrameHeader_ReturnsFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var header = new MediaTypeHeaderValue("text/html");
        context.Request.GetTypedHeaders().Accept = [header];
        context.Request.Headers["turbo-frame"] = "gallery";

        // Act
        var result = context.Request.IsTurboDriveRequest();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTurboDriveRequest_WithoutHtmlAccept_ReturnsFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var header = new MediaTypeHeaderValue("application/json");
        context.Request.GetTypedHeaders().Accept = [header];

        // Act
        var result = context.Request.IsTurboDriveRequest();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTurboRequest_WithTurboDriveRequest_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var header = new MediaTypeHeaderValue("text/html");
        context.Request.GetTypedHeaders().Accept = [header];

        // Act
        var result = context.Request.IsTurboRequest();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTurboRequest_WithTurboFrameRequest_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["turbo-frame"] = "gallery";

        // Act
        var result = context.Request.IsTurboRequest();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTurboRequest_WithTurboStreamRequest_ReturnsTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var header = new MediaTypeHeaderValue("text/vnd.turbo-stream.html");
        context.Request.GetTypedHeaders().Accept = [header];

        // Act
        var result = context.Request.IsTurboRequest();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTurboRequest_WithNonTurboRequest_ReturnsFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var header = new MediaTypeHeaderValue("application/json");
        context.Request.GetTypedHeaders().Accept = [header];

        // Act
        var result = context.Request.IsTurboRequest();

        // Assert
        Assert.False(result);
    }
}
