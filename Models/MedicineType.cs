using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class MedicineType
{
    public int MedicineTypeId { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
