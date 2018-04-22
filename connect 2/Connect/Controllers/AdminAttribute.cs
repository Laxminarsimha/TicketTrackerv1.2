using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Connect.Controllers
{
    public class AdminAttribute : Attribute
    {
        public AdminAttribute()
        {
            var user = HttpContext.Current.User;

            if (user.IsInRole("Admin"))
                return;

            HttpContext.Current.Response.RedirectToRoute("Account/Login");
        }
    }
}