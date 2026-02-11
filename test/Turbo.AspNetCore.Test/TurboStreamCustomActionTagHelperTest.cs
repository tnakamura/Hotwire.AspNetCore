using Microsoft.AspNetCore.Razor.TagHelpers;
using Turbo.AspNetCore.TagHelpers;

namespace Turbo.AspNetCore.Test;

public class TurboStreamCustomActionTagHelperTest
{
    [Fact]
    public void CustomAction_WithActionOnly_GeneratesCorrectHtml()
    {
        // Arrange
        var tagHelper = new TurboStreamCustomActionTagHelper
        {
            Action = "set_title"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("title", "New Title")
        };

        var output = new TagHelperOutput(
            "turbo-stream-custom",
            attributes,
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("set_title", output.Attributes["action"].Value);
        Assert.Equal("New Title", output.Attributes["title"].Value);
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }

    [Fact]
    public void CustomAction_WithMultipleAttributes_PassesThroughAllAttributes()
    {
        // Arrange
        var tagHelper = new TurboStreamCustomActionTagHelper
        {
            Action = "notify"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("message", "Success!"),
            new TagHelperAttribute("type", "success"),
            new TagHelperAttribute("duration", "3000")
        };

        var output = new TagHelperOutput(
            "turbo-stream-custom",
            attributes,
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("notify", output.Attributes["action"].Value);
        Assert.Equal("Success!", output.Attributes["message"].Value);
        Assert.Equal("success", output.Attributes["type"].Value);
        Assert.Equal("3000", output.Attributes["duration"].Value);
    }

    [Fact]
    public void CustomAction_WithTargetAttribute_GeneratesCorrectHtml()
    {
        // Arrange
        var tagHelper = new TurboStreamCustomActionTagHelper
        {
            Action = "slide_in"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var attributes = new TagHelperAttributeList
        {
            new TagHelperAttribute("target", "notifications")
        };

        var output = new TagHelperOutput(
            "turbo-stream-custom",
            attributes,
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                new DefaultTagHelperContent()));

        output.Content.SetHtmlContent("<div class=\"notification\">Content</div>");

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("slide_in", output.Attributes["action"].Value);
        Assert.Equal("notifications", output.Attributes["target"].Value);
        Assert.Contains("<div class=\"notification\">Content</div>",
            output.Content.GetContent());
    }

    [Fact]
    public void CustomAction_WithEmptyAction_StillGeneratesHtml()
    {
        // Arrange
        var tagHelper = new TurboStreamCustomActionTagHelper
        {
            Action = ""
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var output = new TagHelperOutput(
            "turbo-stream-custom",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        // Empty action should not set the action attribute
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }

    [Fact]
    public void CustomAction_WithNullAction_DoesNotSetActionAttribute()
    {
        // Arrange
        var tagHelper = new TurboStreamCustomActionTagHelper
        {
            Action = null
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        var output = new TagHelperOutput(
            "turbo-stream-custom",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(
                new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "action");
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }
}
