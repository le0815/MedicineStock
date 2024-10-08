﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicineStock.Models;

namespace MedicineStock.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly MedicineStockContext _context;

        public MedicinesController(MedicineStockContext context)
        {
            _context = context;
        }

        // GET: Medicines
        [Authentication]        
        public async Task<IActionResult> Index()
        {
            //var medicineStockContext1 = _context.Medicines.Include(m => m.Manufacturer).Include(m => m.MedicineType);
            var medicineStockContext1 = _context.Medicines.Include(m => m.MedicineType);
            return View(await medicineStockContext1.ToListAsync());
        }

        // GET: Medicines/Details/5
        [Authentication]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineId == id);
            if (medicine == null)
            {
                return NotFound();
            }
            ViewData["MedicineTypes"] = await _context.MedicineTypes.ToListAsync();
            return View(medicine);
        }

        // GET: Medicines/Create
        [Authentication]
        [CheckPermisson]
        public IActionResult Create()
        {
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "Name");
            ViewData["MedicineTypeId"] = new SelectList(_context.MedicineTypes, "MedicineTypeId", "Type");
            return View();
        }

        // POST: Medicines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authentication]
        [CheckPermisson]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ManufacturerId,Name,Description,Price,MedicineTypeId")] Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["MedicineTypeId"] = new SelectList(_context.MedicineTypes, "MedicineTypeId", "Type", medicine.MedicineTypeId);
            return View(medicine);
        }

        // GET: Medicines/Edit/5
        [Authentication]
        [CheckPermisson]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            
            ViewData["MedicineTypeId"] = new SelectList(_context.MedicineTypes, "MedicineTypeId", "Type", medicine.MedicineTypeId);
            return View(medicine);
        }

        // POST: Medicines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authentication]
        [CheckPermisson]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicineId,ManufacturerId,Name,Description,Origin,ExpiryDate,Price,Quantiy,MedicineTypeId")] Medicine medicine)
        {
            if (id != medicine.MedicineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicineExists(medicine.MedicineId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["MedicineTypeId"] = new SelectList(_context.MedicineTypes, "MedicineTypeId", "Type", medicine.MedicineTypeId);
            return View(medicine);
            //return await Index();
        }

        // GET: Medicines/Delete/5
        [Authentication]
        [CheckPermisson]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["MedicineTypeId"] = await _context.MedicineTypes.ToListAsync();
            var medicine = await _context.Medicines                
                .FirstOrDefaultAsync(m => m.MedicineId == id);
            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [Authentication]
        [CheckPermisson]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["MedicineTypeId"] = await _context.MedicineTypes.ToListAsync();
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(int id)
        {
            return _context.Medicines.Any(e => e.MedicineId == id);
        }
    }
}
