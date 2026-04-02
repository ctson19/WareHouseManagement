using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly WareManagementContext _context;

    public ProductRepository(WareManagementContext context)
    {
        _context = context;
    }

    public Task<List<Unit>> GetUnitsAsync(CancellationToken cancellationToken = default)
    {
        return _context.Units.AsNoTracking().OrderBy(u => u.Name).ToListAsync(cancellationToken);
    }

    public Task<Unit?> GetUnitByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Units.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<(List<Product> Items, int Total)> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var q = _context.Products
            .Include(p => p.Unit)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(p =>
                (p.Code != null && p.Code.Contains(s)) ||
                (p.Name != null && p.Name.Contains(s)) ||
                (p.Barcode != null && p.Barcode.Contains(s)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(p => p.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, int? excludeId, CancellationToken cancellationToken = default)
    {
        var c = code.Trim();
        return _context.Products.AnyAsync(
            p => p.Code == c && (!excludeId.HasValue || p.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
