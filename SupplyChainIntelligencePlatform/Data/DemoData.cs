using Microsoft.EntityFrameworkCore;
using SupplyChainIntelligencePlatform.Models;

namespace SupplyChainIntelligencePlatform.Data;

public static class DemoData
{
    public static async Task SeedAsync(SupplyChainDbContext db)
    {
        if (await db.Shipments.AnyAsync())
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var supplier = new Supplier
        {
            Name = "Shenzhen Motion Systems",
            Country = "China",
            RiskScore = 74
        };

        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();

        var shipment = new Shipment
        {
            TrackingNumber = "DHL-7842-991",
            ComponentCode = "MTR-440",
            ComponentName = "Servo motor assembly",
            Origin = "Shenzhen, CN",
            Destination = "Munich, DE",
            ExpectedArrival = now.AddDays(-1),
            RevisedArrival = now.AddDays(4),
            Status = "Delayed",
            Quantity = 120,
            SupplierId = supplier.Id,
            FreightCost = 1850
        };

        db.Shipments.Add(shipment);
        await db.SaveChangesAsync();

        db.InventoryItems.AddRange(
            new InventoryItem
            {
                ComponentCode = "MTR-440",
                ComponentName = "Servo motor assembly",
                Warehouse = "Munich Central",
                AvailableQuantity = 36,
                SafetyStock = 60,
                UnitCost = 420
            },
            new InventoryItem
            {
                ComponentCode = "SNS-210",
                ComponentName = "Pressure sensor",
                Warehouse = "Augsburg",
                AvailableQuantity = 220,
                SafetyStock = 100,
                UnitCost = 48
            });

        db.ProductionOrders.AddRange(
            new ProductionOrder
            {
                OrderNumber = "MO-2026-1042",
                ProductName = "Industrial pump X4",
                PlannedStart = now.AddDays(1),
                UnitsPlanned = 40,
                RevenueAtRisk = 68000,
                ShipmentId = shipment.Id
            },
            new ProductionOrder
            {
                OrderNumber = "MO-2026-1051",
                ProductName = "Hydraulic actuator H2",
                PlannedStart = now.AddDays(3),
                UnitsPlanned = 20,
                RevenueAtRisk = 42000,
                ShipmentId = shipment.Id
            });

        db.Recommendations.Add(new Recommendation
        {
            ShipmentId = shipment.Id,
            Action = "Expedite a partial shipment",
            Rationale = "Stock is below safety stock and two production orders depend on this component. Validate supplier capacity and compare expedited freight with production-delay exposure.",
            EstimatedCost = 6400,
            AvoidedLoss = 28500
        });

        await db.SaveChangesAsync();
    }
}
