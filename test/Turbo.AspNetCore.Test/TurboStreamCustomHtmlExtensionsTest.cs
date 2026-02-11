using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using System.IO;
using System.Text.Encodings.Web;

namespace Turbo.AspNetCore.Test;

public class TurboStreamCustomHtmlExtensionsTest
{
    private static string RenderHtml(IHtmlContent content)
    {
        using var writer = new StringWriter();
        content.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }

    [Fact]
    public void TurboStreamCustom_WithActionOnly_GeneratesCorrectHtml()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act
        var result = htmlHelper.TurboStreamCustom("set_title", new { title = "New Title" });
        var html = RenderHtml(result);

        // Assert
        Assert.Contains("<turbo-stream", html);
        Assert.Contains("action=\"set_title\"", html);
        Assert.Contains("title=\"New Title\"", html);
        Assert.Contains("<template>", html);
        Assert.Contains("</template>", html);
    }

    [Fact]
    public void TurboStreamCustom_WithContent_GeneratesCorrectHtml()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act
        var result = htmlHelper.TurboStreamCustom(
            "notify",
            new { message = "Success!" },
            _ => new HtmlString("<div class=\"alert\">Notification</div>"));
        var html = RenderHtml(result);

        // Assert
        Assert.Contains("<turbo-stream", html);
        Assert.Contains("action=\"notify\"", html);
        Assert.Contains("message=\"Success!\"", html);
        Assert.Contains("<template>", html);
        Assert.Contains("<div class=\"alert\">Notification</div>", html);
        Assert.Contains("</template>", html);
    }

    [Fact]
    public void TurboStreamCustom_WithNullAction_ThrowsException()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            htmlHelper.TurboStreamCustom(null, null));
    }

    [Fact]
    public void TurboStreamCustom_WithEmptyAction_ThrowsException()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            htmlHelper.TurboStreamCustom("", null));
    }

    [Fact]
    public void TurboStreamCustom_WithMultipleAttributes_PassesThroughAll()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act
        var result = htmlHelper.TurboStreamCustom("custom", new
        {
            target = "element",
            data_value = "123",
            data_config = "advanced"
        });
        var html = RenderHtml(result);

        // Assert
        Assert.Contains("action=\"custom\"", html);
        Assert.Contains("target=\"element\"", html);
        Assert.Contains("data-value=\"123\"", html);
        Assert.Contains("data-config=\"advanced\"", html);
    }

    [Fact]
    public void TurboStreamCustom_WithNullAttributes_GeneratesOnlyAction()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act
        var result = htmlHelper.TurboStreamCustom("my_action", null);
        var html = RenderHtml(result);

        // Assert
        Assert.Contains("action=\"my_action\"", html);
        Assert.Contains("<template>", html);
        Assert.Contains("</template>", html);
    }

    [Fact]
    public void TurboStreamCustom_WithNullContent_GeneratesEmptyTemplate()
    {
        // Arrange
        var htmlHelper = Substitute.For<IHtmlHelper>();

        // Act
        var result = htmlHelper.TurboStreamCustom("action", new { id = "test" }, content: null);
        var html = RenderHtml(result);

        // Assert
        Assert.Contains("action=\"action\"", html);
        Assert.Contains("id=\"test\"", html);
        Assert.Contains("<template></template>", html);
    }
}
