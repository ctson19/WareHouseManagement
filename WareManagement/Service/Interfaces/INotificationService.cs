using WareManagement.DTO.NotificationDTO;

namespace WareManagement.Service.Interfaces;

public interface INotificationService
{
    Task<(List<NotificationResponseDto> Items, int Total)> GetMyPagedAsync(
        int userId,
        int page,
        int pageSize,
        string? type,
        bool? isRead);

    Task<NotificationResponseDto> GetByIdAsync(int userId, int notificationId);

    Task<NotificationResponseDto> MarkReadAsync(int userId, int notificationId);

    Task<int> MarkAllReadAsync(int userId);

    // Dùng cho nội bộ: tạo thông báo cho user (không kiểm tra admin).
    Task<NotificationResponseDto> CreateForUserAsync(int userId, CreateNotificationRequestDto request);

    // Dùng cho admin: tạo thông báo cho user bất kỳ.
    Task<NotificationResponseDto> CreateForUserByAdminAsync(int adminId, int userId, CreateNotificationRequestDto request);
}
