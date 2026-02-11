using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Stimulus.AspNetCore
{
    /// <summary>
    /// Stimulus data 属性を生成するための拡張メソッド
    /// </summary>
    public static class StimulusHtmlExtensions
    {
        /// <summary>
        /// Stimulus コントローラー属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusController(
            this IHtmlHelper html, 
            string controller)
        {
            return new Dictionary<string, object>
            {
                { "data-controller", controller }
            };
        }

        /// <summary>
        /// Stimulus コントローラー属性を生成（複数）
        /// </summary>
        public static IDictionary<string, object> StimulusController(
            this IHtmlHelper html, 
            params string[] controllers)
        {
            return new Dictionary<string, object>
            {
                { "data-controller", string.Join(" ", controllers) }
            };
        }

        /// <summary>
        /// Stimulus アクション属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusAction(
            this IHtmlHelper html, 
            string action)
        {
            return new Dictionary<string, object>
            {
                { "data-action", action }
            };
        }

        /// <summary>
        /// Stimulus アクション属性を生成（複数）
        /// </summary>
        public static IDictionary<string, object> StimulusAction(
            this IHtmlHelper html, 
            params string[] actions)
        {
            return new Dictionary<string, object>
            {
                { "data-action", string.Join(" ", actions) }
            };
        }

        /// <summary>
        /// Stimulus ターゲット属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusTarget(
            this IHtmlHelper html, 
            string controller, 
            string target)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-target", target }
            };
        }

        /// <summary>
        /// Stimulus ターゲット属性を生成（複数ターゲット）
        /// </summary>
        public static IDictionary<string, object> StimulusTarget(
            this IHtmlHelper html, 
            string controller, 
            params string[] targets)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-target", string.Join(" ", targets) }
            };
        }

        /// <summary>
        /// Stimulus 値属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusValue(
            this IHtmlHelper html, 
            string controller, 
            string name, 
            object value)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-{name}-value", value.ToString() }
            };
        }

        /// <summary>
        /// Stimulus クラス属性を生成
        /// </summary>
        public static IDictionary<string, object> StimulusClass(
            this IHtmlHelper html, 
            string controller, 
            string name, 
            string className)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-{name}-class", className }
            };
        }

        /// <summary>
        /// 複数の Stimulus 属性を結合
        /// </summary>
        public static IDictionary<string, object> StimulusAttributes(
            this IHtmlHelper html,
            params IDictionary<string, object>[] attributeSets)
        {
            var combined = new Dictionary<string, object>();
            
            foreach (var set in attributeSets)
            {
                foreach (var kvp in set)
                {
                    if (combined.ContainsKey(kvp.Key))
                    {
                        // data-action などは結合
                        combined[kvp.Key] = $"{combined[kvp.Key]} {kvp.Value}";
                    }
                    else
                    {
                        combined[kvp.Key] = kvp.Value;
                    }
                }
            }
            
            return combined;
        }
    }
}
