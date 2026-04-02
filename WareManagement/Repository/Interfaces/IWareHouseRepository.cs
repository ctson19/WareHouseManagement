using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface IWareHouseRepository
{
    Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(List<Warehouse> Items, int Total)> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId, CancellationToken cancellationToken = default);
    Task<Warehouse> AddAsync(Warehouse entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Warehouse entity, CancellationToken cancellationToken = default);
}
