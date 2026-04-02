namespace WareManagement.DTO.LocationDTO;

public class CreateLocationRequestDto
{
    public int WarehouseId { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
