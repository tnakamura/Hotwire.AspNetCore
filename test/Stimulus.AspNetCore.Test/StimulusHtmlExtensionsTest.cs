using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;

namespace Stimulus.AspNetCore.Test;

public class StimulusHtmlExtensionsTest
{
    private static readonly IHtmlHelper Html = Substitute.For<IHtmlHelper>();

    [Fact]
    public void StimulusController_SingleController_ReturnsDataController()
    {
        var attributes = Html.StimulusController("dropdown");

        Assert.Equal("dropdown", attributes["data-controller"]);
    }

    [Fact]
    public void StimulusController_MultipleControllers_JoinsWithSpace()
    {
        var attributes = Html.StimulusController("dropdown", "modal");

        Assert.Equal("dropdown modal", attributes["data-controller"]);
    }

    [Fact]
    public void StimulusAction_SingleAction_ReturnsDataAction()
    {
        var attributes = Html.StimulusAction("click->dropdown#toggle");

        Assert.Equal("click->dropdown#toggle", attributes["data-action"]);
    }

    [Fact]
    public void StimulusAction_MultipleActions_JoinsWithSpace()
    {
        var attributes = Html.StimulusAction("click->dropdown#toggle", "keydown->dropdown#close");

        Assert.Equal("click->dropdown#toggle keydown->dropdown#close", attributes["data-action"]);
    }

    [Fact]
    public void StimulusTarget_SingleTarget_ReturnsControllerTargetAttribute()
    {
        var attributes = Html.StimulusTarget("dropdown", "menu");

        Assert.Equal("menu", attributes["data-dropdown-target"]);
    }

    [Fact]
    public void StimulusTarget_MultipleTargets_JoinsWithSpace()
    {
        var attributes = Html.StimulusTarget("dropdown", "menu", "button");

        Assert.Equal("menu button", attributes["data-dropdown-target"]);
    }

    [Fact]
    public void StimulusValue_WithValue_ConvertsToString()
    {
        var attributes = Html.StimulusValue("counter", "count", 42);

        Assert.Equal("42", attributes["data-counter-count-value"]);
    }

    [Fact]
    public void StimulusValue_WithNullValue_UsesEmptyString()
    {
        var attributes = Html.StimulusValue("counter", "count", null!);

        Assert.Equal(string.Empty, attributes["data-counter-count-value"]);
    }

    [Fact]
    public void StimulusClass_ReturnsControllerClassAttribute()
    {
        var attributes = Html.StimulusClass("dropdown", "visible", "is-visible");

        Assert.Equal("is-visible", attributes["data-dropdown-visible-class"]);
    }

    [Fact]
    public void StimulusAttributes_MergesAndAppendsDuplicateKeys()
    {
        var first = Html.StimulusAction("click->dropdown#toggle");
        var second = Html.StimulusAction("keydown->dropdown#close");
        var third = Html.StimulusController("dropdown");

        var attributes = Html.StimulusAttributes(first, second, third);

        Assert.Equal("click->dropdown#toggle keydown->dropdown#close", attributes["data-action"]);
        Assert.Equal("dropdown", attributes["data-controller"]);
    }
}
