using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MedicineStock.Models
{
    public class CheckPermisson : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // if not login -> redirect to login page
            if (!context.HttpContext.Session.GetString("Account").IsNullOrEmpty())
            {
                var sessionAccount = context.HttpContext.Session.GetString("Account");
                var loggedAccount = JsonConvert.DeserializeObject<Dictionary<string, object>>(sessionAccount);

                // if account exceed the permission -> redirect to dashboard 
                if ((int)(long)loggedAccount["PermissionId"] != 1)
                {
                    context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                            {"Controller", "Home"},
                            {"Action", "Index"}
                            }
                        );
                }                
            }            
        }
    }
}
