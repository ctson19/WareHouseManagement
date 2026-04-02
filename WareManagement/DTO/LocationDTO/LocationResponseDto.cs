namespace WareManagement.DTO.LocationDTO;

public class LocationResponseDto
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int? ParentId { get; set; }
    public string? Name { get; set; }
    public string Type { get; set; } = string.Empty;
}
