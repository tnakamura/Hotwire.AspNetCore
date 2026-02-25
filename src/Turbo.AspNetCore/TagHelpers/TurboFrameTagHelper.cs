using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// A tag helper that renders a <c>turbo-frame</c> element.
    /// </summary>
    public class TurboFrameTagHelper : TagHelper
    {
        /// <summary>
        /// Processes the tag helper output and sets the tag name to <c>turbo-frame</c>.
        /// </summary>
        /// <param name="context">The tag helper context.</param>
        /// <param name="output">The tag helper output.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "turbo-frame";
        }
    }
}
