namespace SupplyChainIntelligencePlatform.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Country { get; set; } = "";
    public int RiskScore { get; set; }
}

public class Shipment
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = "";
    public string ComponentCode { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Destination { get; set; } = "";
    public DateTimeOffset ExpectedArrival { get; set; }
    public DateTimeOffset? RevisedArrival { get; set; }
    public string Status { get; set; } = "In Transit";
    public int Quantity { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public decimal FreightCost { get; set; }
}

public class InventoryItem
{
    public int Id { get; set; }
    public string ComponentCode { get; set; } = "";
    public string ComponentName { get; set; } = "";
    public string Warehouse { get; set; } = "";
    public int AvailableQuantity { get; set; }
    public int SafetyStock { get; set; }
    public decimal UnitCost { get; set; }
}

public class ProductionOrder
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string ProductName { get; set; } = "";
    public DateTimeOffset PlannedStart { get; set; }
    public int UnitsPlanned { get; set; }
    public decimal RevenueAtRisk { get; set; }
    public int ShipmentId { get; set; }
}

public class Recommendation
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public string Action { get; set; } = "";
    public string Rationale { get; set; } = "";
    public decimal EstimatedCost { get; set; }
    public decimal AvoidedLoss { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
