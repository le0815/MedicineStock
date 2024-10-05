using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicineStock.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        [Authentication]
        public async Task<IActionResult> Index()
        {
            //var medicineStockContext = _context.Prescriptions.Include(p => p.Patient);

            // viewdata for display price, quantiy of medicine on view module
            ViewData["Medicines"] = await _context.Medicines.ToListAsync();
            ViewData["ManufacturingBatches"] = await _context.ManufacturingBatches.ToListAsync();

            var medicineStockContext = _context.Prescriptions.Include(q => q.PrescriptionDetails);

            return View(await medicineStockContext.ToListAsync());
        }

        // GET: Prescriptions/Details/5
        [Authentication]
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
            ViewData["ManufacturingBatches"] = await _context.ManufacturingBatches.ToListAsync();

            return View(prescriptions); 
        }

        // GET: Prescriptions/Create
        [Authentication]
        public async Task<IActionResult> Create()
        {

            var model = await _context.ManufacturingBatches.Include(q => q.Medicine).Include(q => q.Manufacturer).ToListAsync();
            ViewData["MedicineTypes"] = await _context.MedicineTypes.ToListAsync();

            return View(model);
        }

        // POST: Prescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrescriptionId,PrescriptionDate")] Prescription prescription, [Bind("ManufacturingBatchId,MedicineId,Quantity,Description")] List<PrescriptionDetail> prescriptionDetail, List<int> selectedItems)
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
                    .Where(q => selectedItems.Contains((int)q.ManufacturingBatchId))
                    .ToList();
                
                foreach (var item in selectedPrescriptionDetails)
                {
                    item.PrescriptionId = temp_id;
                }

                // then save prescriptionDetail
                await _context.AddRangeAsync(selectedPrescriptionDetails);
                await _context.SaveChangesAsync();

                //update medicine quantity
                var manufacturingBatchesUpdate = await _context.ManufacturingBatches.Where(q => selectedItems.Contains((int)q.MedicineId)).ToListAsync();

                foreach (var item in manufacturingBatchesUpdate)
                {
                    foreach(var item2 in selectedPrescriptionDetails)
                    {
                        if (item.ManufacturingBatchId == item2.ManufacturingBatchId)
                        {
                            // if item selected with 0 quantity -> continue
                            if(item2.Quantity == null) { continue; }

                            item.Quantity -= item2.Quantity;
                        }
                    }
                }
                // then save updated medicine quantity in manufacturing batch
                _context.UpdateRange(manufacturingBatchesUpdate);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            
            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", prescription.PatientId);
            return View(prescription);
        }

        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowPreView([Bind("PrescriptionId,PrescriptionDate")] Prescription prescription, [Bind("ManufacturingBatchId,MedicineId,Quantity,Description")] List<PrescriptionDetail> prescriptionDetail, List<int> selectedItems)
        {
            if (TryValidateModel(prescription) && TryValidateModel(prescriptionDetail))
            {
                                

                // filter prescriptionDetail based on selectedItems
                var selectedPrescriptionDetails = prescriptionDetail
                    .Where(q => selectedItems.Contains((int)q.ManufacturingBatchId))
                    .ToList();

                var model = await _context.ManufacturingBatches.Where(q => selectedItems.Contains((int)q.ManufacturingBatchId)).ToListAsync();
               
                
                // viewdata for display price, quantiy of medicine on view module                
                ViewData["Medicines"] = await _context.Medicines.ToListAsync();
                ViewData["selectedPrescriptionDetails"] = selectedPrescriptionDetails;

                return PartialView("_DetailsInvoicePreView", model);
            }

            //ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Name", prescription.PatientId);
            return View("Index");
        }

        // GET: Prescriptions/Edit/5
        [Authentication]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var model = new PrescriptionViewModel
            //{
            //    Prescription = new Prescription(),
            //    PrescriptionDetail = new PrescriptionDetail(),
            //    Medicines = await _context.Medicines.Include(j => j.MedicineType).ToListAsync(),
            //    prescriptionDetail = await _context.PrescriptionDetails.Where(q => q.PrescriptionId == id).ToListAsync()
            //};

            // add id to prescription
            //model.Prescription.PrescriptionId = (int)id;
            
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // viewdata for display price, quantiy of medicine on view module
            ViewData["Medicines"] = await _context.Medicines.ToListAsync();
            ViewData["ManufacturingBatches"] = await _context.ManufacturingBatches.ToListAsync();
            ViewData["Manufacturers"] = await _context.Manufacturers.ToListAsync();
            ViewData["MedicineTypes"] = await _context.MedicineTypes.ToListAsync();
            ViewData["PrescriptionDetails"] = await _context.PrescriptionDetails.ToListAsync();

            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrescriptionId,PrescriptionDate")] Prescription prescription, [Bind("PrescriptionDetailId,PrescriptionId,ManufacturingBatchId,Quantity,Description")] List<PrescriptionDetail> prescriptionDetail, List<int> selectedItems)
        {
            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }

            if (TryValidateModel(prescription) && TryValidateModel(prescriptionDetail))
            {
                try
                {
                    //_context.Update(prescription);
                    //await _context.SaveChangesAsync();

                    //if medicine was unselected from prescriptionDetail -> remove
                    try
                    {
                        var oldPrescriptionDetail = await _context.PrescriptionDetails.Where(p => p.PrescriptionId == id).ToListAsync();                       
                        var manufacturingBatchesUpdate = await _context.ManufacturingBatches.ToListAsync();
                        // filter prescriptionDetail based on selectedItems
                        var selectedPrescriptionDetails = prescriptionDetail
                            .Where(q => selectedItems.Contains((int)q.ManufacturingBatchId))
                            .ToList();



                        //var editSelectedPrescriptionDetails = oldPrescriptionDetail.Where(q => selectedPrescriptionDetails.Any(t => t.ManufacturingBatchId == q.ManufacturingBatchId)).ToList();
                        var editSelectedPrescriptionDetails = selectedPrescriptionDetails.Where(q => oldPrescriptionDetail.Any(t => t.ManufacturingBatchId == q.ManufacturingBatchId)).ToList();
                        //if medicine was edited from prescriptionDetail -> update
                        if (!editSelectedPrescriptionDetails.IsNullOrEmpty())
                        {
                            //var oldQuantity = 0;
                            //foreach (var item in editSelectedPrescriptionDetails)
                            //{
                            //    foreach (var item1 in selectedPrescriptionDetails)
                            //    {
                            //        if(item.ManufacturingBatchId == item1.ManufacturingBatchId)
                            //        {
                            //            item.Quantity = item1.Quantity;
                            //            break;
                            //        }
                            //    }
                            //}
                            //_context.UpdateRange(editSelectedPrescriptionDetails);
                            //await _context.SaveChangesAsync();

                            // update medicine quantiy after update
                            foreach (var item in manufacturingBatchesUpdate)
                            {
                                foreach (var item2 in editSelectedPrescriptionDetails)
                                {
                                    if (item.ManufacturingBatchId == item2.ManufacturingBatchId)
                                    {
                                        foreach (var item3 in oldPrescriptionDetail)
                                        {
                                            if (item3.ManufacturingBatchId == item.ManufacturingBatchId)
                                            {
                                                var diffQuantity = item3.Quantity - item2.Quantity;
                                                item.Quantity += diffQuantity;
                                                item3.Quantity = item2.Quantity;
                                                break;
                                            }
                                        }                                        
                                        break;
                                    }
                                }
                            }
                            // then save updated medicine quantity in manufacturing batch
                            _context.UpdateRange(manufacturingBatchesUpdate);
                            _context.UpdateRange(oldPrescriptionDetail);
                            await _context.SaveChangesAsync();
                        }

                        var nonSelectedPrescriptionDetails = oldPrescriptionDetail
                        .Where(q => !selectedItems.Contains((int)q.ManufacturingBatchId))
                        .ToList();

                        //if medicine was unselected from prescriptionDetail -> remove
                        if (!nonSelectedPrescriptionDetails.IsNullOrEmpty())
                        {
                            _context.RemoveRange(nonSelectedPrescriptionDetails);
                            await _context.SaveChangesAsync();

                            // update medicine quantiy after remove
                            

                            foreach (var item in manufacturingBatchesUpdate)
                            {
                                foreach (var item2 in nonSelectedPrescriptionDetails)
                                {
                                    if (item.ManufacturingBatchId == item2.ManufacturingBatchId)
                                    {
                                        item.Quantity += item2.Quantity;
                                        break;
                                    }
                                }
                            }
                            // then save updated medicine quantity in manufacturing batch
                            _context.UpdateRange(manufacturingBatchesUpdate);
                            await _context.SaveChangesAsync();
                        }
                        

                        var newSelectedPrescriptionDetails = selectedPrescriptionDetails.Where(q => !oldPrescriptionDetail.Any(t => t.ManufacturingBatchId == q.ManufacturingBatchId)).ToList();

                        //if medicine was new selected from prescriptionDetail -> add
                        if (!newSelectedPrescriptionDetails.IsNullOrEmpty())
                        {

                            await _context.AddRangeAsync(newSelectedPrescriptionDetails);
                            await _context.SaveChangesAsync();

                            // update medicine quantiy after add                            
                            foreach (var item in manufacturingBatchesUpdate)
                            {
                                foreach (var item2 in newSelectedPrescriptionDetails)
                                {
                                    if (item.ManufacturingBatchId == item2.ManufacturingBatchId)
                                    {
                                        item.Quantity -= item2.Quantity;
                                        break;
                                    }
                                }
                            }
                            // then save updated medicine quantity in manufacturing batch
                            _context.UpdateRange(manufacturingBatchesUpdate);
                            await _context.SaveChangesAsync();
                        }

                    }
                    catch
                    {
                        throw;
                    }
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
        [Authentication]        
        public async Task<IActionResult> Delete(int? id)
        {                    
            
            if (id == null)
            {
                return NotFound();
            }

            //var prescription = await _context.Prescriptions
            //    .Include(p => p.Patient)
            //    .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            var prescriptions = await _context.Prescriptions.Include(q => q.PrescriptionDetails).FirstOrDefaultAsync(m => m.PrescriptionId == id);

            if (prescriptions == null)
            {
                return NotFound();
            }

            // viewdata for display price, quantiy of medicine on view module
            ViewData["Medicines"] = await _context.Medicines.ToListAsync();
            ViewData["ManufacturingBatches"] = await _context.ManufacturingBatches.ToListAsync();

            return View(prescriptions);
        }

        // POST: Prescriptions/Delete/5
        [Authentication]        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                _context.Prescriptions.Remove(prescription);

                //also delete prescriptionDetails
                var prescriptionDetail = await _context.PrescriptionDetails.Where(q => q.PrescriptionId.Value == id).ToListAsync();
                _context.RemoveRange(prescriptionDetail);

                //update quantity after remove
                var manufacturingBatchesUpdate = await _context.ManufacturingBatches.ToListAsync();
                foreach (var item in manufacturingBatchesUpdate)
                {
                    foreach (var item2 in prescriptionDetail)
                    {
                        if (item.ManufacturingBatchId == item2.ManufacturingBatchId)
                        {
                            // if item selected with 0 quantity -> continue
                            if (item2.Quantity == null) { continue; }

                            item.Quantity += item2.Quantity;
                            break;
                        }
                    }
                }
                _context.UpdateRange(manufacturingBatchesUpdate);
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


