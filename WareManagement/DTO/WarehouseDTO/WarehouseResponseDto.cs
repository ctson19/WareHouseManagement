namespace WareManagement.DTO.WarehouseDTO;

public class WarehouseResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}
