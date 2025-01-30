using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Turbo.AspNetCore
{
    public static class TurboHttpRequestExtensions
    {
        public static bool IsTurboFrameRequest(this HttpRequest request)
        {
            return request.Headers.Any(x => x.Key == "turbo-frame");
        }

        public static bool IsTurboStreamRequest(this HttpRequest request)
        {
            return request.GetTypedHeaders().Accept.Any(x => x.MediaType == "text/vnd.turbo-stream.html");
        }
    }
}
