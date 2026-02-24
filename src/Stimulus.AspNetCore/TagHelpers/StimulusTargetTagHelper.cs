using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper that sets Stimulus targets on HTML elements.
    /// </summary>
    /// <example>
    /// &lt;div stimulus-target="dropdown.menu"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-target="menu"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-target")]
    public class StimulusTargetTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus target (format: "controller.target" or "target").
        /// Multiple values can be specified (space-separated).
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
                        // "controller.target" format.
                        var controller = parts[0].Trim();
                        var targetName = parts[1].Trim();
                        var attributeName = $"data-{controller}-target";
                        
                        // Merge with an existing target attribute.
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
            
            // Remove the stimulus-target attribute itself.
            output.Attributes.RemoveAll("stimulus-target");
        }
    }
}
