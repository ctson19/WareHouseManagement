using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<Permission> CreateAsync(string code, string name, CancellationToken cancellationToken = default);

    Task<Permission> UpdateAsync(int id, string code, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Permission>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);

    Task AddPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default);
}
