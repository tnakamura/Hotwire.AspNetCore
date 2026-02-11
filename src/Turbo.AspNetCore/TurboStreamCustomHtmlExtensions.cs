using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Text.Encodings.Web;

namespace Turbo.AspNetCore
{
    /// <summary>
    /// HTML helper extensions for custom Turbo Stream actions.
    /// </summary>
    public static class TurboStreamCustomHtmlExtensions
    {
        /// <summary>
        /// Generates a custom Turbo Stream action without content.
        /// </summary>
        /// <param name="html">The HTML helper.</param>
        /// <param name="action">The name of the custom action.</param>
        /// <param name="attributes">Additional HTML attributes for the turbo-stream element.</param>
        /// <returns>An IHtmlContent representing the turbo-stream element.</returns>
        /// <example>
        /// <code>
        /// @Html.TurboStreamCustom("set_title", new { title = "New Page Title" })
        /// </code>
        /// </example>
        public static IHtmlContent TurboStreamCustom(
            this IHtmlHelper html,
            string action,
            object attributes = null)
        {
            return TurboStreamCustom(html, action, attributes, content: null);
        }

        /// <summary>
        /// Generates a custom Turbo Stream action with content.
        /// </summary>
        /// <param name="html">The HTML helper.</param>
        /// <param name="action">The name of the custom action.</param>
        /// <param name="attributes">Additional HTML attributes for the turbo-stream element.</param>
        /// <param name="content">The content to include in the template.</param>
        /// <returns>An IHtmlContent representing the turbo-stream element.</returns>
        /// <example>
        /// <code>
        /// @Html.TurboStreamCustom("notify", new { message = "Success!", type = "info" }, 
        ///     @&lt;div class="alert"&gt;Notification&lt;/div&gt;)
        /// </code>
        /// </example>
        public static IHtmlContent TurboStreamCustom(
            this IHtmlHelper html,
            string action,
            object attributes,
            Func<object, IHtmlContent> content)
        {
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentException("Action name cannot be null or empty.", nameof(action));
            }

            var tagBuilder = new TagBuilder("turbo-stream");
            tagBuilder.Attributes.Add("action", action);

            // Add custom attributes
            if (attributes != null)
            {
                var routeValueDictionary = new RouteValueDictionary(attributes);
                foreach (var attr in routeValueDictionary)
                {
                    var key = attr.Key.Replace('_', '-'); // Convert underscores to hyphens (e.g., data_value -> data-value)
                    tagBuilder.Attributes.Add(key, attr.Value?.ToString());
                }
            }

            // Build template content
            var templateBuilder = new TagBuilder("template");
            if (content != null)
            {
                var contentHtml = content(null);
                if (contentHtml != null)
                {
                    using (var writer = new StringWriter())
                    {
                        contentHtml.WriteTo(writer, HtmlEncoder.Default);
                        templateBuilder.InnerHtml.AppendHtml(writer.ToString());
                    }
                }
            }

            tagBuilder.InnerHtml.AppendHtml(templateBuilder);

            return tagBuilder;
        }
    }
}
