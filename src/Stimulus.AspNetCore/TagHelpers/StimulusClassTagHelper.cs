using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Stimulus.AspNetCore.TagHelpers
{
    /// <summary>
    /// Stimulus CSS クラス名を HTML 要素に設定する Tag Helper
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
        /// 動的に Stimulus CSS クラスを受け付ける
        /// 例: stimulus-class-dropdown-active="show"
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
                // stimulus-class-dropdown-active → data-dropdown-active-class
                var key = kvp.Key; // 例: "dropdown-active"
                var value = kvp.Value;
                
                output.Attributes.SetAttribute($"data-{key}-class", value);
            }
            
            // stimulus-class-* 属性自体は削除
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
