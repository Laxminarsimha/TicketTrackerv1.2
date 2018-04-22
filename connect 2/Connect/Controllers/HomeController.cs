using Connect.Models;
using Connect.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Connect.Controllers
{
    [Authorize]
    [TimeFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.AllowPosting = TimeService.IsValidPostingTime();
            if (User.IsInRole("Respond"))
                return RedirectToAction("Respond");
            return View();
        }

        [Authorize(Roles = "Respond")]
        public ActionResult Respond()
        {
            return View();
        }
    }
}