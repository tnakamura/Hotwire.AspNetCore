using Microsoft.AspNetCore.Razor.TagHelpers;
using Stimulus.AspNetCore.TagHelpers;

namespace Stimulus.AspNetCore.Test;

public class StimulusActionTagHelperTest
{
    [Fact]
    public void StimulusActionTagHelper_SetsDataAction()
    {
        // Arrange
        var tagHelper = new StimulusActionTagHelper
        {
            Action = "click->dropdown#toggle"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "button",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("click->dropdown#toggle", output.Attributes["data-action"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "stimulus-action");
    }

    [Fact]
    public void StimulusActionTagHelper_MergesWithExistingDataAction()
    {
        // Arrange
        var tagHelper = new StimulusActionTagHelper
        {
            Action = "click->dropdown#toggle"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "button",
            new TagHelperAttributeList
            {
                new TagHelperAttribute("data-action", "mouseenter->dropdown#show")
            },
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("mouseenter->dropdown#show click->dropdown#toggle", 
            output.Attributes["data-action"].Value);
    }

    [Fact]
    public void StimulusActionTagHelper_SupportsMultipleActions()
    {
        // Arrange
        var tagHelper = new StimulusActionTagHelper
        {
            Action = "click->dropdown#toggle mouseenter->dropdown#show"
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "button",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("click->dropdown#toggle mouseenter->dropdown#show", 
            output.Attributes["data-action"].Value);
    }

    [Fact]
    public void StimulusActionTagHelper_HandlesEmptyAction()
    {
        // Arrange
        var tagHelper = new StimulusActionTagHelper
        {
            Action = ""
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "button",
            new TagHelperAttributeList(),
            (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "data-action");
    }
}
