namespace WareManagement.DTO.ImportReceiptDTO;

public class ImportReceiptLineDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public int LocationId { get; set; }
}

public class CreateImportReceiptRequestDto
{
    public string? Code { get; set; }
    public int WarehouseId { get; set; }
    public int? SupplierId { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<ImportReceiptLineDto> Lines { get; set; } = new();
}

public class UpdateImportReceiptRequestDto
{
    public int? SupplierId { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<ImportReceiptLineDto> Lines { get; set; } = new();
}

public class ImportReceiptDetailResponseDto
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

public class ImportReceiptResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<ImportReceiptDetailResponseDto> Lines { get; set; } = new();
}
