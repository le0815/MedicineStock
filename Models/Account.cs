using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? PermissionId { get; set; }

    public virtual Permission? Permission { get; set; }
}
