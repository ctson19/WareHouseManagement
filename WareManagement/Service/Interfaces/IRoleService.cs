using WareManagement.DTO.RoleDTO;

namespace WareManagement.Service.Interfaces;

public interface IRoleService
{
    Task<List<RoleResponseDto>> GetAllAsync(int adminId);
    Task<RoleResponseDto> GetByIdAsync(int adminId, int id);
    Task<RoleResponseDto> CreateAsync(int adminId, CreateRoleRequestDto request);
    Task<RoleResponseDto> UpdateAsync(int adminId, int id, UpdateRoleRequestDto request);
    Task DeleteAsync(int adminId, int id);
    Task<RolesAssignmentResponseDto> AddRolesToUserAsync(int adminId, int userId, AddRolesToUserRequestDto request);
}

