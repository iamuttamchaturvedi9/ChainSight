
using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace SupplyChainIntelligencePlatform.Services;

[McpServerToolType]
public class ShipmentTools
{
    [McpServerTool]
    [Description("Gets the operational and financial impact of a shipment disruption.")]
    public static async Task<string> GetShipmentImpact(
        [Description("The database ID of the shipment")]
        int shipmentId,
        SupplyChainAnalyticsService analytics)
    {
        var impact = await analytics.Impact(shipmentId);

        if (impact is null)
        {
            return $"Shipment with ID {shipmentId} was not found.";
        }

        return JsonSerializer.Serialize(
            impact,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
    }
}
