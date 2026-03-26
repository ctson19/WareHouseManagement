using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Type { get; set; }

    public bool? IsRead { get; set; }

    public int? ReferenceId { get; set; }

    public string? ReferenceType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
