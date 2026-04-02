namespace WareManagement.DTO.ExportReceiptDTO;

public class ExportReceiptLineDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public int LocationId { get; set; }
}

public class CreateExportReceiptRequestDto
{
    public string? Code { get; set; }
    public int WarehouseId { get; set; }
    public int? CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<ExportReceiptLineDto> Lines { get; set; } = new();
}

public class UpdateExportReceiptRequestDto
{
    public int? CustomerId { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<ExportReceiptLineDto> Lines { get; set; } = new();
}

public class ExportReceiptDetailResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal LineAmount { get; set; }
    public int LocationId { get; set; }
    public string? LocationName { get; set; }
}

public class ExportReceiptResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<ExportReceiptDetailResponseDto> Lines { get; set; } = new();
}
