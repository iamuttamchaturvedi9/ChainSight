using Microsoft.EntityFrameworkCore;
using SupplyChainIntelligencePlatform.Models;

namespace SupplyChainIntelligencePlatform.Data;

public class SupplyChainDbContext(DbContextOptions<SupplyChainDbContext> options) : DbContext(options)
{
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>()
            .HasOne(shipment => shipment.Supplier)
            .WithMany()
            .HasForeignKey(shipment => shipment.SupplierId);

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(item => item.ComponentCode);
    }
}
