using Microsoft.EntityFrameworkCore;
using OrderDesk.Data;
using OrderDesk.Models;

namespace OrderDesk.Support;

/// <summary>
/// Exam convenience only: the "logged in" user is whoever is selected
/// in the user switcher. There is no real authentication in this app.
/// </summary>
public class CurrentUser(AppDbContext db, IHttpContextAccessor httpContextAccessor)
{
    public const string SessionKey = "current_user_id";

    private User? _user;

    public async Task<User> GetAsync()
    {
        if (_user is null)
        {
            var session = httpContextAccessor.HttpContext!.Session;
            var id = session.GetInt32(SessionKey);

            _user = (id is int userId ? await db.Users.FindAsync(userId) : null)
                ?? await db.Users.OrderBy(user => user.Id).FirstAsync();

            session.SetInt32(SessionKey, _user.Id);
        }

        return _user;
    }
}
