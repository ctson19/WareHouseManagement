using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class LocationRepository : ILocationRepository
{
    private readonly WareManagementContext _context;

    public LocationRepository(WareManagementContext context)
    {
        _context = context;
    }

    public Task<List<Location>> GetByWarehouseAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        return _context.Locations
            .AsNoTracking()
            .Where(l => l.WarehouseId == warehouseId)
            .OrderBy(l => l.Type)
            .ThenBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Locations.AnyAsync(l => l.ParentId == id, cancellationToken);
    }

    public async Task<Location> AddAsync(Location entity, CancellationToken cancellationToken = default)
    {
        _context.Locations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Location entity, CancellationToken cancellationToken = default)
    {
        _context.Locations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Location entity, CancellationToken cancellationToken = default)
    {
        _context.Locations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
