using Microsoft.AspNetCore.Razor.TagHelpers;
using Stimulus.AspNetCore.TagHelpers;

namespace Stimulus.AspNetCore.Test;

public class StimulusControllerTagHelperTest
{
    [Fact]
    public void StimulusControllerTagHelper_SetsDataController()
    {
        // Arrange
        var tagHelper = new StimulusControllerTagHelper
        {
            Controller = "dropdown"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "div",
            new TagHelperAttributeList 
            {
                new TagHelperAttribute("stimulus-controller", "dropdown")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("dropdown", output.Attributes["data-controller"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-controller");
    }

    [Fact]
    public void StimulusControllerTagHelper_SupportsMultipleControllers()
    {
        // Arrange
        var tagHelper = new StimulusControllerTagHelper
        {
            Controller = "dropdown clipboard"
        };
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
        Assert.Equal("dropdown clipboard", output.Attributes["data-controller"].Value);
    }

    [Fact]
    public void StimulusControllerTagHelper_HandlesEmptyController()
    {
        // Arrange
        var tagHelper = new StimulusControllerTagHelper
        {
            Controller = ""
        };
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
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "data-controller");
    }

    [Fact]
    public void StimulusControllerTagHelper_TrimsWhitespace()
    {
        // Arrange
        var tagHelper = new StimulusControllerTagHelper
        {
            Controller = "  dropdown  "
        };
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
        Assert.Equal("dropdown", output.Attributes["data-controller"].Value);
    }
}
