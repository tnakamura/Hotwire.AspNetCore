using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Turbo Drive でページ遷移時に保持される永続的な要素を定義
    /// </summary>
    [HtmlTargetElement("turbo-permanent")]
    public class TurboPermanentTagHelper : TagHelper
    {
        /// <summary>
        /// 要素の一意な ID（必須）
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
