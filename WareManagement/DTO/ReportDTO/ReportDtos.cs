namespace WareManagement.DTO.ReportDTO;

public class NxtRowDto
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public decimal ImportQty { get; set; }
    public decimal ExportQty { get; set; }
    public decimal NetChange { get; set; }
}

public class StockTransactionRowDto
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? TransactionType { get; set; }
    public decimal? Quantity { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public int ProductId { get; set; }
    public string? ProductCode { get; set; }
}

public class PartnerReceiptRowDto
{
    public string ReceiptKind { get; set; } = string.Empty;
    public int ReceiptId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Status { get; set; }
}

public class DashboardSummaryDto
{
    public int ImportReceiptsToday { get; set; }
    public int ExportReceiptsToday { get; set; }
    public int DraftImportCount { get; set; }
    public int DraftExportCount { get; set; }
    public List<InventoryTopDto> TopStockHighest { get; set; } = new();
    public List<InventoryTopDto> TopStockLowest { get; set; } = new();
}

public class InventoryTopDto
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
}
