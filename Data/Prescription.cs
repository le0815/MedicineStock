using System;
using System.Collections.Generic;

namespace MedicineStock.Data;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int? DoctorId { get; set; }

    public int? PatientId { get; set; }

    public DateOnly? PrescriptionDate { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual ICollection<PrescriptionDetail>? PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
