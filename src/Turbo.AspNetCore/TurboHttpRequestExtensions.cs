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
        /// Turbo Drive によるリクエストかどうかを判定
        /// </summary>
        public static bool IsTurboDriveRequest(this HttpRequest request)
        {
            // Turbo Drive は "Turbo-Frame" ヘッダーが存在しない通常のリクエスト
            // かつ Accept ヘッダーに text/html が含まれる
            return !request.Headers.ContainsKey("turbo-frame") &&
                   request.GetTypedHeaders().Accept.Any(x => x.MediaType == "text/html");
        }

        /// <summary>
        /// Turbo によるリクエストかどうかを判定（Drive/Frame/Stream のいずれか）
        /// </summary>
        public static bool IsTurboRequest(this HttpRequest request)
        {
            return request.IsTurboDriveRequest() ||
                   request.IsTurboFrameRequest() ||
                   request.IsTurboStreamRequest();
        }
    }
}
