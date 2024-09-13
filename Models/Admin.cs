﻿using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public int? PermissionId { get; set; }

    public virtual Permission? Permission { get; set; }
}
