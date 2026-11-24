namespace OrderDesk.Models;

public class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = "";

    public string Name { get; set; } = "";

    public int Price { get; set; }

    public int Stock { get; set; }
}
