using Microsoft.AspNetCore.Mvc;

namespace WireStimulus.Controllers;

public class ClipboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
