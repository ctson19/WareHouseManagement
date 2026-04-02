using WareManagement.DTO.PermissionDTO;

namespace WareManagement.Service.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionResponseDto>> GetAllAsync(int adminId);
    Task<PermissionResponseDto> GetByIdAsync(int adminId, int id);
    Task<PermissionResponseDto> CreateAsync(int adminId, CreatePermissionRequestDto request);
    Task<PermissionResponseDto> UpdateAsync(int adminId, int id, UpdatePermissionRequestDto request);
    Task DeleteAsync(int adminId, int id);

    Task<RolePermissionsResponseDto> GetRolePermissionsAsync(int adminId, int roleId);
    Task<RolePermissionsResponseDto> AssignPermissionsToRoleAsync(int adminId, int roleId, AssignPermissionsToRoleRequestDto request);
}
