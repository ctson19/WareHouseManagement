namespace WareManagement.DTO.PermissionDTO;

public class RolePermissionsResponseDto
{
    public int RoleId { get; set; }
    public List<PermissionResponseDto> Permissions { get; set; } = new();
}
