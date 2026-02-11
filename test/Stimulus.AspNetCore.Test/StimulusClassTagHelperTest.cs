using Microsoft.AspNetCore.Razor.TagHelpers;
using Stimulus.AspNetCore.TagHelpers;

namespace Stimulus.AspNetCore.Test;

public class StimulusClassTagHelperTest
{
    [Fact]
    public void StimulusClassTagHelper_SetsSingleClass()
    {
        // Arrange
        var tagHelper = new StimulusClassTagHelper();
        tagHelper.Classes["dropdown-active"] = "show";
        
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList
            {
                new TagHelperAttribute("stimulus-class-dropdown-active", "show")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("show", output.Attributes["data-dropdown-active-class"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name.StartsWith("stimulus-class-"));
    }

    [Fact]
    public void StimulusClassTagHelper_SetsMultipleClasses()
    {
        // Arrange
        var tagHelper = new StimulusClassTagHelper();
        tagHelper.Classes["dropdown-active"] = "show";
        tagHelper.Classes["dropdown-loading"] = "spinner";
        
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList
            {
                new TagHelperAttribute("stimulus-class-dropdown-active", "show"),
                new TagHelperAttribute("stimulus-class-dropdown-loading", "spinner")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("show", output.Attributes["data-dropdown-active-class"].Value);
        Assert.Equal("spinner", output.Attributes["data-dropdown-loading-class"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name.StartsWith("stimulus-class-"));
    }

    [Fact]
    public void StimulusClassTagHelper_HandlesMultipleClassNames()
    {
        // Arrange
        var tagHelper = new StimulusClassTagHelper();
        tagHelper.Classes["list-loading"] = "spinner opacity-50";
        
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
        Assert.Equal("spinner opacity-50", output.Attributes["data-list-loading-class"].Value);
    }
}
