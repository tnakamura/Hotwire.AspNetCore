using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus アクションを HTML 要素に接続する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;button stimulus-action="click->dropdown#toggle"&gt;Toggle&lt;/button&gt;
    /// → &lt;button data-action="click->dropdown#toggle"&gt;Toggle&lt;/button&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-action")]
    public class StimulusActionTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus アクション（フォーマット: "event->controller#method"）
        /// 複数指定可能（スペース区切り）
        /// </summary>
        [HtmlAttributeName("stimulus-action")]
        public string Action { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Action))
            {
                // 既存の data-action と統合
                var existingAction = output.Attributes["data-action"]?.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(existingAction))
                {
                    output.Attributes.SetAttribute("data-action", 
                        $"{existingAction} {Action.Trim()}");
                }
                else
                {
                    output.Attributes.SetAttribute("data-action", Action.Trim());
                }
            }
            
            // stimulus-action 属性自体は削除
            output.Attributes.RemoveAll("stimulus-action");
        }
    }
}
