using Microsoft.AspNetCore.Razor.TagHelpers;
using Turbo.AspNetCore.TagHelpers;

namespace Turbo.AspNetCore.Test;

public class TurboRefreshMetaTagHelperTest
{
    [Fact]
    public void TurboRefreshMethodMetaTagHelper_WithDefaultContent_GeneratesCorrectMeta()
    {
        // Arrange
        var tagHelper = new TurboRefreshMethodMetaTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-refresh-method",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("meta", output.TagName);
        Assert.Equal("turbo-refresh-method", output.Attributes["name"].Value);
        Assert.Equal("morph", output.Attributes["content"].Value);
        Assert.Equal(TagMode.SelfClosing, output.TagMode);
    }

    [Fact]
    public void TurboRefreshMethodMetaTagHelper_WithCustomContent_GeneratesCorrectMeta()
    {
        // Arrange
        var tagHelper = new TurboRefreshMethodMetaTagHelper
        {
            Content = "replace"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-refresh-method",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("meta", output.TagName);
        Assert.Equal("turbo-refresh-method", output.Attributes["name"].Value);
        Assert.Equal("replace", output.Attributes["content"].Value);
        Assert.Equal(TagMode.SelfClosing, output.TagMode);
    }

    [Fact]
    public void TurboRefreshScrollMetaTagHelper_WithDefaultContent_GeneratesCorrectMeta()
    {
        // Arrange
        var tagHelper = new TurboRefreshScrollMetaTagHelper();
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-refresh-scroll",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("meta", output.TagName);
        Assert.Equal("turbo-refresh-scroll", output.Attributes["name"].Value);
        Assert.Equal("preserve", output.Attributes["content"].Value);
        Assert.Equal(TagMode.SelfClosing, output.TagMode);
    }

    [Fact]
    public void TurboRefreshScrollMetaTagHelper_WithCustomContent_GeneratesCorrectMeta()
    {
        // Arrange
        var tagHelper = new TurboRefreshScrollMetaTagHelper
        {
            Content = "reset"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "turbo-refresh-scroll",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("meta", output.TagName);
        Assert.Equal("turbo-refresh-scroll", output.Attributes["name"].Value);
        Assert.Equal("reset", output.Attributes["content"].Value);
        Assert.Equal(TagMode.SelfClosing, output.TagMode);
    }
}
