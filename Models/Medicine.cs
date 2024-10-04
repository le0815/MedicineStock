using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? MedicineTypeId { get; set; }

    public double? Discount { get; set; }

    public virtual ICollection<ManufacturingBatch> ManufacturingBatches { get; set; } = new List<ManufacturingBatch>();

    public virtual MedicineType? MedicineType { get; set; }
}
