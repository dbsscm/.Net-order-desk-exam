using System.Text.Json;

namespace OrderDesk.Support;

/// <summary>
/// The cart lives in the session as product ID => quantity.
/// </summary>
public static class CartSession
{
    private const string Key = "cart";

    public static Dictionary<int, int> GetCart(this ISession session)
    {
        var json = session.GetString(Key);

        return json is null
            ? new Dictionary<int, int>()
            : JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    public static void SetCart(this ISession session, Dictionary<int, int> cart) =>
        session.SetString(Key, JsonSerializer.Serialize(cart));

    public static void ClearCart(this ISession session) => session.Remove(Key);
}
