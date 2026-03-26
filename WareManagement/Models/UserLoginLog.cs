using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class UserLoginLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public bool? IsSuccess { get; set; }

    public string? Ipaddress { get; set; }

    public virtual User? User { get; set; }
}
