using Microsoft.AspNetCore.Mvc;
using Turbo.AspNetCore;

namespace WireStream.Controllers
{
    public class CustomActionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetTitle(string title)
        {
            return this.TurboStream("SetTitle", new { title });
        }

        [HttpPost]
        public IActionResult ShowNotification(string message, string type = "info")
        {
            return this.TurboStream("ShowNotification", new { message, type });
        }

        [HttpPost]
        public IActionResult AddItemWithAnimation(string itemText)
        {
            var model = new
            {
                itemText,
                timestamp = DateTime.Now.ToString("HH:mm:ss")
            };
            return this.TurboStream("AddItemWithAnimation", model);
        }

        [HttpPost]
        public IActionResult HighlightItem(string itemId)
        {
            return this.TurboStream("HighlightItem", new { itemId });
        }

        [HttpPost]
        public IActionResult DebugLog(string message)
        {
            return this.TurboStream("DebugLog", new { message });
        }
    }
}
