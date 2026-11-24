using Microsoft.AspNetCore.Mvc;

namespace OrderDesk.Controllers;

public abstract class AppController : Controller
{
    /// <summary>
    /// Shows a success message on the next page.
    /// </summary>
    protected void Flash(string message) => TempData["Status"] = message;

    /// <summary>
    /// Redirects to the page the request came from, or to the products page.
    /// </summary>
    protected IActionResult Back()
    {
        var referer = Request.Headers.Referer.ToString();

        if (Uri.TryCreate(referer, UriKind.Absolute, out var uri)
            && uri.Host == Request.Host.Host
            && uri.Port == (Request.Host.Port ?? uri.Port))
        {
            return Redirect(uri.PathAndQuery);
        }

        return Redirect("/products");
    }

    /// <summary>
    /// Redirects back and shows the given errors, or the model validation errors if none are given.
    /// </summary>
    protected IActionResult BackWithErrors(params string[] errors)
    {
        TempData["Errors"] = errors.Length > 0
            ? errors
            : ModelState.Values.SelectMany(entry => entry.Errors).Select(error => error.ErrorMessage).ToArray();

        return Back();
    }
}
