using MedicineStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MedicineStock.Controllers
{
    public class ProfilesController : Controller
    {
        public IActionResult Authentic()
        {
            return View();
        }
    }
}
