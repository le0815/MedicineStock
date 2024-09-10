using MedicineStock.Models;

namespace MedicineStock.Models
{
    public class DashboardViewModel
    {
        public int quantity_medicines { get; set; }
        public int quantity_prescriptions { get; set; }
        public int quantity_manufacturer { get; set; }

        public virtual required List<ManufacturingBatch> manufacturingBatch { get; set; }
        //public virtual Medicine medicines { get; set; }
        //public required List<Manufacturer> manufacturers { get; set; }
        
    }
}
