using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper that connects a Stimulus controller to an HTML element.
    /// </summary>
    /// <example>
    /// &lt;div stimulus-controller="dropdown"&gt;&lt;/div&gt;
    /// → &lt;div data-controller="dropdown"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-controller")]
    public class StimulusControllerTagHelper : TagHelper
    {
        /// <summary>
        /// Stimulus controller name (multiple values allowed, space-separated).
        /// </summary>
        [HtmlAttributeName("stimulus-controller")]
        public string Controller { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Controller))
            {
                output.Attributes.SetAttribute("data-controller", Controller.Trim());
            }
            
            // Remove the stimulus-controller attribute itself.
            output.Attributes.RemoveAll("stimulus-controller");
        }
    }
}
