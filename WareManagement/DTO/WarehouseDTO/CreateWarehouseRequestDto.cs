namespace WareManagement.DTO.WarehouseDTO;

public class CreateWarehouseRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
