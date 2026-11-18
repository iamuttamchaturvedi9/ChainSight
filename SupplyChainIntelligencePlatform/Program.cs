using Microsoft.EntityFrameworkCore;
using SupplyChainIntelligencePlatform.Data;
using SupplyChainIntelligencePlatform.Models;
using SupplyChainIntelligencePlatform.Services;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<SupplyChainDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("SupplyChain") ?? "Data Source=chainsight.db";

    if (cs.StartsWith("Host=", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(cs);
    }
    else
    {
        options.UseSqlite(cs);
    }
});
builder.Services.AddScoped<SupplyChainAnalyticsService>();
builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.SessionMode = HttpServerSessionMode.Stateless;
    })
    .WithTools<ShipmentTools>();

builder.Services.AddScoped<ShipmentService>();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupplyChainDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DemoData.SeedAsync(db);
}

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/api/dashboard", async (SupplyChainAnalyticsService service) =>
    Results.Ok(await service.Dashboard()));
app.MapGet("/api/shipments", async (SupplyChainDbContext db) =>
    Results.Ok(await db.Shipments.Include(shipment => shipment.Supplier).ToListAsync()));
app.MapGet("/api/shipments/{id:int}/impact", async (int id, SupplyChainAnalyticsService service) =>
{
    var result = await service.Impact(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
});



app.MapMcp("/mcp");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
