using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public DateTime PrescriptionDate { get; set; }

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
