using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface IProductRepository
{
    Task<List<Unit>> GetUnitsAsync(CancellationToken cancellationToken = default);
    Task<Unit?> GetUnitByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Product> Items, int Total)> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product entity, CancellationToken cancellationToken = default);
}
