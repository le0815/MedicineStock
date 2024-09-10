using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string? Name { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }
}
