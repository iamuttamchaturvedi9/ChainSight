using Microsoft.EntityFrameworkCore;
using SupplyChainIntelligencePlatform.Data;

namespace SupplyChainIntelligencePlatform.Services;

public record DashboardDto(
    int ActiveShipments,
    int DelayedShipments,
    int AtRiskOrders,
    decimal RevenueAtRisk,
    decimal InventoryValue);

public record ImpactDto(
    int ShipmentId,
    string TrackingNumber,
    string Component,
    int AvailableStock,
    int SafetyStock,
    int Shortage,
    int DelayDays,
    int AffectedOrders,
    decimal RevenueAtRisk,
    decimal ExpediteCost,
    decimal PotentialAvoidedExposure,
    string Recommendation);

public class SupplyChainAnalyticsService(SupplyChainDbContext db)
{
    public async Task<DashboardDto> Dashboard()
    {
        var shipments = await db.Shipments.ToListAsync();
        var delayedShipmentIds = shipments
            .Where(shipment => shipment.Status == "Delayed")
            .Select(shipment => shipment.Id)
            .ToHashSet();
        var orders = await db.ProductionOrders
            .Where(order => delayedShipmentIds.Contains(order.ShipmentId))
            .ToListAsync();
        var inventoryItems = await db.InventoryItems.ToListAsync();

        return new(
            shipments.Count(shipment => shipment.Status != "Delivered"),
            shipments.Count(shipment => shipment.Status == "Delayed"),
            orders.Count,
            orders.Sum(order => order.RevenueAtRisk),
            inventoryItems.Sum(item => item.AvailableQuantity * item.UnitCost));
    }

    public async Task<ImpactDto?> Impact(int id)
    {
        var shipment = await db.Shipments.FirstOrDefaultAsync(shipment => shipment.Id == id);
        if (shipment is null)
        {
            return null;
        }

        var stock = await db.InventoryItems
            .Where(item => item.ComponentCode == shipment.ComponentCode)
            .SumAsync(item => (int?)item.AvailableQuantity) ?? 0;
        var safety = await db.InventoryItems
            .Where(item => item.ComponentCode == shipment.ComponentCode)
            .SumAsync(item => (int?)item.SafetyStock) ?? 0;
        var orders = await db.ProductionOrders
            .Where(order => order.ShipmentId == id)
            .ToListAsync();
        var days = shipment.RevisedArrival.HasValue
            ? Math.Max(
                0,
                (int)Math.Ceiling(
                    (shipment.RevisedArrival.Value - shipment.ExpectedArrival).TotalDays))
            : 0;
        var revenue = orders.Sum(order => order.RevenueAtRisk);

        return new(
            id,
            shipment.TrackingNumber,
            shipment.ComponentName,
            stock,
            safety,
            Math.Max(0, safety - stock),
            days,
            orders.Count,
            revenue,
            1800m + days * 350m,
            revenue * .35m,
            "Request a partial expedited shipment, validate alternate stock and reschedule only after planner approval.");
    }
}
