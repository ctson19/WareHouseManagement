namespace WareManagement.DTO.ProductDTO;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? Barcode { get; set; }
    public decimal? ImportPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? MaxStock { get; set; }
    public bool IsActive { get; set; }
}
