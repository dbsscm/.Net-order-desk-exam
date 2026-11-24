using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;
using OrderDesk.Support;

namespace OrderDesk.Controllers;

/// <summary>
/// Exam convenience only. Switching users stands in for logging in
/// and is out of scope for the exam.
/// </summary>
public class UserSwitchController(AppDbContext db) : AppController
{
    [HttpPost("/switch-user")]
    public async Task<IActionResult> Switch(int? userId)
    {
        if (userId is null || !await db.Users.AnyAsync(user => user.Id == userId))
        {
            return BackWithErrors("The selected user is invalid.");
        }

        HttpContext.Session.SetInt32(CurrentUser.SessionKey, userId.Value);
        HttpContext.Session.ClearCart();

        return RedirectToAction("Index", "Products");
    }
}
