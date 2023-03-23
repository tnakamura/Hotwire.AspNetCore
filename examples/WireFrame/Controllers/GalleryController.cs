using Microsoft.AspNetCore.Mvc;

namespace WireFrame.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Forest()
        {
            return View();
        }

        public IActionResult Mountains()
        {
            return View();
        }

        public IActionResult Ocean()
        {
            return View();
        }
    }
}
