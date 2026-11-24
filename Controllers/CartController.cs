using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;
using OrderDesk.Support;
using OrderDesk.ViewModels;

namespace OrderDesk.Controllers;

public class CartController(AppDbContext db) : AppController
{
    [HttpGet("/cart")]
    public async Task<IActionResult> Index()
    {
        var cart = HttpContext.Session.GetCart();
        var productIds = cart.Keys.ToList();
        var products = await db.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);

        var lines = new List<CartLine>();
        var total = 0;

        foreach (var (productId, quantity) in cart)
        {
            if (!products.TryGetValue(productId, out var product))
            {
                continue;
            }

            lines.Add(new CartLine(product, quantity, product.Price * quantity));

            total += product.Price;
        }

        return View(new CartViewModel { Lines = lines, Total = total });
    }

    [HttpPost("/cart")]
    public async Task<IActionResult> Add(AddToCartInput input)
    {
        if (!ModelState.IsValid)
        {
            return BackWithErrors();
        }

        var product = await db.Products.FindAsync(input.ProductId);

        if (product is null)
        {
            return BackWithErrors("The selected product is invalid.");
        }

        var cart = HttpContext.Session.GetCart();
        cart[product.Id] = cart.GetValueOrDefault(product.Id) + input.Quantity!.Value;
        HttpContext.Session.SetCart(cart);

        Flash($"{product.Name} added to cart.");

        return Back();
    }

    [HttpPost("/cart/{productId:int}/update")]
    public async Task<IActionResult> Update(int productId, UpdateCartInput input)
    {
        var product = await db.Products.FindAsync(productId);

        if (product is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BackWithErrors();
        }

        var cart = HttpContext.Session.GetCart();

        if (cart.ContainsKey(product.Id))
        {
            cart[product.Id] = input.Quantity!.Value;
        }

        Flash("Quantity updated.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("/cart/{productId:int}/remove")]
    public async Task<IActionResult> Remove(int productId)
    {
        var product = await db.Products.FindAsync(productId);

        if (product is null)
        {
            return NotFound();
        }

        var cart = HttpContext.Session.GetCart();
        cart.Remove(product.Id);
        HttpContext.Session.SetCart(cart);

        Flash($"{product.Name} removed from cart.");

        return RedirectToAction(nameof(Index));
    }
}

public class AddToCartInput
{
    [Required]
    public int? ProductId { get; set; }

    [Required]
    public int? Quantity { get; set; }
}

public class UpdateCartInput
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The quantity must be at least 1.")]
    public int? Quantity { get; set; }
}
