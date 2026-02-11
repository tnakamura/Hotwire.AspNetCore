using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus 値を HTML 要素に設定する Tag Helper
    /// </summary>
    /// <example>
    /// &lt;div stimulus-value-dropdown-open="false"&gt;&lt;/div&gt;
    /// → &lt;div data-dropdown-open-value="false"&gt;&lt;/div&gt;
    /// </example>
    [HtmlTargetElement(Attributes = "stimulus-value-*")]
    public class StimulusValueTagHelper : TagHelper
    {
        private readonly IDictionary<string, string> _values = 
            new Dictionary<string, string>();

        /// <summary>
        /// 動的に Stimulus 値を受け付ける
        /// 例: stimulus-value-dropdown-open="true"
        /// </summary>
        [HtmlAttributeName("stimulus-value-", DictionaryAttributePrefix = "stimulus-value-")]
        public IDictionary<string, string> Values
        {
            get => _values;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var kvp in Values)
            {
                // stimulus-value-dropdown-open → data-dropdown-open-value
                var key = kvp.Key; // 例: "dropdown-open"
                var value = kvp.Value;
                
                output.Attributes.SetAttribute($"data-{key}-value", value);
            }
            
            // stimulus-value-* 属性自体は削除
            var attributesToRemove = output.Attributes
                .Where(attr => attr.Name.StartsWith("stimulus-value-"))
                .ToList();
            
            foreach (var attr in attributesToRemove)
            {
                output.Attributes.Remove(attr);
            }
        }
    }
}
