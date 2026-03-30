using Microsoft.AspNetCore.Mvc;

namespace Turbo.AspNetCore.Test;

public class TurboControllerExtensionsTest
{
    [Fact]
    public void TurboStream_WithoutParameters_SetsTurboStreamContentType()
    {
        var controller = new TestController();

        var result = controller.TurboStream();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("text/vnd.turbo-stream.html; charset=utf-8", viewResult.ContentType);
        Assert.Null(viewResult.Model);
    }

    [Fact]
    public void TurboStream_WithModel_SetsTurboStreamContentTypeAndModel()
    {
        var controller = new TestController();
        var model = new { Name = "Alice" };

        var result = controller.TurboStream(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("text/vnd.turbo-stream.html; charset=utf-8", viewResult.ContentType);
        Assert.Same(model, viewResult.Model);
    }

    [Fact]
    public void TurboStream_WithViewName_SetsTurboStreamContentTypeAndViewName()
    {
        var controller = new TestController();

        var result = controller.TurboStream("_TurboStream");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("text/vnd.turbo-stream.html; charset=utf-8", viewResult.ContentType);
        Assert.Equal("_TurboStream", viewResult.ViewName);
    }

    [Fact]
    public void TurboStream_WithViewNameAndModel_SetsTurboStreamContentTypeViewNameAndModel()
    {
        var controller = new TestController();
        var model = new { Id = 123 };

        var result = controller.TurboStream("_TurboStream", model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("text/vnd.turbo-stream.html; charset=utf-8", viewResult.ContentType);
        Assert.Equal("_TurboStream", viewResult.ViewName);
        Assert.Same(model, viewResult.Model);
    }

    private sealed class TestController : Controller;
}
