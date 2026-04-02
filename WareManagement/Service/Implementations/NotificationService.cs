using WareManagement.DTO.NotificationDTO;
using WareManagement.Models;
using WareManagement.Repository.Interfaces;
using WareManagement.Service.Exceptions;
using WareManagement.Service.Interfaces;

namespace WareManagement.Service.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    private static NotificationResponseDto Map(Notification n) => new()
    {
        Id = n.Id,
        UserId = n.UserId,
        Title = n.Title ?? string.Empty,
        Content = n.Content ?? string.Empty,
        Type = n.Type,
        IsRead = n.IsRead ?? false,
        ReferenceId = n.ReferenceId,
        ReferenceType = n.ReferenceType,
        CreatedAt = n.CreatedAt
    };

    public async Task<(List<NotificationResponseDto> Items, int Total)> GetMyPagedAsync(
        int userId,
        int page,
        int pageSize,
        string? type,
        bool? isRead)
    {
        var (items, total) = await _notificationRepository.GetPagedByUserAsync(
            userId, page, pageSize, type, isRead);

        return (items.Select(Map).ToList(), total);
    }

    public async Task<NotificationResponseDto> GetByIdAsync(int userId, int notificationId)
    {
        var n = await _notificationRepository.GetByIdAsync(userId, notificationId);
        if (n is null) throw new NotFoundException("Không tìm thấy thông báo.");
        return Map(n);
    }

    public async Task<NotificationResponseDto> MarkReadAsync(int userId, int notificationId)
    {
        try
        {
            var n = await _notificationRepository.MarkReadAsync(userId, notificationId);
            return Map(n);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Không tìm thấy thông báo.");
        }
    }

    public Task<int> MarkAllReadAsync(int userId)
    {
        return _notificationRepository.MarkAllReadAsync(userId);
    }

    public async Task<NotificationResponseDto> CreateForUserAsync(int userId, CreateNotificationRequestDto request)
    {
        if (request is null)
            throw new ValidationException("Yêu cầu không hợp lệ.");
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ValidationException("Tiêu đề là bắt buộc.");
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ValidationException("Nội dung là bắt buộc.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new NotFoundException("Không tìm thấy user.");

        var title = request.Title.Trim();
        var content = request.Content.Trim();
        var type = string.IsNullOrWhiteSpace(request.Type) ? null : request.Type.Trim();
        var referenceType = string.IsNullOrWhiteSpace(request.ReferenceType) ? null : request.ReferenceType.Trim();

        if (title.Length > 255)
            throw new ValidationException("Title quá dài (max 255).");
        if (type is not null && type.Length > 50)
            throw new ValidationException("Type quá dài (max 50).");
        if (referenceType is not null && referenceType.Length > 50)
            throw new ValidationException("ReferenceType quá dài (max 50).");

        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            Type = type,
            IsRead = false,
            ReferenceId = request.ReferenceId,
            ReferenceType = referenceType,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _notificationRepository.CreateAsync(notification);
        return Map(created);
    }

    public async Task<NotificationResponseDto> CreateForUserByAdminAsync(int adminId, int userId, CreateNotificationRequestDto request)
    {
        if (!await _userRepository.IsAdminAsync(adminId))
            throw new ForbiddenException("Bạn không có quyền tạo thông báo cho người khác.");

        return await CreateForUserAsync(userId, request);
    }
}

