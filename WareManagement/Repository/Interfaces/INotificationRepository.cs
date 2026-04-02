using WareManagement.Models;

namespace WareManagement.Repository.Interfaces;

public interface INotificationRepository
{
    Task<(List<Notification> Items, int Total)> GetPagedByUserAsync(
        int userId,
        int page,
        int pageSize,
        string? type,
        bool? isRead,
        CancellationToken cancellationToken = default);

    Task<Notification?> GetByIdAsync(int userId, int notificationId, CancellationToken cancellationToken = default);

    Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);

    Task<Notification> MarkReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default);

    Task<int> MarkAllReadAsync(int userId, CancellationToken cancellationToken = default);
}
