using System;
using System.Collections.Generic;

namespace MedicineStock.Data;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public int ManufacturerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Origin { get; set; } = null!;

    public DateOnly? ExpiryDate { get; set; }

    public decimal? Price { get; set; }

    public decimal? Quantiy { get; set; }

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
