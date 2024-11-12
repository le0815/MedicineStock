using MedicineStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;

namespace MedicineStock.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly MedicineStockContext _context;

        public StatisticsController(MedicineStockContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public List<object> GetData(DateOnly startTime, DateOnly endTime)
        {
            List<object> data = new List<object>();

            var startDateTime = startTime.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endTime.ToDateTime(TimeOnly.MaxValue);            

            // get prescriptionDetail by time
            var prescriptionDetails = _context.PrescriptionDetails
                .Include(q => q.Medicine)
                .Include(q => q.Prescription)
                .Include(q => q.Medicine.ManufacturingBatches).ToList()
                .Where(t => t.Prescription.PrescriptionDate >= startDateTime
                         && t.Prescription.PrescriptionDate <= endDateTime)
                .ToList();

            // get top 6 best medicines top seller
            var bestSeller = prescriptionDetails
                .GroupBy(t => new { t.MedicineId, t.Medicine.Name })
                .Select(g => new
                {
                    MedicineName = g.Key.Name,
                    TotalQuantity = g.Sum(x => x.Quantity ?? 0)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(6)
                .ToList();

            
            // get prescription by time
            var prescriptions = _context.Prescriptions
               .Where(t => t.PrescriptionDate >= startDateTime
                         && t.PrescriptionDate <= endDateTime)
                .ToList();


            var totalSale = prescriptions.Sum(q => q.TotalPrice);
            var totalCost = prescriptionDetails.Sum(x => x.Quantity * x.Medicine.ManufacturingBatches.First()?.CostPrice ?? 0);            


            List<string> medicinesNameBestSeller = new List<string>();
            List<int> medicinesQuantityBestSeller = new List<int>();

            foreach (var item in bestSeller)
            {
                medicinesQuantityBestSeller.Add(item.TotalQuantity);
                medicinesNameBestSeller.Add(item.MedicineName);
            }

            data.Add(medicinesNameBestSeller);
            data.Add(medicinesQuantityBestSeller);
            data.Add(totalCost);
            data.Add(totalSale);

            return data;
        }
    }
}
