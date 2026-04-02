namespace WareManagement.DTO.WarehouseDTO;

public class UpdateWarehouseRequestDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
