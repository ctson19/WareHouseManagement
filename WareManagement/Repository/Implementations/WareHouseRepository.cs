using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class WareHouseRepository : IWareHouseRepository
{
    private readonly WareManagementContext _context;

    public WareHouseRepository(WareManagementContext context)
    {
        _context = context;
    }

    public Task<List<Warehouse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Warehouses
            .AsNoTracking()
            .OrderBy(w => w.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Warehouse> Items, int Total)> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var q = _context.Warehouses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(w =>
                (w.Code != null && w.Code.Contains(s)) ||
                (w.Name != null && w.Name.Contains(s)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(w => w.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Warehouse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, int? excludeId, CancellationToken cancellationToken = default)
    {
        var c = code.Trim();
        return _context.Warehouses.AnyAsync(
            w => w.Code == c && (!excludeId.HasValue || w.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<Warehouse> AddAsync(Warehouse entity, CancellationToken cancellationToken = default)
    {
        _context.Warehouses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Warehouse entity, CancellationToken cancellationToken = default)
    {
        _context.Warehouses.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
