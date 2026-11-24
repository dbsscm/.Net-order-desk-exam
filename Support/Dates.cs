using System.Globalization;

namespace OrderDesk.Support;

public static class Dates
{
    public static string Format(DateTime value) =>
        value.ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture);
}
