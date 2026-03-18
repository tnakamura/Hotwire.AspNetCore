using TagKit;

namespace Turbo.AspNetCore.Test;

public class TagParserTest
{
    [Fact]
    public void Parse_WhenInputIsEmpty_ReturnsEmptyCollection()
    {
        var tags = TagParser.Parse(" ");

        Assert.Empty(tags);
    }

    [Fact]
    public void Parse_WhenInputContainsDuplicateTags_ReturnsDistinctTags()
    {
        var tags = TagParser.Parse("dotnet, hotwire, DotNet");

        Assert.Equal(["dotnet", "hotwire"], tags.Select(tag => tag.Name));
    }
}
