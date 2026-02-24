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

        /// <summary>
        /// Determines whether the request is a Turbo Drive request.
        /// </summary>
        public static bool IsTurboDriveRequest(this HttpRequest request)
        {
            // A Turbo Drive request is a normal request without the "Turbo-Frame" header
            // and with text/html included in the Accept header.
            return !request.Headers.ContainsKey("turbo-frame") &&
                   request.GetTypedHeaders().Accept.Any(x => x.MediaType == "text/html");
        }

        /// <summary>
        /// Determines whether the request is a Turbo request (Drive/Frame/Stream).
        /// </summary>
        public static bool IsTurboRequest(this HttpRequest request)
        {
            return request.IsTurboDriveRequest() ||
                   request.IsTurboFrameRequest() ||
                   request.IsTurboStreamRequest();
        }
    }
}
