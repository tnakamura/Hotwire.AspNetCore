using Microsoft.AspNetCore.Mvc;

namespace Turbo.AspNetCore
{
    public static class TurboControllerExtensions
    {
        private const string TurboStreamContentType = "text/vnd.turbo-stream.html; charset=utf-8";

        public static IActionResult TurboStream(this Controller controller)
        {
            var result = controller.View();
            result.ContentType = TurboStreamContentType;
            return result;
        }

        public static IActionResult TurboStream(this Controller controller, object model)
        {
            var result = controller.View(model);
            result.ContentType = TurboStreamContentType;
            return result;
        }

        public static IActionResult TurboStream(this Controller controller, string viewName)
        {
            var result = controller.View(viewName);
            result.ContentType = TurboStreamContentType;
            return result;
        }

        public static IActionResult TurboStream(this Controller controller, string viewName, object model)
        {
            var result = controller.View(viewName, model);
            result.ContentType = TurboStreamContentType;
            return result;
        }
    }
}
