using Microsoft.AspNetCore.Mvc;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// Provides extension methods for returning Turbo Stream responses from MVC controllers.
    /// </summary>
    public static class TurboControllerExtensions
    {
        private const string TurboStreamContentType = "text/vnd.turbo-stream.html; charset=utf-8";

        /// <summary>
        /// Returns the default view result with the Turbo Stream content type.
        /// </summary>
        /// <param name="controller">The controller instance.</param>
        /// <returns>An <see cref="IActionResult"/> configured for Turbo Stream rendering.</returns>
        public static IActionResult TurboStream(this Controller controller)
        {
            var result = controller.View();
            result.ContentType = TurboStreamContentType;
            return result;
        }

        /// <summary>
        /// Returns the default view result with the specified model and Turbo Stream content type.
        /// </summary>
        /// <param name="controller">The controller instance.</param>
        /// <param name="model">The model object passed to the view.</param>
        /// <returns>An <see cref="IActionResult"/> configured for Turbo Stream rendering.</returns>
        public static IActionResult TurboStream(this Controller controller, object model)
        {
            var result = controller.View(model);
            result.ContentType = TurboStreamContentType;
            return result;
        }

        /// <summary>
        /// Returns the specified view result with the Turbo Stream content type.
        /// </summary>
        /// <param name="controller">The controller instance.</param>
        /// <param name="viewName">The name of the view to render.</param>
        /// <returns>An <see cref="IActionResult"/> configured for Turbo Stream rendering.</returns>
        public static IActionResult TurboStream(this Controller controller, string viewName)
        {
            var result = controller.View(viewName);
            result.ContentType = TurboStreamContentType;
            return result;
        }

        /// <summary>
        /// Returns the specified view result with the specified model and Turbo Stream content type.
        /// </summary>
        /// <param name="controller">The controller instance.</param>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The model object passed to the view.</param>
        /// <returns>An <see cref="IActionResult"/> configured for Turbo Stream rendering.</returns>
        public static IActionResult TurboStream(this Controller controller, string viewName, object model)
        {
            var result = controller.View(viewName, model);
            result.ContentType = TurboStreamContentType;
            return result;
        }
    }
}
