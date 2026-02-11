using Microsoft.AspNetCore.Mvc;
using WireDrive.Models;

namespace WireDrive.Controllers;

public class OrdersController : Controller
{
    public IActionResult New(int? productId)
    {
        var model = new OrderViewModel
        {
            ProductId = productId
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(OrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("New", model);
        }

        // In a real application, you would save the order to a database here
        TempData["OrderId"] = Random.Shared.Next(1000, 9999);
        TempData["CustomerName"] = model.CustomerName;
        TempData["Email"] = model.Email;
        
        return RedirectToAction("Confirmation");
    }

    public IActionResult Confirmation()
    {
        if (TempData["OrderId"] == null)
        {
            return RedirectToAction("New");
        }

        var model = new OrderConfirmationViewModel
        {
            OrderId = (int)TempData["OrderId"]!,
            CustomerName = TempData["CustomerName"]?.ToString() ?? "",
            Email = TempData["Email"]?.ToString() ?? ""
        };

        return View(model);
    }
}
