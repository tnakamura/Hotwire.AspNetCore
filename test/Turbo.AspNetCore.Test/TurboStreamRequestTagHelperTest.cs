using Microsoft.AspNetCore.Razor.TagHelpers;
using Turbo.AspNetCore.TagHelpers;

namespace Turbo.AspNetCore.Test;

public class TurboStreamRequestTagHelperTest
{
    [Fact]
    public void Form_WithTurboStream_ConvertsToDataTurboStream()
    {
        // Arrange
        var tagHelper = new TurboStreamRequestTagHelper
        {
            TurboStream = true
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "form",
            new TagHelperAttributeList { { "turbo-stream", "" } },
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("", output.Attributes["data-turbo-stream"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "turbo-stream");
    }

    [Fact]
    public void Anchor_WithTurboStream_ConvertsToDataTurboStream()
    {
        // Arrange
        var tagHelper = new TurboStreamRequestTagHelper
        {
            TurboStream = true
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList { { "turbo-stream", "" } },
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("", output.Attributes["data-turbo-stream"].Value);
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "turbo-stream");
    }

    [Fact]
    public void TurboStreamFalse_DoesNotAddDataTurboStream()
    {
        // Arrange
        var tagHelper = new TurboStreamRequestTagHelper
        {
            TurboStream = false
        };
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput(
            "form",
            new TagHelperAttributeList { { "turbo-stream", "false" } },
            (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "data-turbo-stream");
        Assert.DoesNotContain(output.Attributes, attr => attr.Name == "turbo-stream");
    }
}
