using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly WareManagementContext _context;

    public RoleRepository(WareManagementContext context)
    {
        _context = context;
    }

    public Task<List<Role>> GetAllAsync()
    {
        return _context.Roles
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public Task<Role?> GetByIdAsync(int id)
    {
        return _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<bool> NameExistsAsync(string name)
    {
        return _context.Roles.AnyAsync(r => r.Name == name);
    }

    public Task<List<Role>> GetRolesByIdsAsync(IEnumerable<int> roleIds)
    {
        return _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<Role> CreateAsync(string name)
    {
        var role = new Role
        {
            Name = name
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateNameAsync(int id, string name)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role is null)
            throw new InvalidOperationException("Role not found.");

        role.Name = name;
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role is null) return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task AddRolesToUserAsync(int userId, IEnumerable<int> roleIds)
    {
        var roleIdList = roleIds.Distinct().ToList();

        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        var roles = await _context.Roles
            .Where(r => roleIdList.Contains(r.Id))
            .ToListAsync();

        foreach (var role in roles)
        {
            if (!user.Roles.Any(r => r.Id == role.Id))
                user.Roles.Add(role);
        }

        await _context.SaveChangesAsync();
    }
}

