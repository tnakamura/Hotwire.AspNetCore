using Microsoft.AspNetCore.Mvc;

namespace WireStimulus.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
