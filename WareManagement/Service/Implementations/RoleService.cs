using WareManagement.DTO.RoleDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public RoleService(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    private async Task EnsureAdminAsync(int adminId)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("You don't have permission to perform this action.");
    }

    public async Task<List<RoleResponseDto>> GetAllAsync(int adminId)
    {
        await EnsureAdminAsync(adminId);

        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(r => new RoleResponseDto { Id = r.Id, Name = r.Name }).ToList();
    }

    public async Task<RoleResponseDto> GetByIdAsync(int adminId, int id)
    {
        await EnsureAdminAsync(adminId);

        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null) throw new NotFoundException("Role not found.");

        return new RoleResponseDto { Id = role.Id, Name = role.Name };
    }

    public async Task<RoleResponseDto> CreateAsync(int adminId, CreateRoleRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null) throw new ValidationException("Request is required.");
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Role name is required.");

        if (request.Name.Length > 100)
            throw new ValidationException("Role name is too long (max 100).");

        if (await _roleRepository.NameExistsAsync(request.Name))
            throw new ConflictException("Role name already exists.");

        var role = await _roleRepository.CreateAsync(request.Name.Trim());
        return new RoleResponseDto { Id = role.Id, Name = role.Name };
    }

    public async Task<RoleResponseDto> UpdateAsync(int adminId, int id, UpdateRoleRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null) throw new ValidationException("Request is required.");
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Role name is required.");

        if (request.Name.Length > 100)
            throw new ValidationException("Role name is too long (max 100).");

        var existing = await _roleRepository.GetByIdAsync(id);
        if (existing is null) throw new NotFoundException("Role not found.");

        // Nếu đổi sang tên đã tồn tại cho role khác
        var nameToSet = request.Name.Trim();
        if (!string.Equals(existing.Name, nameToSet, StringComparison.Ordinal) &&
            await _roleRepository.NameExistsAsync(nameToSet))
            throw new ConflictException("Role name already exists.");

        var updated = await _roleRepository.UpdateNameAsync(id, nameToSet);
        return new RoleResponseDto { Id = updated.Id, Name = updated.Name };
    }

    public async Task DeleteAsync(int adminId, int id)
    {
        await EnsureAdminAsync(adminId);

        var deleted = await _roleRepository.DeleteAsync(id);
        if (!deleted) throw new NotFoundException("Role not found.");
    }

    public async Task<RolesAssignmentResponseDto> AddRolesToUserAsync(int adminId, int userId, AddRolesToUserRequestDto request)
    {
        await EnsureAdminAsync(adminId);

        if (request is null) throw new ValidationException("Request is required.");
        if (request.RoleIds is null || request.RoleIds.Length == 0)
            throw new ValidationException("RoleIds is required.");

        var roleIdList = request.RoleIds.Distinct().ToList();

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new NotFoundException("User not found.");

        if (user.IsActive != true)
            throw new ValidationException("User is inactive.");

        var roles = await _roleRepository.GetRolesByIdsAsync(roleIdList);
        if (roles.Count != roleIdList.Count)
            throw new ValidationException("Some roleIds are invalid.");

        await _roleRepository.AddRolesToUserAsync(userId, roleIdList);

        return new RolesAssignmentResponseDto
        {
            UserId = userId,
            Roles = roles.Select(r => new RoleResponseDto { Id = r.Id, Name = r.Name }).ToList()
        };
    }
}

