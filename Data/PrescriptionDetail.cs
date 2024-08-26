using System;
using System.Collections.Generic;

namespace MedicineStock.Data;

public partial class PrescriptionDetail
{
    public int PrescriptionDetailId { get; set; }

    public int? PrescriptionId { get; set; }

    public int? MedicineId { get; set; }

    public int ManufacturerId { get; set; }

    public int? Quantity { get; set; }

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual Medicine? Medicine { get; set; }

    public virtual Prescription? Prescription { get; set; }
}
