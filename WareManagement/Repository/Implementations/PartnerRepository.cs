using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class PartnerRepository : IPartnerRepository
{
    private readonly WareManagementContext _context;

    public PartnerRepository(WareManagementContext context)
    {
        _context = context;
    }

    public async Task<(List<Partner> Items, int Total)> GetPagedAsync(string? type, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var q = _context.Partners.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            var t = type.Trim();
            q = q.Where(p => p.Type == t);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(p =>
                (p.Code != null && p.Code.Contains(s)) ||
                (p.Name != null && p.Name.Contains(s)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(p => p.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Partner?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Partners.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsForTypeAsync(string code, string type, int? excludeId, CancellationToken cancellationToken = default)
    {
        var c = code.Trim();
        var t = type.Trim();
        return _context.Partners.AnyAsync(
            p => p.Code == c && p.Type == t && (!excludeId.HasValue || p.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<Partner> AddAsync(Partner entity, CancellationToken cancellationToken = default)
    {
        _context.Partners.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Partner entity, CancellationToken cancellationToken = default)
    {
        _context.Partners.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasReferencesAsync(int id, CancellationToken cancellationToken = default)
    {
        var imp = await _context.ImportReceipts.AnyAsync(r => r.SupplierId == id, cancellationToken);
        if (imp) return true;
        return await _context.ExportReceipts.AnyAsync(r => r.CustomerId == id, cancellationToken);
    }

    public async Task DeleteAsync(Partner entity, CancellationToken cancellationToken = default)
    {
        _context.Partners.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
