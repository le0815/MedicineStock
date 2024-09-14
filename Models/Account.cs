using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicineStock.Models;

public partial class Account 
{
    public int AccountId { get; set; }

    [Required]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? PermissionId { get; set; }

    public virtual Permission? Permission { get; set; }
}
