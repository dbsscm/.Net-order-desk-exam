using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;
using OrderDesk.Support;
using OrderDesk.ViewModels;

namespace OrderDesk.Filters;

/// <summary>
/// Gives the layout the current user, the user switcher list, and the cart count.
/// </summary>
public class ShareLayoutData(CurrentUser currentUser, AppDbContext db) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is Controller controller)
        {
            controller.ViewData["Layout"] = new LayoutData(
                await currentUser.GetAsync(),
                await db.Users.OrderBy(user => user.Name).ToListAsync(),
                context.HttpContext.Session.GetCart().Values.Sum());
        }

        await next();
    }
}
