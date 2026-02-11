using Microsoft.AspNetCore.Razor.TagHelpers;

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
            output.Attributes.SetAttribute("id", Id);
            output.Attributes.SetAttribute("data-turbo-permanent", "");
        }
    }
}
