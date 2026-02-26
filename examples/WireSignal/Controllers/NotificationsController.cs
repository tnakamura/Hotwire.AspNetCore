using Microsoft.AspNetCore.Mvc;
using Turbo.AspNetCore;
using WireSignal.Models;

namespace WireSignal.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ITurboStreamBroadcaster _broadcaster;
        private static int _notificationId = 1;

        public NotificationsController(ITurboStreamBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string title, string message, string type = "info")
        {
            var notification = new Notification
            {
                Id = _notificationId++,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.Now
            };

            // Broadcast to all subscribers of the "notifications" channel
            await _broadcaster.BroadcastViewAsync("notifications", "_Notification", notification);

            if (Request.IsTurboRequest())
            {
                return NoContent();
            }

            return RedirectToAction("Index");
        }
    }
}
