using MedicineStock.Models;

namespace MedicineStock.Models
{
    public class PrescriptionViewModel
    {
        //public virtual required List<Prescription> prescription { get; set; }
        //public virtual required List<PrescriptionDetail> prescriptionDetail { get; set; }

        public Prescription? Prescription { get; set; }
        public PrescriptionDetail? PrescriptionDetail { get; set; }

        public virtual List<Medicine>? Medicines { get; set; }
        //public virtual required List<Doctor>? doctors { get; set; }
        public virtual List<Prescription>? prescription { get; set; }
        public virtual List<PrescriptionDetail>? prescriptionDetail { get; set; }

    }
}
