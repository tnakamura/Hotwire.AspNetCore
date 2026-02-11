using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus コントローラーを HTML 要素に接続する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-controller="dropdown"&gt;&lt;/div&gt;
    /// → &lt;div data-controller="dropdown"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-controller")]
    public class StimulusControllerTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus コントローラー名（複数指定可能、スペース区切り）
        /// </summary>
        [HtmlAttributeName("stimulus-controller")]
        public string Controller { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Controller))
            {
                output.Attributes.SetAttribute("data-controller", Controller.Trim());
            }
            
            // stimulus-controller 属性自体は削除
            output.Attributes.RemoveAll("stimulus-controller");
        }
    }
}
