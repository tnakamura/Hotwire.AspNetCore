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
        context.Request.Headers.Add(key, "");

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
}
