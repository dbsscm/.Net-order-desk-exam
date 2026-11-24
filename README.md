# Order Desk

A small ASP.NET Core ordering app: browse products, manage a cart, place orders, and view order history.

Products are Century Pacific Food canned goods, used as sample data. Prices, pack sizes, and stock levels are illustrative only.

Read **EXAM_BRIEF.md** first for your task and the exam rules.

## Requirements

- .NET 10 SDK
- Git
- VS Code (Visual Studio also works if your version supports .NET 10)

The app uses ASP.NET Core MVC and Entity Framework Core with SQLite. NuGet packages download on the first build. If you ran the environment check app before the exam, they are already on your machine. The database is created with sample data the first time the app starts.

## Start the app

1. Open this folder in VS Code.
2. Open a terminal (**Terminal > New Terminal**) and run:

   ```
   dotnet run
   ```

3. Open http://127.0.0.1:8000 in your browser.

Stop the app with `Ctrl+C`. After you change C# code or a view, stop the app and run `dotnet run` again. Your cart and the selected user are kept between runs.

## Switching users

There is no login screen. Use the **Signed in as** dropdown in the top bar to change the current user. Whoever is selected there is treated as the logged-in user.

## Reset the sample data

To restore the database to its original state, stop the app and run:

```
dotnet run -- --reset-db
```

## Using SQL Server (optional)

The app uses SQLite by default. It uses only Entity Framework Core, so it also runs on SQL Server.

1. In `appsettings.json`, set `Database:Provider` to `SqlServer` and set `ConnectionStrings:Default` to your SQL Server connection string.
2. Run `dotnet run -- --reset-db`. This drops and recreates the database named in the connection string, so point it at an empty database.

Time spent switching databases counts toward your exam time. SQLite is recommended.

## Troubleshooting

| Problem | Fix |
|---|---|
| `dotnet` is not recognized | The .NET SDK isn't installed or isn't on your PATH. Install the .NET 10 SDK and restart VS Code. |
| `Unable to load the service index for source` | NuGet packages couldn't download. Check your internet connection and run `dotnet run` again. |
| `address already in use` | Another app is using port 8000. Run `dotnet run --urls http://127.0.0.1:8080` and open http://127.0.0.1:8080. |
| Pages don't show your change | Stop the app with `Ctrl+C` and run `dotnet run` again. |
