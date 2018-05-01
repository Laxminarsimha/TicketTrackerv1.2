using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Security;
using System.Web.Http;
using System.Web.Mvc;

namespace MVC_Sample.Controllers
{
    public class LoginController : Controller
    {
        //[System.Web.Mvc.HttpPost]
        ////public string Login(Login data)
        ////{
        ////    bool isPasswordCorrect = false;
        ////    string un = data.Username;
        ////    string Password = data.Password;
        ////    using (SahilEntities entity = new SahilEntities())
        ////    {
        ////        var user = entity.Logins1.Where(u => u.UserName == un).FirstOrDefault();
        ////        if (user != null)
        ////        {
        ////            if (Password == user.Password)
        ////            {
        ////                Session["LoginID"] = user.ID;
        ////                Session["Username"] = user.Fname + ' ' + user.Lname;
        ////                return user.ID.ToString();
        ////            }
        ////            else
        ////            {
        ////                return "0";
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return "-1";
        ////        }
        ////    }
        ////}
    }
}