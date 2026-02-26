using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper that enables Turbo Stream responses for <c>form</c> and <c>a</c> elements.
    /// </summary>
    /// <example>
    /// &lt;form turbo-stream&gt;&lt;/form&gt;
    /// → &lt;form data-turbo-stream=""&gt;&lt;/form&gt;
    ///
    /// &lt;a href="/messages" turbo-stream&gt;Messages&lt;/a&gt;
    /// → &lt;a href="/messages" data-turbo-stream=""&gt;Messages&lt;/a&gt;
    /// </example>
    [HtmlTargetElement("form", Attributes = "turbo-stream")]
    [HtmlTargetElement("a", Attributes = "turbo-stream")]
    public sealed class TurboStreamRequestTagHelper : TagHelper
    {
        /// <summary>
        /// Whether to enable Turbo Stream handling for this request element.
        /// </summary>
        [HtmlAttributeName("turbo-stream")]
        public bool TurboStream { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (TurboStream)
            {
                output.Attributes.SetAttribute("data-turbo-stream", "");
            }

            output.Attributes.RemoveAll("turbo-stream");
        }
    }
}
