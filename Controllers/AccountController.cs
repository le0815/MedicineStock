using MedicineStock.Models;
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
                TempData["Message"] = "Invalid Username or Password";

            }            
            return View();
        }

        public IActionResult logout()
        {
            // remove account info session
            HttpContext.Session.Remove("Account");

            return View("Login");
        }

        [Authentication]        
        public async Task<IActionResult> Index()
        {
            //if user is not admin -> only manage current account
            var sessionAccount = HttpContext.Session.GetString("Account");            
            // convert to json 
            var loggedAccount = JsonConvert.DeserializeObject<Dictionary<string, object>>(sessionAccount);

            if ((int)(long)loggedAccount["PermissionId"] != 1)
            {
                ViewData["AccountId"] = loggedAccount["AccountId"];
                return RedirectToAction("Details", new { id = loggedAccount["AccountId"] });
            }

            //var medicineStockContext1 = _context.Medicines.Include(m => m.Manufacturer).Include(m => m.MedicineType);
            var medicineStockContext1 = _context.Accounts.Include(m => m.Permission);
            return View(await medicineStockContext1.ToListAsync());
        }

        [Authentication]
        [CheckPermisson]
        public async Task<IActionResult> Create()
        {
            //var medicineStockContext1 = _context.Medicines.Include(m => m.Manufacturer).Include(m => m.MedicineType);
            //var medicineStockContext1 = _context.Accounts.Include(m => m.Permission);
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name");
            //ViewData["Accounts"] = await _context.Accounts.ToListAsync();
            return View();
        }

        [Authentication]
        [CheckPermisson]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Password,PermissionId")] Account account)
        {
            if (ModelState.IsValid)
            {
                // check userName if exist in database
                var usrName = _context.Accounts.Where(q => q.UserName == account.UserName).FirstOrDefault();
                if (usrName != null)
                {
                    ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name", account.PermissionId);
                    return View(account);
                }
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name", account.PermissionId);
            return View(account);
        }

        [Authentication]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            ViewData["Permissions"] = await _context.Permissions.ToListAsync();
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Medicines/Edit/5
        [Authentication]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name", account.PermissionId);
            return View(account);
        }

        // POST: Medicines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId", "UserName,Password,PermissionId")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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

            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name", account.PermissionId);
            return View(account);
            //return await Index();
        }


        [Authentication]
        [CheckPermisson]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            ViewData["Permissions"] = await _context.Permissions.ToListAsync();
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authentication]
        [CheckPermisson]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {

            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            

            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Name");
            return View();
        }
        private bool AccountExists(int id)
        {
            return _context.Medicines.Any(e => e.MedicineId == id);
        }
    }
}
