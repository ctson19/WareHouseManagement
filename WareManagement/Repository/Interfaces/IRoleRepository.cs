using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name);
    Task<List<Role>> GetRolesByIdsAsync(IEnumerable<int> roleIds);

    Task<Role> CreateAsync(string name);
    Task<Role> UpdateNameAsync(int id, string name);
    Task<bool> DeleteAsync(int id);

    Task AddRolesToUserAsync(int userId, IEnumerable<int> roleIds);
}

