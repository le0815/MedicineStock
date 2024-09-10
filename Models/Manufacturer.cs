using System;
using System.Collections.Generic;

namespace MedicineStock.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<ManufacturingBatch> ManufacturingBatches { get; set; } = new List<ManufacturingBatch>();
}
