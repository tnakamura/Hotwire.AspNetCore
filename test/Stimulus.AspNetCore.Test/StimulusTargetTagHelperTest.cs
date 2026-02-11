using Microsoft.AspNetCore.Razor.TagHelpers;
using Stimulus.AspNetCore.TagHelpers;

namespace Stimulus.AspNetCore.Test;

public class StimulusTargetTagHelperTest
{
    [Fact]
    public void StimulusTargetTagHelper_SetsDataTarget()
    {
        // Arrange
        var tagHelper = new StimulusTargetTagHelper
        {
            Target = "dropdown.menu"
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
        Assert.Equal("menu", output.Attributes["data-dropdown-target"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-target");
    }

    [Fact]
    public void StimulusTargetTagHelper_SupportsMultipleTargetsSameController()
    {
        // Arrange
        var tagHelper = new StimulusTargetTagHelper
        {
            Target = "form.name form.email"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "input",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("name email", output.Attributes["data-form-target"].Value);
    }

    [Fact]
    public void StimulusTargetTagHelper_SupportsDifferentControllers()
    {
        // Arrange
        var tagHelper = new StimulusTargetTagHelper
        {
            Target = "dropdown.item list.item"
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
        Assert.Equal("item", output.Attributes["data-dropdown-target"].Value);
        Assert.Equal("item", output.Attributes["data-list-target"].Value);
    }

    [Fact]
    public void StimulusTargetTagHelper_MergesWithExistingTarget()
    {
        // Arrange
        var tagHelper = new StimulusTargetTagHelper
        {
            Target = "form.email"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "input",
            new TagHelperAttributeList
            {
                new TagHelperAttribute("data-form-target", "name")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("name email", output.Attributes["data-form-target"].Value);
    }

    [Fact]
    public void StimulusTargetTagHelper_HandlesEmptyTarget()
    {
        // Arrange
        var tagHelper = new StimulusTargetTagHelper
        {
            Target = ""
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
        // Should have removed stimulus-target but not added any data-*-target
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-target");
    }
}
