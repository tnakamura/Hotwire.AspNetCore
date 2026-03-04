using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Tag Helper for custom Turbo Stream actions.
    /// Allows users to define and use custom actions beyond the standard Turbo Stream actions.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;turbo-stream-custom action="set_title" title="New Page Title"&gt;&lt;/turbo-stream-custom&gt;
    /// 
    /// &lt;turbo-stream-custom action="notify" message="Success!" type="success"&gt;&lt;/turbo-stream-custom&gt;
    /// 
    /// &lt;turbo-stream-custom action="slide_in" target="notifications"&gt;
    ///   &lt;div class="notification"&gt;New notification&lt;/div&gt;
    /// &lt;/turbo-stream-custom&gt;
    /// </code>
    /// </example>
    [HtmlTargetElement("turbo-stream-custom", TagStructure = TagStructure.NormalOrSelfClosing)]
    public sealed class TurboStreamCustomActionTagHelper : TurboStreamTagHelper
    {
        /// <summary>
        /// The name of the custom action. This should match the action name
        /// registered in JavaScript via Turbo.StreamActions.
        /// </summary>
        [HtmlAttributeName("action")]
        public string? Action { get; set; }

        /// <summary>
        /// Additional attributes passed to <c>turbo-stream-custom</c>.
        /// These are emitted to the resulting <c>turbo-stream</c> element.
        /// </summary>
        [HtmlAttributeName(DictionaryAttributePrefix = "")]
        public IDictionary<string, string?> AdditionalAttributes { get; set; } = new Dictionary<string, string?>();

        /// <summary>
        /// Processes the tag helper and sets the action attribute.
        /// All other attributes will be passed through to the turbo-stream element automatically.
        /// </summary>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var attribute in AdditionalAttributes)
            {
                if (attribute.Key.Equals("action", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                output.Attributes.SetAttribute(attribute.Key, attribute.Value);
            }

            // Set the action attribute
            if (!string.IsNullOrEmpty(Action))
            {
                output.Attributes.SetAttribute("action", Action);
            }

            // Call base to generate the turbo-stream structure
            base.Process(context, output);
        }
    }
}
