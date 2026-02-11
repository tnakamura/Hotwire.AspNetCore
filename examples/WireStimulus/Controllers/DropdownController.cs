using Microsoft.AspNetCore.Mvc;

namespace WireStimulus.Controllers;

public class DropdownController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
