using Microsoft.AspNetCore.Mvc;
using Turbo.AspNetCore;
using WireSignal.Models;

namespace WireSignal.Controllers
{
    public class ChatController : Controller
    {
        private readonly ITurboStreamBroadcaster _broadcaster;
        private static int _messageId = 1;
        private static List<ChatMessage> _messages = new();

        public ChatController(ITurboStreamBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }

        public IActionResult Index()
        {
            ViewBag.Messages = _messages;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string username, string message)
        {
            var chatMessage = new ChatMessage
            {
                Id = _messageId++,
                Username = username,
                Message = message,
                Timestamp = DateTime.Now
            };

            _messages.Add(chatMessage);

            // Broadcast to all subscribers of the "chat" channel
            await _broadcaster.BroadcastViewAsync("chat", "_ChatMessage", chatMessage);

            if (Request.IsTurboRequest())
            {
                return NoContent();
            }

            return RedirectToAction("Index");
        }
    }
}
