using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OrderDesk.Data;
using OrderDesk.Filters;
using OrderDesk.Support;

// "dotnet run -- --reset-db" recreates the database with the sample data, then exits.
var resetDatabase = args.Contains("--reset-db");

var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--reset-db").ToArray());

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add<ShareLayoutData>();
});

builder.Services.AddAppDatabase(builder.Configuration, builder.Environment.ContentRootPath);

builder.Services.AddSingleton<IDistributedCache>(
    new FileSessionCache(Path.Combine(builder.Environment.ContentRootPath, "storage", "sessions")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.Name = "OrderDesk.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>();

var app = builder.Build();

await DbSeeder.InitializeAsync(app.Services, resetDatabase);

if (resetDatabase)
{
    Console.WriteLine("Database reset with sample data.");
    return;
}

app.UseSession();

app.MapGet("/", () => Results.Redirect("/products"));
app.MapControllers();

app.Run();
