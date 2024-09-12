using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicineStock.Models;

namespace MedicineStock.Controllers
{
    public class ManufacturingBatchesController : Controller
    {
        private readonly MedicineStockContext _context;

        public ManufacturingBatchesController(MedicineStockContext context)
        {
            _context = context;
        }

        // GET: ManufacturingBatches
        public async Task<IActionResult> Index()
        {
            var medicineStockContext = _context.ManufacturingBatches.Include(m => m.Manufacturer).Include(m => m.Medicine);
            return View(await medicineStockContext.ToListAsync());
        }

        // GET: ManufacturingBatches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturingBatch = await _context.ManufacturingBatches
                .Include(m => m.Manufacturer)
                .Include(m => m.Medicine)
                .FirstOrDefaultAsync(m => m.ManufacturingBatchId == id);
            if (manufacturingBatch == null)
            {
                return NotFound();
            }

            return View(manufacturingBatch);
        }

        // GET: ManufacturingBatches/Create
        public IActionResult Create()
        {
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "Name");
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "Name");
            return View();
        }

        // POST: ManufacturingBatches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ManufacturingBatchId,Name,ManufacturerId,MedicineId,ImportDate,Origin,ExpiryDate,Quantity,Description")] ManufacturingBatch manufacturingBatch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manufacturingBatch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "Name", manufacturingBatch.ManufacturerId);
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "Name", manufacturingBatch.MedicineId);
            return View(manufacturingBatch);
        }

        // GET: ManufacturingBatches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturingBatch = await _context.ManufacturingBatches.FindAsync(id);
            if (manufacturingBatch == null)
            {
                return NotFound();
            }
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "Name", manufacturingBatch.ManufacturerId);
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "Name", manufacturingBatch.MedicineId);
            return View(manufacturingBatch);
        }

        // POST: ManufacturingBatches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ManufacturingBatchId,Name,ManufacturerId,MedicineId,ImportDate,Origin,ExpiryDate,Quantity,Description")] ManufacturingBatch manufacturingBatch)
        {
            if (id != manufacturingBatch.ManufacturingBatchId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manufacturingBatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManufacturingBatchExists(manufacturingBatch.ManufacturingBatchId))
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
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "ManufacturerId", "Name", manufacturingBatch.ManufacturerId);
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "Name", manufacturingBatch.MedicineId);
            return View(manufacturingBatch);
        }

        // GET: ManufacturingBatches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manufacturingBatch = await _context.ManufacturingBatches
                .Include(m => m.Manufacturer)
                .Include(m => m.Medicine)
                .FirstOrDefaultAsync(m => m.ManufacturingBatchId == id);
            if (manufacturingBatch == null)
            {
                return NotFound();
            }

            return View(manufacturingBatch);
        }

        // POST: ManufacturingBatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manufacturingBatch = await _context.ManufacturingBatches.FindAsync(id);
            if (manufacturingBatch != null)
            {
                _context.ManufacturingBatches.Remove(manufacturingBatch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManufacturingBatchExists(int id)
        {
            return _context.ManufacturingBatches.Any(e => e.ManufacturingBatchId == id);
        }
    }
}
