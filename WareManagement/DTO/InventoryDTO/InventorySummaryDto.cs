namespace WareManagement.DTO.InventoryDTO;

public class InventorySummaryDto
{
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    /// <summary>Ok, BelowMin, AboveMax</summary>
    public string StockAlert { get; set; } = "Ok";
}
