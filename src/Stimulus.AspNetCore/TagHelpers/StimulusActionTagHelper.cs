using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper that connects Stimulus actions to HTML elements.
    /// </summary>
    /// <example>
    /// &lt;button stimulus-action="click->dropdown#toggle"&gt;Toggle&lt;/button&gt;
    /// → &lt;button data-action="click->dropdown#toggle"&gt;Toggle&lt;/button&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-action")]
    public class StimulusActionTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus action (format: "event->controller#method").
        /// Multiple values can be specified (space-separated).
        /// </summary>
        [HtmlAttributeName("stimulus-action")]
        public string Action { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Action))
            {
                // Merge with an existing data-action attribute.
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
            
            // Remove the stimulus-action attribute itself.
            output.Attributes.RemoveAll("stimulus-action");
        }
    }
}
