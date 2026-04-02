using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface IPartnerRepository
{
    Task<(List<Partner> Items, int Total)> GetPagedAsync(string? type, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Partner?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsForTypeAsync(string code, string type, int? excludeId, CancellationToken cancellationToken = default);
    Task<Partner> AddAsync(Partner entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Partner entity, CancellationToken cancellationToken = default);
    Task<bool> HasReferencesAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Partner entity, CancellationToken cancellationToken = default);
}
