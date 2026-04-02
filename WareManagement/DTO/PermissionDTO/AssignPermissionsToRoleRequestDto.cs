namespace WareManagement.DTO.PermissionDTO;

public class AssignPermissionsToRoleRequestDto
{
    public int[] PermissionIds { get; set; } = Array.Empty<int>();
}
