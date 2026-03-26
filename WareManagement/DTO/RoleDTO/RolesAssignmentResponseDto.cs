namespace WareManagement.DTO.RoleDTO;

public class RolesAssignmentResponseDto
{
    public int UserId { get; set; }
    public List<RoleResponseDto> Roles { get; set; } = new List<RoleResponseDto>();
}

