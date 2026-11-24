using System.Globalization;

namespace OrderDesk.Support;

public static class Money
{
    /// <summary>
    /// Prices are stored as whole cents to avoid rounding differences
    /// between database engines.
    /// </summary>
    public static string Format(int cents) =>
        "₱" + (cents / 100m).ToString("N2", CultureInfo.InvariantCulture);
}
