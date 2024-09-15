using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace MedicineStock.Models
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("Account").IsNullOrEmpty())
            {
                context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"Controller", "Account"},
                            {"Action", "Login"}
                        }
                    );
            }
        }
    }
}
