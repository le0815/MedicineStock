using MedicineStock.Data;
using MedicineStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MedicineStock.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly MedicineStockContext _context;

        public HomeController(MedicineStockContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public async Task<IActionResult> Index()
        {
            var medicines = await _context.Medicines.Include(m => m.Manufacturer).Include(j => j.MedicineType).ToListAsync();
            var prescriptions = await _context.Prescriptions.ToListAsync();
            var manufacturers = await _context.Manufacturers.ToListAsync();            


            var model = new DashboardViewModel
            {
                quantity_medicines = medicines.Count,
                quantity_prescriptions = prescriptions.Count,  
                quantity_manufacturer = manufacturers.Count,
                medicines = medicines,
                //manufacturers = manufacturers
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
