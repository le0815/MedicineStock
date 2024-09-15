using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MedicineStock.Views.Shared
{
    public class _LayoutModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public _LayoutModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Dictionary<string, object> LoggedAccount { get; private set; }

        public IActionResult OnGet()
        {
            var sessionAccount = _httpContextAccessor.HttpContext.Session.GetString("Account");
            if (sessionAccount == null)
            {
                Console.WriteLine("session account is null");
                return RedirectToAction("Login", "Account");
            }

            // Convert to JSON
            LoggedAccount = JsonConvert.DeserializeObject<Dictionary<string, object>>(sessionAccount);
            return Page();
        }
    }
}
