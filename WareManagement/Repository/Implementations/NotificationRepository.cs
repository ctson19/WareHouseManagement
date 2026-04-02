using Microsoft.EntityFrameworkCore;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;

namespace WareManagement.Repository.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly WareManagementContext _context;

    public NotificationRepository(WareManagementContext context)
    {
        _context = context;
    }

    public async Task<(List<Notification> Items, int Total)> GetPagedByUserAsync(
        int userId,
        int page,
        int pageSize,
        string? type,
        bool? isRead,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 200) pageSize = 200;

        var q = _context.Notifications
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            q = q.Where(n => n.Type == type.Trim());

        if (isRead.HasValue)
            q = q.Where(n => (n.IsRead ?? false) == isRead.Value);

        q = q.OrderByDescending(n => n.CreatedAt);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Notification?> GetByIdAsync(int userId, int notificationId, CancellationToken cancellationToken = default)
    {
        return _context.Notifications
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == notificationId, cancellationToken);
    }

    public async Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
        return notification;
    }

    public async Task<Notification> MarkReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default)
    {
        var n = await _context.Notifications
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == notificationId, cancellationToken);

        if (n is null)
            throw new InvalidOperationException("Notification not found.");

        n.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);
        return n;
    }

    public async Task<int> MarkAllReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Notifications
            .Where(n => n.UserId == userId && (n.IsRead ?? false) == false)
            .ToListAsync(cancellationToken);

        foreach (var n in list)
            n.IsRead = true;

        await _context.SaveChangesAsync(cancellationToken);
        return list.Count;
    }
}

