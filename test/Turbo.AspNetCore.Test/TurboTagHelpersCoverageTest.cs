using Microsoft.AspNetCore.Razor.TagHelpers;
using Turbo.AspNetCore.TagHelpers;

namespace Turbo.AspNetCore.Test;

public class TurboTagHelpersCoverageTest
{
    [Fact]
    public void TurboDriveMetaTagHelper_DefaultSettings_RenderExpectedMetaTags()
    {
        var tagHelper = new TurboDriveMetaTagHelper();
        var output = CreateOutput("turbo-drive-meta");

        tagHelper.Process(CreateContext(), output);

        var html = output.Content.GetContent();
        Assert.Null(output.TagName);
        Assert.Contains("name=\"turbo-visit-control\" content=\"advance\"", html);
        Assert.Contains("name=\"turbo-refresh-method\" content=\"morph\"", html);
        Assert.Contains("name=\"turbo-refresh-scroll\" content=\"preserve\"", html);
        Assert.DoesNotContain("name=\"turbo-transition\"", html);
    }

    [Fact]
    public void TurboDriveMetaTagHelper_DisabledAndValidTransition_RendersReloadAndTransition()
    {
        var tagHelper = new TurboDriveMetaTagHelper
        {
            Enabled = false,
            Transition = "slide"
        };
        var output = CreateOutput("turbo-drive-meta");

        tagHelper.Process(CreateContext(), output);

        var html = output.Content.GetContent();
        Assert.Contains("name=\"turbo-visit-control\" content=\"reload\"", html);
        Assert.Contains("name=\"turbo-transition\" content=\"slide\"", html);
    }

    [Fact]
    public void TurboDriveMetaTagHelper_InvalidTransition_DoesNotRenderTransitionTag()
    {
        var tagHelper = new TurboDriveMetaTagHelper
        {
            Transition = "javascript:alert(1)"
        };
        var output = CreateOutput("turbo-drive-meta");

        tagHelper.Process(CreateContext(), output);

        Assert.DoesNotContain("name=\"turbo-transition\"", output.Content.GetContent());
    }

    [Fact]
    public void TurboFrameTagHelper_RendersTurboFrameTag()
    {
        var tagHelper = new TurboFrameTagHelper();
        var output = CreateOutput("turbo-frame");

        tagHelper.Process(CreateContext(), output);

        Assert.Equal("turbo-frame", output.TagName);
    }

    [Fact]
    public void TurboPermanentTagHelper_WithValidId_SetsPermanentAttributes()
    {
        var tagHelper = new TurboPermanentTagHelper
        {
            Id = "sidebar_1"
        };
        var output = CreateOutput("turbo-permanent");

        tagHelper.Process(CreateContext(), output);

        Assert.Equal("div", output.TagName);
        Assert.Equal("sidebar_1", output.Attributes["id"].Value);
        Assert.NotNull(output.Attributes["data-turbo-permanent"]);
    }

    [Fact]
    public void TurboPermanentTagHelper_WithInvalidId_SkipsIdButKeepsPermanentAttribute()
    {
        var tagHelper = new TurboPermanentTagHelper
        {
            Id = "1invalid"
        };
        var output = CreateOutput("turbo-permanent");

        tagHelper.Process(CreateContext(), output);

        Assert.Equal("div", output.TagName);
        Assert.DoesNotContain(output.Attributes, x => x.Name == "id");
        Assert.NotNull(output.Attributes["data-turbo-permanent"]);
    }

    public static TheoryData<TurboStreamActionTagHelper, string> SingleTargetActions() =>
        new()
        {
            { new TurboStreamPrependTagHelper(), "prepend" },
            { new TurboStreamAppendTagHelper(), "append" },
            { new TurboStreamBeforeTagHelper(), "before" },
            { new TurboStreamAfterTagHelper(), "after" },
            { new TurboStreamReplaceTagHelper(), "replace" },
            { new TurboStreamUpdateTagHelper(), "update" },
            { new TurboStreamRemoveTagHelper(), "remove" },
            { new TurboStreamMorphTagHelper(), "morph" }
        };

    [Theory]
    [MemberData(nameof(SingleTargetActions))]
    public void TurboStreamSingleTargetActionTagHelpers_SetExpectedAction(TurboStreamActionTagHelper tagHelper, string expectedAction)
    {
        tagHelper.Target = "message";
        var output = CreateOutput("turbo-stream-action");

        tagHelper.Process(CreateContext(), output);

        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal(expectedAction, output.Attributes["action"].Value);
        Assert.Equal("message", output.Attributes["target"].Value);
    }

    public static TheoryData<TurboStreamActionAllTagHelper, string> MultiTargetActions() =>
        new()
        {
            { new TurboStreamPrependAllTagHelper(), "prepend" },
            { new TurboStreamAppendAllTagHelper(), "append" },
            { new TurboStreamBeforeAllTagHelper(), "before" },
            { new TurboStreamAfterAllTagHelper(), "after" },
            { new TurboStreamReplaceAllTagHelper(), "replace" },
            { new TurboStreamUpdateAllTagHelper(), "update" },
            { new TurboStreamRemoveAllTagHelper(), "remove" },
            { new TurboStreamMorphAllTagHelper(), "morph" }
        };

    [Theory]
    [MemberData(nameof(MultiTargetActions))]
    public void TurboStreamMultiTargetActionTagHelpers_SetExpectedAction(TurboStreamActionAllTagHelper tagHelper, string expectedAction)
    {
        tagHelper.Targets = ".message";
        var output = CreateOutput("turbo-stream-action-all");

        tagHelper.Process(CreateContext(), output);

        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal(expectedAction, output.Attributes["action"].Value);
        Assert.Equal(".message", output.Attributes["targets"].Value);
    }

    private static TagHelperContext CreateContext() =>
        new(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));

    private static TagHelperOutput CreateOutput(string tagName) =>
        new(
            tagName,
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
}
