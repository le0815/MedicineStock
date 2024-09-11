using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicineStock.Models;
using Microsoft.IdentityModel.Tokens;

namespace MedicineStock.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly MedicineStockContext _context;

        public PrescriptionsController(MedicineStockContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index(string searchPhrase = null)
        {
            //var medicineStockContext = _context.Prescriptions.Include(p => p.Patient);

            // viewdata for display price, quantiy of medicine on view module
            ViewData["Medicines"] = await _context.Medicines.ToListAsync();

            var medicineStockContext = _context.Prescriptions.Include(q => q.PrescriptionDetails);

            if (string.IsNullOrEmpty(searchPhrase))
            {
                return View(await medicineStockContext.ToListAsync());
            }

            //var temp = medicineStockContext.Where(q => q.Patient.Name.Contains(searchPhrase));
            var temp = medicineStockContext;
            return View(await temp.ToListAsync());
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var prescription = await _context.Prescriptions
            //    .Include(p => p.Doctor)
            //    .Include(p => p.Patient)
            //    .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            //var Prescription = await _context.Prescriptions.Include(q => q.Patient).Include(q => q.PrescriptionDetails).Where(m => m.PrescriptionId == id).ToListAsync();
            var prescriptions = await _context.Prescriptions.Include(q => q.PrescriptionDetails).FirstOrDefaultAsync(m => m.PrescriptionId == id);
			//var Prescription = _context.Prescriptions.Include(q => q.Doctor).Include(q => q.Patient).Include(q => q.PrescriptionDetails).Where(m => m.PrescriptionId == id);

			if (prescriptions == null)
			{
				return NotFound();
			}

			//var model = new PrescriptionViewModel
			//{
			//    prescription = Prescription
			//};            

			// viewdata for display price, quantiy of medicine on view module
			ViewData["Medicines"] = await _context.Medicines.ToListAsync();

			return View(prescriptions); 
        }

        // GET: Prescriptions/Create
        public async Task<IActionResult> Create()
        {
            //var model = new PrescriptionViewModel
            //{
            //    Prescription = new Prescription(),
            //    PrescriptionDetail = new PrescriptionDetail(),
            //    Medicines = await _context.Medicines.Include(j => j.MedicineType).ToListAsync(),                
            //};

            var model = await _context.ManufacturingBatches.Include(q => q.Medicine).Include(q => q.Manufacturer).ToListAsync();


            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name");
            ViewData["MedicineTypes"] = await _context.MedicineTypes.ToListAsync();

            return View(model);
        }

        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrescriptionId,PrescriptionDate")] Prescription prescription, [Bind("ManufacturingBatchId,MedicineId,Quantity")] List<PrescriptionDetail> prescriptionDetail, List<int> selectedItems)
        {
            if (TryValidateModel(prescription) && TryValidateModel(prescriptionDetail))
            {
                // save prescription to database first
                _context.Add(prescription);
                await _context.SaveChangesAsync();


                // get latest id of prescription
                var temp_id = _context.Prescriptions.OrderByDescending(t => t.PrescriptionId).FirstOrDefault().PrescriptionId;

                // filter prescriptionDetail based on selectedItems
                var selectedPrescriptionDetails = prescriptionDetail
                    .Where(q => selectedItems.Contains((int)q.MedicineId))
                    .ToList();

                foreach (var item in selectedPrescriptionDetails)
                {
                    item.PrescriptionId = temp_id;
                }

                // then save prescriptionDetail
                await _context.AddRangeAsync(selectedPrescriptionDetails);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            
            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", prescription.PatientId);
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = new PrescriptionViewModel
            {
                Prescription = new Prescription(),
                PrescriptionDetail = new PrescriptionDetail(),
                Medicines = await _context.Medicines.Include(j => j.MedicineType).ToListAsync(),
                prescriptionDetail = await _context.PrescriptionDetails.Where(q => q.PrescriptionId == id).ToListAsync()
            };

            // add id to prescription
            model.Prescription.PrescriptionId = (int)id;
            
            //var prescription = await _context.Prescriptions.FindAsync(id);
            //if (prescription == null)
            //{
            //    return NotFound();
            //}
            
            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", model.Prescription.PatientId);
            return View(model);
        }

        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrescriptionId,DoctorId,PatientId,PrescriptionDate")] Prescription prescription, [Bind("PrescriptionDetailId,PrescriptionId,MedicineId,Quantity")] List<PrescriptionDetail> prescriptionDetail, List<int> selectedItems)
        {
            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }

            if (TryValidateModel(prescription) && TryValidateModel(prescriptionDetail))
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();

                    //if medicine was unselected from prescriptionDetail -> remove
                    try
                    {
                        Console.WriteLine("removing unselected item");
                        var nonSelectedPrescriptionDetails = prescriptionDetail
                        .Where(q => !selectedItems.Contains((int)q.MedicineId))
                        .ToList();

                        _context.PrescriptionDetails.RemoveRange(nonSelectedPrescriptionDetails);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        throw;
                    }

                    //if medicine are new -> add to prescriptionDetail
                    try
                    {
                        Console.WriteLine("adding selected item");                        

                        //foreach (var item in selectedItems)
                        //{
                        //    if (prescriptionDetail.Find(q => q.MedicineId.Equals(item)))
                        //    {

                        //    }
                        //}
                        //var newSelectedPrescriptionDetails = selectedItems.Where(q => !prescriptionDetail.Contains(q));
                        
                        //foreach (var item in selectedPrescriptionDetails)
                        //{
                        //    item.PrescriptionId = id;
                        //}
                    }
                    catch
                    {
                        throw;
                    }

                    



                    //// then save prescriptionDetail
                    //_context.UpdateRange(selectedPrescriptionDetails);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.PrescriptionId))
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
            
            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", prescription.PatientId);
            return View(prescription);
        }

        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var prescription = await _context.Prescriptions
            //    .Include(p => p.Patient)
            //    .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            var prescription = await _context.Prescriptions                
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                _context.Prescriptions.Remove(prescription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.PrescriptionId == id);
        }
    }
}


