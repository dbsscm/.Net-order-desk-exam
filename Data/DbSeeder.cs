using OrderDesk.Models;

namespace OrderDesk.Data;

public static class DbSeeder
{
    /// <summary>
    /// Creates the database and sample data on first run. With reset, drops it first.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, bool reset)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (reset)
        {
            await db.Database.EnsureDeletedAsync();
        }

        if (await db.Database.EnsureCreatedAsync())
        {
            await SeedAsync(db);
        }
    }

    public static async Task SeedAsync(AppDbContext db)
    {
        var maria = await AddAsync(db, new User { Name = "Maria Santos", Email = "maria@example.com" });
        var juan = await AddAsync(db, new User { Name = "Juan Dela Cruz", Email = "juan@example.com" });
        var ana = await AddAsync(db, new User { Name = "Ana Reyes", Email = "ana@example.com" });

        Product[] rows =
        [
            new() { Sku = "CTN-OIL155", Name = "Century Tuna Flakes in Oil 155g (case of 48)", Price = 206400, Stock = 40 },
            new() { Sku = "SRD-TOM155", Name = "555 Sardines in Tomato Sauce 155g (case of 100)", Price = 215000, Stock = 12 },
            new() { Sku = "ARG-CB175", Name = "Argentina Corned Beef 175g (case of 48)", Price = 288000, Stock = 200 },
            new() { Sku = "CTN-HOT180", Name = "Century Tuna Hot & Spicy 180g (case of 48)", Price = 235200, Stock = 3 },
            new() { Sku = "ARG-ML150", Name = "Argentina Meat Loaf 150g (case of 48)", Price = 124800, Stock = 60 },
            new() { Sku = "ARG-VS130", Name = "Argentina Vienna Sausage 130g (case of 48)", Price = 168000, Stock = 5 },
            new() { Sku = "TNA-AFR155", Name = "555 Tuna Afritada 155g (case of 48)", Price = 163200, Stock = 25 },
            new() { Sku = "SRD-HOT155", Name = "555 Sardines Hot 155g (case of 100)", Price = 225000, Stock = 150 },
            new() { Sku = "CTN-CAL155", Name = "Century Tuna Calamansi 155g (case of 48)", Price = 211200, Stock = 18 },
            new() { Sku = "TNA-MEC155", Name = "555 Tuna Mechado 155g (case of 48)", Price = 163200, Stock = 80 },
            new() { Sku = "ARG-BL150", Name = "Argentina Beef Loaf 150g (case of 48)", Price = 139200, Stock = 35 },
        ];

        var products = new Dictionary<string, Product>();

        foreach (var row in rows)
        {
            products[row.Sku] = await AddAsync(db, row);
        }

        await AddOrderAsync(db, maria, OrderStatus.Completed, 21, [("CTN-CAL155", 2), ("SRD-HOT155", 1)], products);
        await AddOrderAsync(db, ana, OrderStatus.Completed, 14, [("ARG-BL150", 3), ("ARG-CB175", 10)], products);
        await AddOrderAsync(db, maria, OrderStatus.Completed, 10, [("ARG-CB175", 5), ("ARG-ML150", 2)], products);
        await AddOrderAsync(db, juan, OrderStatus.Completed, 8, [("CTN-HOT180", 1)], products);
        await AddOrderAsync(db, maria, OrderStatus.Cancelled, 6, [("SRD-TOM155", 1)], products);
        await AddOrderAsync(db, juan, OrderStatus.Pending, 3, [("ARG-VS130", 1), ("TNA-MEC155", 4)], products);
        await AddOrderAsync(db, maria, OrderStatus.Pending, 1, [("TNA-AFR155", 2)], products);

        // This product was removed from the catalog after it had already been ordered.
        db.Products.Remove(products["CTN-CAL155"]);
        await db.SaveChangesAsync();
    }

    // Saves rows one at a time so the IDs follow the order above.
    private static async Task<T> AddAsync<T>(AppDbContext db, T entity) where T : class
    {
        db.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    private static async Task AddOrderAsync(
        AppDbContext db,
        User user,
        string status,
        int daysAgo,
        (string Sku, int Quantity)[] lines,
        Dictionary<string, Product> products)
    {
        var date = DateTime.UtcNow.Date.AddDays(-daysAgo).AddHours(9 + daysAgo % 8).AddMinutes(15);

        var order = new Order
        {
            UserId = user.Id,
            Status = status,
            Total = 0,
            CreatedAt = date,
            UpdatedAt = date,
        };

        foreach (var (sku, quantity) in lines)
        {
            var product = products[sku];

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity,
            });

            order.Total += product.Price * quantity;
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();
    }
}
