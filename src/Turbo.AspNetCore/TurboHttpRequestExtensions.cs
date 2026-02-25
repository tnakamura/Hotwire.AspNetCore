using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Provides extension methods for detecting Turbo-related HTTP requests.
    /// </summary>
    public static class TurboHttpRequestExtensions
    {
        /// <summary>
        /// Determines whether the request is a Turbo Frame request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns><see langword="true"/> if the request contains the <c>turbo-frame</c> header; otherwise, <see langword="false"/>.</returns>
        public static bool IsTurboFrameRequest(this HttpRequest request)
        {
            return request.Headers.Any(x => x.Key == "turbo-frame");
        }

        /// <summary>
        /// Determines whether the request is a Turbo Stream request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns><see langword="true"/> if the request accepts <c>text/vnd.turbo-stream.html</c>; otherwise, <see langword="false"/>.</returns>
        public static bool IsTurboStreamRequest(this HttpRequest request)
        {
            return request.GetTypedHeaders().Accept.Any(x => x.MediaType == "text/vnd.turbo-stream.html");
        }

        /// <summary>
        /// Determines whether the request is a Turbo Drive request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns><see langword="true"/> if the request is a Turbo Drive request; otherwise, <see langword="false"/>.</returns>
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
        /// <param name="request">The HTTP request.</param>
        /// <returns><see langword="true"/> if the request is any Turbo request type; otherwise, <see langword="false"/>.</returns>
        public static bool IsTurboRequest(this HttpRequest request)
        {
            return request.IsTurboDriveRequest() ||
                   request.IsTurboFrameRequest() ||
                   request.IsTurboStreamRequest();
        }
    }
}
