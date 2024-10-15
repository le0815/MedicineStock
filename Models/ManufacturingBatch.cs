using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class ManufacturingBatch
{
    public int ManufacturingBatchId { get; set; }

    public string? Name { get; set; }

    public int? ManufacturerId { get; set; }

    public int? MedicineId { get; set; }

    public DateOnly? ImportDate { get; set; }

    public string? Origin { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public decimal? CostPrice { get; set; }

    public int? QuantityRemaining { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual Medicine? Medicine { get; set; }

    public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; } = new List<PrescriptionDetail>();
}
