using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly WareManagementContext _context;

    public PermissionRepository(WareManagementContext context)
    {
        _context = context;
    }

    public Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Permissions
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return _context.Permissions.AnyAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<Permission> CreateAsync(string code, string name, CancellationToken cancellationToken = default)
    {
        var permission = new Permission
        {
            Code = code,
            Name = name
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync(cancellationToken);
        return permission;
    }

    public async Task<Permission> UpdateAsync(int id, string code, string name, CancellationToken cancellationToken = default)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (permission is null)
            throw new InvalidOperationException("Permission not found.");

        permission.Code = code;
        permission.Name = name;
        await _context.SaveChangesAsync(cancellationToken);
        return permission;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (permission is null) return false;

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<List<Permission>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return _context.Permissions
            .Where(p => p.Roles.Any(r => r.Id == roleId))
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPermissionsToRoleAsync(
        int roleId,
        IEnumerable<int> permissionIds,
        CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role is null)
            throw new InvalidOperationException("Role not found.");

        var permissionIdList = permissionIds.Distinct().ToList();
        var permissions = await _context.Permissions
            .Where(p => permissionIdList.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (permissions.Count != permissionIdList.Count)
            throw new InvalidOperationException("Some permissionIds are invalid.");

        foreach (var permission in permissions)
        {
            if (!role.Permissions.Any(p => p.Id == permission.Id))
                role.Permissions.Add(permission);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

