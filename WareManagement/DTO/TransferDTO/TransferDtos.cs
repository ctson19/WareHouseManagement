namespace WareManagement.DTO.TransferDTO;

public class TransferLineDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}

public class CreateTransferRequestDto
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public List<TransferLineDto> Lines { get; set; } = new();
}

public class TransferConfirmLineDto
{
    public int DetailId { get; set; }
    public int FromLocationId { get; set; }
    public int ToLocationId { get; set; }
}

public class TransferConfirmRequestDto
{
    public List<TransferConfirmLineDto> Lines { get; set; } = new();
}

public class TransferDetailResponseDto
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public decimal? Quantity { get; set; }
}

public class TransferResponseDto
{
    public int Id { get; set; }
    public int FromWarehouseId { get; set; }
    public string? FromWarehouseCode { get; set; }
    public int ToWarehouseId { get; set; }
    public string? ToWarehouseCode { get; set; }
    public string? Status { get; set; }
    public int? ExportReceiptId { get; set; }
    public int? ImportReceiptId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<TransferDetailResponseDto> Lines { get; set; } = new();
}
