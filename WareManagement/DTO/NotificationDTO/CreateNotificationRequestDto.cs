namespace WareManagement.DTO.NotificationDTO;

public class CreateNotificationRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Type { get; set; }

    // Tham chiếu (VD: ReceiptId), tùy app dùng.
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
}
