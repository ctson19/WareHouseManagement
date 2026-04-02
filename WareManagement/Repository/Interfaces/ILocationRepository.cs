using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default);
    Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);
    Task<Location> AddAsync(Location entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Location entity, CancellationToken cancellationToken = default);
}
