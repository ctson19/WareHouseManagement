namespace WareManagement.DTO.NotificationDTO;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string? Type { get; set; }

    public bool IsRead { get; set; }

    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }

    public DateTime? CreatedAt { get; set; }
}
