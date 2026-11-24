using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;
using OrderDesk.Models;
using OrderDesk.Support;

namespace OrderDesk.Controllers;

public class CheckoutController(AppDbContext db, CurrentUser currentUser) : AppController
{
    [HttpPost("/checkout")]
    public async Task<IActionResult> Store(Dictionary<int, int>? prices)
    {
        var cart = HttpContext.Session.GetCart();

        if (cart.Count == 0)
        {
            TempData["Errors"] = new[] { "Your cart is empty." };

            return RedirectToAction("Index", "Cart");
        }

        prices ??= new Dictionary<int, int>();
        var productIds = cart.Keys.ToList();
        var products = await db.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);

        await using var transaction = await db.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;
        var order = new Order
        {
            UserId = (await currentUser.GetAsync()).Id,
            Status = OrderStatus.Pending,
            Total = 0,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var total = 0;

        foreach (var (productId, quantity) in cart)
        {
            if (!products.TryGetValue(productId, out var product))
            {
                continue;
            }

            var unitPrice = prices.TryGetValue(productId, out var postedPrice) ? postedPrice : product.Price;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = unitPrice,
                Quantity = quantity,
            });

            product.Stock -= 1;

            total += unitPrice * quantity;
        }

        order.Total = total;
        await db.SaveChangesAsync();

        await transaction.CommitAsync();

        HttpContext.Session.ClearCart();

        Flash($"Order #{order.Id} placed.");

        return RedirectToAction("Show", "Orders", new { id = order.Id });
    }
}
