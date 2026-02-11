using Microsoft.AspNetCore.Mvc;

namespace WireStimulus.Controllers;

public class CounterController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
