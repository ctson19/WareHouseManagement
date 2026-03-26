using System;
using System.Collections.Generic;

namespace WareManagement.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
