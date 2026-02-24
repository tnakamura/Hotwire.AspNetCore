using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Generates meta tags to control Turbo Drive behavior.
    /// </summary>
    [HtmlTargetElement("turbo-drive-meta")]
    public class TurboDriveMetaTagHelper : TagHelper
    {
        /// <summary>
        /// Enables or disables Turbo Drive (default: true).
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Animation used during page transitions (default: "").
        /// Allowed values: "fade", "slide", "none".
        /// </summary>
        public string Transition { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // Do not render the tag itself.
            output.TagMode = TagMode.StartTagAndEndTag;

            var sb = new StringBuilder();

            // Enable/disable Turbo Drive.
            // "advance": Enables Turbo Drive (default).
            // "reload": Forces a full page reload.
            sb.AppendLine($"<meta name=\"turbo-visit-control\" content=\"{(Enabled ? "advance" : "reload")}\">");

            // Turbo 8 feature: Page refresh behavior.
            sb.AppendLine("<meta name=\"turbo-refresh-method\" content=\"morph\">");
            sb.AppendLine("<meta name=\"turbo-refresh-scroll\" content=\"preserve\">");

            if (!string.IsNullOrEmpty(Transition))
            {
                // Validate transition value to prevent XSS
                var validTransitions = new[] { "fade", "slide", "none" };
                if (System.Array.IndexOf(validTransitions, Transition.ToLowerInvariant()) >= 0)
                {
                    sb.AppendLine($"<meta name=\"turbo-transition\" content=\"{Transition}\">");
                }
            }

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
