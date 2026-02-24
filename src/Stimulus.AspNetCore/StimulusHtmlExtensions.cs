using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Stimulus.AspNetCore
{
    /// <summary>
    /// Extension methods for generating Stimulus data attributes.
    /// </summary>
    public static class StimulusHtmlExtensions
    {
        /// <summary>
        /// Generates a Stimulus controller attribute.
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
        /// Generates Stimulus controller attributes (multiple controllers).
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
        /// Generates a Stimulus action attribute.
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
        /// Generates Stimulus action attributes (multiple actions).
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
        /// Generates a Stimulus target attribute.
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
        /// Generates a Stimulus target attribute (multiple targets).
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
        /// Generates a Stimulus value attribute.
        /// </summary>
        public static IDictionary<string, object> StimulusValue(
            this IHtmlHelper html, 
            string controller, 
            string name, 
            object value)
        {
            return new Dictionary<string, object>
            {
                { $"data-{controller}-{name}-value", value?.ToString() ?? string.Empty }
            };
        }

        /// <summary>
        /// Generates a Stimulus class attribute.
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
        /// Combines multiple Stimulus attributes.
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
                        // Merge values for attributes such as data-action.
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
