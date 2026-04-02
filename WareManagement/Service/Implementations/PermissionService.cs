using WareManagement.DTO.PermissionDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;

    public PermissionService(IPermissionRepository permissionRepository, IUserRepository userRepository)
    {
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
    }

    private async Task EnsureAdminAsync(int adminId)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("Bạn không có quyền quản trị permission.");
    }

    private static PermissionResponseDto Map(Permission p) => new()
    {
        Id = p.Id,
        Code = p.Code ?? string.Empty,
        Name = p.Name ?? string.Empty
    };

    public async Task<List<PermissionResponseDto>> GetAllAsync(int adminId)
    {
        await EnsureAdminAsync(adminId);
        var items = await _permissionRepository.GetAllAsync();
        return items.Select(Map).ToList();
    }

    public async Task<PermissionResponseDto> GetByIdAsync(int adminId, int id)
    {
        await EnsureAdminAsync(adminId);
        var p = await _permissionRepository.GetByIdAsync(id);
        if (p is null) throw new NotFoundException("Không tìm thấy permission.");
        return Map(p);
    }

    public async Task<PermissionResponseDto> CreateAsync(int adminId, CreatePermissionRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null)
            throw new ValidationException("Yêu cầu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Code permission là bắt buộc.");
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Name permission là bắt buộc.");

        var code = request.Code.Trim();
        var name = request.Name.Trim();

        if (code.Length > 100)
            throw new ValidationException("Code permission quá dài (max 100).");
        if (name.Length > 255)
            throw new ValidationException("Name permission quá dài (max 255).");

        if (await _permissionRepository.CodeExistsAsync(code))
            throw new ConflictException("Code permission đã tồn tại.");

        var created = await _permissionRepository.CreateAsync(code, name);
        return Map(created);
    }

    public async Task<PermissionResponseDto> UpdateAsync(int adminId, int id, UpdatePermissionRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null)
            throw new ValidationException("Yêu cầu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new ValidationException("Code permission là bắt buộc.");
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Name permission là bắt buộc.");

        var code = request.Code.Trim();
        var name = request.Name.Trim();

        if (code.Length > 100)
            throw new ValidationException("Code permission quá dài (max 100).");
        if (name.Length > 255)
            throw new ValidationException("Name permission quá dài (max 255).");

        var existing = await _permissionRepository.GetByIdAsync(id);
        if (existing is null)
            throw new NotFoundException("Không tìm thấy permission.");

        if (!string.Equals(existing.Code, code, StringComparison.Ordinal) &&
            await _permissionRepository.CodeExistsAsync(code))
            throw new ConflictException("Code permission đã tồn tại.");

        var updated = await _permissionRepository.UpdateAsync(id, code, name);
        return Map(updated);
    }

    public async Task DeleteAsync(int adminId, int id)
    {
        await EnsureAdminAsync(adminId);
        var deleted = await _permissionRepository.DeleteAsync(id);
        if (!deleted) throw new NotFoundException("Không tìm thấy permission.");
    }

    public async Task<RolePermissionsResponseDto> GetRolePermissionsAsync(int adminId, int roleId)
    {
        await EnsureAdminAsync(adminId);

        var perms = await _permissionRepository.GetPermissionsByRoleIdAsync(roleId);
        return new RolePermissionsResponseDto
        {
            RoleId = roleId,
            Permissions = perms.Select(Map).ToList()
        };
    }

    public async Task<RolePermissionsResponseDto> AssignPermissionsToRoleAsync(
        int adminId,
        int roleId,
        AssignPermissionsToRoleRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null)
            throw new ValidationException("Yêu cầu không hợp lệ.");
        if (request.PermissionIds is null || request.PermissionIds.Length == 0)
            throw new ValidationException("PermissionIds là bắt buộc.");

        try
        {
            await _permissionRepository.AddPermissionsToRoleAsync(roleId, request.PermissionIds);
        }
        catch (InvalidOperationException ex)
        {
            var msg = ex.Message ?? string.Empty;
            if (msg.Contains("Role not found", StringComparison.OrdinalIgnoreCase))
                throw new NotFoundException("Không tìm thấy role.");
            if (msg.Contains("invalid", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Một hoặc nhiều permissionIds không hợp lệ.");
            throw;
        }

        var perms = await _permissionRepository.GetPermissionsByRoleIdAsync(roleId);
        return new RolePermissionsResponseDto
        {
            RoleId = roleId,
            Permissions = perms.Select(Map).ToList()
        };
    }
}

