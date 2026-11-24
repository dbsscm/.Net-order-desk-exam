using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;

namespace OrderDesk.Controllers;

public class ProductsController(AppDbContext db) : AppController
{
    [HttpGet("/products")]
    public async Task<IActionResult> Index()
    {
        var products = await db.Products.OrderBy(product => product.Name).ToListAsync();

        return View(products);
    }
}
