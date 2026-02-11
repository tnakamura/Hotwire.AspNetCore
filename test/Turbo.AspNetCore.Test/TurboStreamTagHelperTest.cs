using Microsoft.AspNetCore.Razor.TagHelpers;
using Turbo.AspNetCore.TagHelpers;

namespace Turbo.AspNetCore.Test;

public class TurboStreamTagHelperTest
{
    [Fact]
    public void TurboStreamMorphTagHelper_SetsCorrectAction()
    {
        // Arrange
        var tagHelper = new TurboStreamMorphTagHelper
        {
            Target = "message"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-stream-morph",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("morph", output.Attributes["action"].Value);
        Assert.Equal("message", output.Attributes["target"].Value);
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }

    [Fact]
    public void TurboStreamMorphAllTagHelper_SetsCorrectAction()
    {
        // Arrange
        var tagHelper = new TurboStreamMorphAllTagHelper
        {
            Targets = ".message"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-stream-morph-all",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("morph", output.Attributes["action"].Value);
        Assert.Equal(".message", output.Attributes["targets"].Value);
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }

    [Fact]
    public void TurboStreamRefreshTagHelper_WithoutRequestId_SetsCorrectAction()
    {
        // Arrange
        var tagHelper = new TurboStreamRefreshTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-stream-refresh",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("refresh", output.Attributes["action"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "request-id");
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }

    [Fact]
    public void TurboStreamRefreshTagHelper_WithRequestId_SetsRequestIdAttribute()
    {
        // Arrange
        var tagHelper = new TurboStreamRefreshTagHelper
        {
            RequestId = "123"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-stream-refresh",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("turbo-stream", output.TagName);
        Assert.Equal("refresh", output.Attributes["action"].Value);
        Assert.Equal("123", output.Attributes["request-id"].Value);
        Assert.Contains("<template>", output.PreContent.GetContent());
        Assert.Contains("</template>", output.PostContent.GetContent());
    }
}
