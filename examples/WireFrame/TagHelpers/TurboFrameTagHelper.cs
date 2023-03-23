using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Hotwire.AspNetCore.TagHelpers;

public class TurboFrameTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "turbo-frame";
    }
}
