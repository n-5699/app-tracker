using AppRegistry.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Database: "Sqlite" for the local demo, "SqlServer" for Azure SQL (see appsettings*.json).
var provider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("AppRegistry");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Connection string 'AppRegistry' is not set. Add it in appsettings or the App Service configuration.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (provider == "Sqlite")
        options.UseSqlite(connectionString);
    else
        options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Create/update the database schema on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (provider == "Sqlite")
        db.Database.EnsureCreated();   // local demo: build tables straight from the models
    else
        db.Database.Migrate();         // Azure SQL: apply the migrations in /Migrations
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Show a friendly page (instead of a blank browser error) for 404 Not Found and similar responses.
app.UseStatusCodePagesWithReExecute("/status/{0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
