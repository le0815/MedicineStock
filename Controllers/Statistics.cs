using Microsoft.AspNetCore.Mvc;

namespace MedicineStock.Controllers
{
    public class Statistics : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
