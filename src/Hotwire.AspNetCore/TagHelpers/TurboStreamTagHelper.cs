using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hotwire.AspNetCore.TagHelpers
{
    public class TurboStreamTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "turbo-stream";
            output.PreContent.SetHtmlContent("<template>");
            output.PostContent.SetHtmlContent("</template>");
        }
    }
}
