using Microsoft.AspNetCore.Mvc;

namespace WireStimulus.Controllers;

public class SlideshowController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
