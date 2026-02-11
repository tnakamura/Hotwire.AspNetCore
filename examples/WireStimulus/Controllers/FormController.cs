using Microsoft.AspNetCore.Mvc;

namespace WireStimulus.Controllers;

public class FormController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
