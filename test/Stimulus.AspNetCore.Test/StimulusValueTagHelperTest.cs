using Microsoft.AspNetCore.Razor.TagHelpers;
using Stimulus.AspNetCore.TagHelpers;

namespace Stimulus.AspNetCore.Test;

public class StimulusValueTagHelperTest
{
    [Fact]
    public void StimulusValueTagHelper_SetsSingleValue()
    {
        // Arrange
        var tagHelper = new StimulusValueTagHelper();
        tagHelper.Values["dropdown-open"] = "false";
        
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList
            {
                new TagHelperAttribute("stimulus-value-dropdown-open", "false")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("false", output.Attributes["data-dropdown-open-value"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name.StartsWith("stimulus-value-"));
    }

    [Fact]
    public void StimulusValueTagHelper_SetsMultipleValues()
    {
        // Arrange
        var tagHelper = new StimulusValueTagHelper();
        tagHelper.Values["counter-count"] = "42";
        tagHelper.Values["counter-step"] = "1";
        
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList
            {
                new TagHelperAttribute("stimulus-value-counter-count", "42"),
                new TagHelperAttribute("stimulus-value-counter-step", "1")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("42", output.Attributes["data-counter-count-value"].Value);
        Assert.Equal("1", output.Attributes["data-counter-step-value"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name.StartsWith("stimulus-value-"));
    }

    [Fact]
    public void StimulusValueTagHelper_HandlesStringValue()
    {
        // Arrange
        var tagHelper = new StimulusValueTagHelper();
        tagHelper.Values["search-query"] = "hello world";
        
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("hello world", output.Attributes["data-search-query-value"].Value);
    }

    [Fact]
    public void StimulusValueTagHelper_HandlesJsonValue()
    {
        // Arrange
        var tagHelper = new StimulusValueTagHelper();
        tagHelper.Values["map-config"] = "{\"lat\":35.6762,\"lng\":139.6503}";
        
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("{\"lat\":35.6762,\"lng\":139.6503}", output.Attributes["data-map-config-value"].Value);
    }
}
