using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class PrescriptionDetail
{
    public int PrescriptionDetailId { get; set; }

    public int? PrescriptionId { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public int? MedicineId { get; set; }

    public virtual Medicine? Medicine { get; set; }

    public virtual Prescription? Prescription { get; set; }
}
