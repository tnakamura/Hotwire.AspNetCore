using TagKit;

namespace WireStream.Models;

public sealed class BlogArticle
{
    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public IReadOnlyList<Tag> Tags { get; private set; } = [];

    public void SetTags(string? tagsInput)
    {
        Tags = TagParser.Parse(tagsInput);
    }
}
