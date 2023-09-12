using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Turbo.AspNetCore.TagHelpers
{
    public abstract class TurboStreamTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "turbo-stream";
            output.PreContent.SetHtmlContent("<template>");
            output.PostContent.SetHtmlContent("</template>");
            base.Process(context, output);
        }
    }

    public abstract class TurboStreamActionTagHelper : TurboStreamTagHelper
    {
        public string Target { get; set; }

        private protected abstract string Action { get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("target", Target);
            output.Attributes.SetAttribute("action", Action);
            base.Process(context, output);
        }
    }

    public abstract class TurboStreamActionAllTagHelper : TurboStreamTagHelper
    {
        public string Targets { get; set; }

        private protected abstract string Action { get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("targets", Targets);
            output.Attributes.SetAttribute("action", Action);
            base.Process(context, output);
        }
    }

    public sealed class TurboStreamPrependTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "prepend";
    }

    public sealed class TurboStreamAppendTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "append";
    }

    public sealed class TurboStreamBeforeTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "before";
    }

    public sealed class TurboStreamAfterTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "after";
    }

    public sealed class TurboStreamReplaceTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "replace";
    }

    public sealed class TurboStreamUpdateTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "update";
    }

    public sealed class TurboStreamRemoveTagHelper : TurboStreamActionTagHelper
    {
        private protected override string Action => "remove";
    }

    public sealed class TurboStreamPrependAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "prepend";
    }

    public sealed class TurboStreamAppendAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "append";
    }

    public sealed class TurboStreamBeforeAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "before";
    }

    public sealed class TurboStreamAfterAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "after";
    }

    public sealed class TurboStreamReplaceAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "replace";
    }

    public sealed class TurboStreamUpdateAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "update";
    }

    public sealed class TurboStreamRemoveAllTagHelper : TurboStreamActionAllTagHelper
    {
        private protected override string Action => "remove";
    }
}
