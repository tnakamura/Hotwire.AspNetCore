using Microsoft.AspNetCore.Mvc;
using WireDrive.Models;

namespace WireDrive.Controllers;

public class ProductsController : Controller
{
    private static readonly List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Turbo Drive Pro", Description = "高速なページ遷移を実現", Price = 29.99m, ImageUrl = "https://via.placeholder.com/300x200?text=Turbo+Drive+Pro" },
        new Product { Id = 2, Name = "Turbo Frames Starter", Description = "部分的なページ更新に最適", Price = 19.99m, ImageUrl = "https://via.placeholder.com/300x200?text=Turbo+Frames" },
        new Product { Id = 3, Name = "Turbo Streams Premium", Description = "リアルタイム更新を実現", Price = 39.99m, ImageUrl = "https://via.placeholder.com/300x200?text=Turbo+Streams" },
        new Product { Id = 4, Name = "Hotwire Complete", Description = "すべての機能を含むバンドル", Price = 79.99m, ImageUrl = "https://via.placeholder.com/300x200?text=Hotwire+Complete" },
    };

    public IActionResult Index()
    {
        return View(Products);
    }

    public IActionResult Details(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
}
