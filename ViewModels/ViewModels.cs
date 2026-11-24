using OrderDesk.Models;

namespace OrderDesk.ViewModels;

public record LayoutData(User CurrentUser, List<User> SwitchableUsers, int CartCount);

public record CartLine(Product Product, int Quantity, int LineTotal);

public class CartViewModel
{
    public List<CartLine> Lines { get; init; } = new();

    public int Total { get; init; }
}

public record OrderListItem(int Id, DateTime CreatedAt, int ItemsCount, int Total, string Status);

public class OrdersIndexViewModel
{
    public List<OrderListItem> Orders { get; init; } = new();

    public string[] Statuses { get; init; } = [];

    public string? SelectedStatus { get; init; }
}
