using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper that sets Stimulus CSS class names on HTML elements.
    /// </summary>
    /// <example>
    /// &lt;div stimulus-class-dropdown-active="highlight"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-active-class="highlight"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-class-*")]
    public class StimulusClassTagHelper : TagHelper
    {
        private readonly IDictionary<string, string> _classes = 
            new Dictionary<string, string>();

        /// <summary>
        /// Accepts Stimulus CSS classes dynamically.
        /// Example: stimulus-class-dropdown-active="show"
        /// </summary>
        [HtmlAttributeName("stimulus-class-", DictionaryAttributePrefix = "stimulus-class-")]
        public IDictionary<string, string> Classes
        {
            get => _classes;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var kvp in Classes)
            {
                // stimulus-class-dropdown-active -> data-dropdown-active-class
                var key = kvp.Key; // Example: "dropdown-active"
                var value = kvp.Value;
                
                output.Attributes.SetAttribute($"data-{key}-class", value);
            }
            
            // Remove stimulus-class-* attributes themselves.
            var attributesToRemove = output.Attributes
                .Where(attr => attr.Name.StartsWith("stimulus-class-"))
                .ToList();
            
            foreach (var attr in attributesToRemove)
            {
                output.Attributes.Remove(attr);
            }
        }
    }
}
