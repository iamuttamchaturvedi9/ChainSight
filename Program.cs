using Microsoft.EntityFrameworkCore;
using SupplyChainIntelligencePlatform.Data;
using SupplyChainIntelligencePlatform.Models;
using SupplyChainIntelligencePlatform.Services;

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

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
