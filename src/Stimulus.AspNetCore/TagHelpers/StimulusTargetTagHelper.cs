using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus ターゲットを HTML 要素に設定する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-target="dropdown.menu"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-target="menu"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-target")]
    public class StimulusTargetTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus ターゲット（フォーマット: "controller.target" または "target"）
        /// 複数指定可能（スペース区切り）
        /// </summary>
        [HtmlAttributeName("stimulus-target")]
        public string Target { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Target))
            {
                var targets = Target.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var target in targets)
                {
                    var parts = target.Split('.');
                    
                    if (parts.Length == 2)
                    {
                        // "controller.target" 形式
                        var controller = parts[0].Trim();
                        var targetName = parts[1].Trim();
                        var attributeName = $"data-{controller}-target";
                        
                        // 既存の target 属性と統合
                        var existingTarget = output.Attributes[attributeName]?.Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(existingTarget))
                        {
                            output.Attributes.SetAttribute(attributeName, 
                                $"{existingTarget} {targetName}");
                        }
                        else
                        {
                            output.Attributes.SetAttribute(attributeName, targetName);
                        }
                    }
                }
            }
            
            // stimulus-target 属性自体は削除
            output.Attributes.RemoveAll("stimulus-target");
        }
    }
}
