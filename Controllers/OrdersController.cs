using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;
using OrderDesk.Models;
using OrderDesk.Support;
using OrderDesk.ViewModels;

namespace OrderDesk.Controllers;

public class OrdersController(AppDbContext db, CurrentUser currentUser) : AppController
{
    [HttpGet("/orders")]
    public async Task<IActionResult> Index()
    {
        string? status = Request.Query["order_status"];
        var user = await currentUser.GetAsync();

        var query = db.Orders.Where(order => order.UserId == user.Id);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(order => order.Status == status);
        }

        var orders = await query
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => new OrderListItem(order.Id, order.CreatedAt, order.Items.Count, order.Total, order.Status))
            .ToListAsync();

        return View(new OrdersIndexViewModel
        {
            Orders = orders,
            Statuses = OrderStatus.All,
            SelectedStatus = Request.Query["status"],
        });
    }

    [HttpGet("/orders/{id:int}")]
    public async Task<IActionResult> Show(int id)
    {
        var order = await db.Orders
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost("/orders/{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await db.Orders.FindAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        if (!order.IsPending)
        {
            return BackWithErrors("Only pending orders can be cancelled.");
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        Flash($"Order #{order.Id} cancelled.");

        return RedirectToAction(nameof(Show), new { id = order.Id });
    }
}
