namespace WareManagement.DTO.LocationDTO;

public class UpdateLocationRequestDto
{
    public int? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
}
