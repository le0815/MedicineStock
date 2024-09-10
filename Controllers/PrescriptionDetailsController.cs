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
    public class PrescriptionDetailsController : Controller
    {
        private readonly MedicineStockContext _context;

        public PrescriptionDetailsController(MedicineStockContext context)
        {
            _context = context;
        }

        // GET: PrescriptionDetails
        public async Task<IActionResult> Index()
        {
            var medicineStockContext = _context.PrescriptionDetails.Include(p => p.Medicine).Include(p => p.Prescription);
            return View(await medicineStockContext.ToListAsync());
        }

        // GET: PrescriptionDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescriptionDetail = await _context.PrescriptionDetails
                .Include(p => p.Medicine)
                .Include(p => p.Prescription)
                .FirstOrDefaultAsync(m => m.PrescriptionDetailId == id);
            if (prescriptionDetail == null)
            {
                return NotFound();
            }

            return View(prescriptionDetail);
        }

        // GET: PrescriptionDetails/Create
        public IActionResult Create()
        {
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "MedicineId");
            ViewData["PrescriptionId"] = new SelectList(_context.Prescriptions, "PrescriptionId", "PrescriptionId");
            return View();
        }

        // POST: PrescriptionDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrescriptionDetailId,PrescriptionId,MedicineId,Quantity")] PrescriptionDetail prescriptionDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prescriptionDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "MedicineId", prescriptionDetail.MedicineId);
            ViewData["PrescriptionId"] = new SelectList(_context.Prescriptions, "PrescriptionId", "PrescriptionId", prescriptionDetail.PrescriptionId);
            return View(prescriptionDetail);
        }

        // GET: PrescriptionDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescriptionDetail = await _context.PrescriptionDetails.FindAsync(id);
            if (prescriptionDetail == null)
            {
                return NotFound();
            }
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "MedicineId", prescriptionDetail.MedicineId);
            ViewData["PrescriptionId"] = new SelectList(_context.Prescriptions, "PrescriptionId", "PrescriptionId", prescriptionDetail.PrescriptionId);
            return View(prescriptionDetail);
        }

        // POST: PrescriptionDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrescriptionDetailId,PrescriptionId,MedicineId,Quantity")] PrescriptionDetail prescriptionDetail)
        {
            if (id != prescriptionDetail.PrescriptionDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescriptionDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionDetailExists(prescriptionDetail.PrescriptionDetailId))
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
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "MedicineId", "MedicineId", prescriptionDetail.MedicineId);
            ViewData["PrescriptionId"] = new SelectList(_context.Prescriptions, "PrescriptionId", "PrescriptionId", prescriptionDetail.PrescriptionId);
            return View(prescriptionDetail);
        }

        // GET: PrescriptionDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescriptionDetail = await _context.PrescriptionDetails
                .Include(p => p.Medicine)
                .Include(p => p.Prescription)
                .FirstOrDefaultAsync(m => m.PrescriptionDetailId == id);
            if (prescriptionDetail == null)
            {
                return NotFound();
            }

            return View(prescriptionDetail);
        }

        // POST: PrescriptionDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescriptionDetail = await _context.PrescriptionDetails.FindAsync(id);
            if (prescriptionDetail != null)
            {
                _context.PrescriptionDetails.Remove(prescriptionDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionDetailExists(int id)
        {
            return _context.PrescriptionDetails.Any(e => e.PrescriptionDetailId == id);
        }
    }
}
