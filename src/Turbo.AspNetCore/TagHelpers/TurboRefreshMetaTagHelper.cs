using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag helper for setting the turbo-refresh-method meta tag.
    /// Controls whether Turbo uses morphing for page refreshes.
    /// </summary>
    /// <example>
    /// &lt;turbo-refresh-method content="morph" /&gt;
    /// </example>
    [HtmlTargetElement("turbo-refresh-method")]
    public class TurboRefreshMethodMetaTagHelper : TagHelper
    {
        /// <summary>
        /// The refresh method: "replace" (default) or "morph".
        /// </summary>
        public string Content { get; set; } = "morph";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "meta";
            output.Attributes.SetAttribute("name", "turbo-refresh-method");
            output.Attributes.SetAttribute("content", Content);
            output.TagMode = TagMode.SelfClosing;
        }
    }

    /// <summary>
    /// Tag helper for setting the turbo-refresh-scroll meta tag.
    /// Controls whether scroll position is preserved during page refreshes.
    /// </summary>
    /// <example>
    /// &lt;turbo-refresh-scroll content="preserve" /&gt;
    /// </example>
    [HtmlTargetElement("turbo-refresh-scroll")]
    public class TurboRefreshScrollMetaTagHelper : TagHelper
    {
        /// <summary>
        /// The scroll behavior: "reset" (default) or "preserve".
        /// </summary>
        public string Content { get; set; } = "preserve";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "meta";
            output.Attributes.SetAttribute("name", "turbo-refresh-scroll");
            output.Attributes.SetAttribute("content", Content);
            output.TagMode = TagMode.SelfClosing;
        }
    }
}
