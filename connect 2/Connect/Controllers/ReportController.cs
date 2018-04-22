using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Connect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return Redirect("~/Reports/ReportViewer.aspx");
        }
    }
}