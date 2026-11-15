
using Microsoft.EntityFrameworkCore;
using SupplyChainIntelligencePlatform.Data;
using SupplyChainIntelligencePlatform.Models;

namespace SupplyChainIntelligencePlatform.Services;

// Data returned to the Logistics UI
public record ShipmentListItemDto(
    int Id,
    string TrackingNumber,
    string ComponentName,
    string ComponentCode,
    string SupplierName,
    string Origin,
    string Destination,
    DateTimeOffset ExpectedArrival,
    string Status
);

// Result of a recommendation approval operation
public record ApprovalResult(
    bool Success,
    string Message
);

public class ShipmentService(SupplyChainDbContext db)
{
    // Retrieve shipments for the Logistics screen
    public async Task<List<ShipmentListItemDto>> GetShipmentsAsync()
    {
        return await db.Shipments
            .AsNoTracking()
            .OrderBy(x => x.ExpectedArrival)
            .Select(x => new ShipmentListItemDto(
                x.Id,
                x.TrackingNumber,
                x.ComponentName,
                x.ComponentCode,
                x.Supplier != null ? x.Supplier.Name : "Unknown",
                x.Origin,
                x.Destination,
                x.ExpectedArrival,
                x.Status
            ))
            .ToListAsync();
    }

    // Approve a pending recommendation
    public async Task<ApprovalResult> ApproveRecommendationAsync(
        int shipmentId)
    {
        var recommendation = await db.Recommendations
            .FirstOrDefaultAsync(x =>
                x.ShipmentId == shipmentId &&
                x.Status == "Pending");

        if (recommendation is null)
        {
            return new ApprovalResult(
                false,
                "No pending recommendation found.");
        }

        recommendation.Status = "Approved";

        await db.SaveChangesAsync();

        return new ApprovalResult(
            true,
            "Approved in demo workflow. No external action executed.");
    }
}
