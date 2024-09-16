﻿using MedicineStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Text.Json;

namespace MedicineStock.Controllers
{

    public class AccountController : Controller
    {

        private readonly MedicineStockContext _context;

        public AccountController(MedicineStockContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Account model)
        {
            if (ModelState.IsValid)
            {
                var result = _context.Accounts.Where(q => q.Password == model.Password && q.UserName == model.UserName).FirstOrDefault();
                if (result != null)
                {
                    // set account info to session
                    HttpContext.Session.SetString("Account", System.Text.Json.JsonSerializer.Serialize(result));
                    
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        public IActionResult logout()
        {
            // remove account info session
            HttpContext.Session.Remove("Account");

            return View("Login");
        }

        public async Task<IActionResult> Index()
        {
            //var medicineStockContext1 = _context.Medicines.Include(m => m.Manufacturer).Include(m => m.MedicineType);
            var medicineStockContext1 = _context.Accounts.Include(m => m.Permission);
            return View(await medicineStockContext1.ToListAsync());
        }

        public IActionResult Create()
        {
            //var medicineStockContext1 = _context.Medicines.Include(m => m.Manufacturer).Include(m => m.MedicineType);
            //var medicineStockContext1 = _context.Accounts.Include(m => m.Permission);
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Password,PermissionId")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MedicineTypeId"] = new SelectList(_context.Permissions, "PermissionId", "Name", account.PermissionId);
            return View(account);
        }
    }
}
