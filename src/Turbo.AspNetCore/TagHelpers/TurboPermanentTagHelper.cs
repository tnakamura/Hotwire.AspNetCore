using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Defines a persistent element that is preserved across Turbo Drive page transitions.
    /// </summary>
    [HtmlTargetElement("turbo-permanent")]
    public class TurboPermanentTagHelper : TagHelper
    {
        /// <summary>
        /// Unique ID for the element (required).
        /// </summary>
        public string Id { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            
            // Validate ID to ensure it's a valid HTML ID (alphanumeric, hyphens, underscores)
            if (!string.IsNullOrEmpty(Id) && Regex.IsMatch(Id, @"^[a-zA-Z][\w\-]*$"))
            {
                output.Attributes.SetAttribute("id", Id);
                output.Attributes.SetAttribute("data-turbo-permanent", "");
            }
            else
            {
                // If ID is invalid, render without the attributes (graceful degradation)
                output.Attributes.SetAttribute("data-turbo-permanent", "");
            }
        }
    }
}
