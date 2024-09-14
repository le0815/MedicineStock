using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
