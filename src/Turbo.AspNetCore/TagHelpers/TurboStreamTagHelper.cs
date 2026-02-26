using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    /// <summary>
    /// Base tag helper for rendering a <c>turbo-stream</c> element with a <c>template</c> wrapper.
    /// </summary>
    public abstract class TurboStreamTagHelper : TagHelper
    {
        /// <summary>
        /// Processes the tag helper output as a <c>turbo-stream</c> element.
        /// </summary>
        /// <param name="context">The tag helper context.</param>
        /// <param name="output">The tag helper output.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "turbo-stream";
            var content = output.Content.GetContent().Trim();
            if (!IsTemplateRoot(content))
            {
                output.PreContent.SetHtmlContent("<template>");
                output.PostContent.SetHtmlContent("</template>");
            }

            base.Process(context, output);
        }

        private static bool IsTemplateRoot(string content)
        {
            if (!content.StartsWith("<template", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var openTagEnd = content.IndexOf('>');
            if (openTagEnd < 0)
            {
                return false;
            }

            return content.EndsWith("</template>", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Base tag helper for Turbo Stream actions that target a single element.
    /// </summary>
    public abstract class TurboStreamActionTagHelper : TurboStreamTagHelper
    {
        /// <summary>
        /// Gets or sets the target element ID.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets the Turbo Stream action name.
        /// </summary>
        private protected abstract string Action { get; }

        /// <summary>
        /// Processes the tag helper output and sets the <c>target</c> and <c>action</c> attributes.
        /// </summary>
        /// <param name="context">The tag helper context.</param>
        /// <param name="output">The tag helper output.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("target", Target);
            output.Attributes.SetAttribute("action", Action);
            base.Process(context, output);
        }
    }

    /// <summary>
    /// Base tag helper for Turbo Stream actions that target multiple elements.
    /// </summary>
    public abstract class TurboStreamActionAllTagHelper : TurboStreamTagHelper
    {
        /// <summary>
        /// Gets or sets the target selector for multiple elements.
        /// </summary>
        public string Targets { get; set; }

        /// <summary>
        /// Gets the Turbo Stream action name.
        /// </summary>
        private protected abstract string Action { get; }

        /// <summary>
        /// Processes the tag helper output and sets the <c>targets</c> and <c>action</c> attributes.
        /// </summary>
        /// <param name="context">The tag helper context.</param>
        /// <param name="output">The tag helper output.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("targets", Targets);
            output.Attributes.SetAttribute("action", Action);
            base.Process(context, output);
        }
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>prepend</c> for a single target.
    /// </summary>
    public sealed class TurboStreamPrependTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "prepend";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>append</c> for a single target.
    /// </summary>
    public sealed class TurboStreamAppendTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "append";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>before</c> for a single target.
    /// </summary>
    public sealed class TurboStreamBeforeTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "before";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>after</c> for a single target.
    /// </summary>
    public sealed class TurboStreamAfterTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "after";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>replace</c> for a single target.
    /// </summary>
    public sealed class TurboStreamReplaceTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "replace";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>update</c> for a single target.
    /// </summary>
    public sealed class TurboStreamUpdateTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "update";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>remove</c> for a single target.
    /// </summary>
    public sealed class TurboStreamRemoveTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "remove";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>prepend</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamPrependAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "prepend";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>append</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamAppendAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "append";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>before</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamBeforeAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "before";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>after</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamAfterAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "after";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>replace</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamReplaceAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "replace";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>update</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamUpdateAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "update";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>remove</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamRemoveAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "remove";
    }

    // Turbo 8 new actions
    /// <summary>
    /// Renders a Turbo Stream action with <c>morph</c> for a single target.
    /// </summary>
    public sealed class TurboStreamMorphTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "morph";
    }

    /// <summary>
    /// Renders a Turbo Stream action with <c>morph</c> for multiple targets.
    /// </summary>
    public sealed class TurboStreamMorphAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "morph";
    }

    /// <summary>
    /// Renders a Turbo Stream <c>refresh</c> action.
    /// </summary>
    public sealed class TurboStreamRefreshTagHelper : TurboStreamTagHelper
    {
        /// <summary>
        /// Gets or sets the optional request identifier used for refresh deduplication.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Processes the tag helper output and sets the <c>refresh</c> action attributes.
        /// </summary>
        /// <param name="context">The tag helper context.</param>
        /// <param name="output">The tag helper output.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("action", "refresh");
            if (!string.IsNullOrEmpty(RequestId))
            {
                output.Attributes.SetAttribute("request-id", RequestId);
            }
            base.Process(context, output);
        }
    }
}
