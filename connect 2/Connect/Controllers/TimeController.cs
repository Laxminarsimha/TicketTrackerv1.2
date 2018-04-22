using Connect.Controllers.Hubs;
using Connect.Models;
using Connect.Models.Core;
using Connect.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Connect.Controllers
{
    public class TimeController : Controller
    {
        [Dependency("TimeService")]
        public TimeService TimeService { get; set; }
        [Dependency("SettingsService")]
        public SettingsService SettingsService { get; set; }

        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            Models.Core.Session session = TimeService.CurrentSession(); 
            if (session != null && TimeService.IsToday(session))
            {
                var countdownSeconds = session.StartTime.Subtract(DateTime.Now).TotalSeconds;
                if (session.StartTime <= DateTime.Now && DateTime.Now <= session.EndTime)
                {
                    return RedirectToAction("index", "home");
                }
                if (countdownSeconds > 0)
                {
                    ViewBag.currentSession = session;
                    ViewBag.countdownSeconds = countdownSeconds;
                }
                else
                {
                    var msg = SettingsService.GetSettings().SessionCompleteMessage;
                    ViewBag.msg = String.IsNullOrEmpty(msg) ? "Session completed . ." : msg;
                }
            }
            else
            {
                var msg = SettingsService.GetSettings()?.NoSessionsToday;
                ViewBag.msg = String.IsNullOrEmpty(msg) ? "No sessions scheduled for today" : msg;
            }
            return View();
        }
        public ActionResult Duration()
        {
            return Json(TimeService.Duration());
        }

        public ActionResult ExtendSession(int minutes)
        {
            var session = TimeService.CurrentSession();
            if (session != null)
            {
                session.EndTime = session.EndTime.AddMinutes(minutes);
                db.Entry(session).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.sessionExtend(@"<script type='text/javascript'>loadSessionTime();</script>");

                return Content("Session Time Updated");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public ActionResult SessionTime(string from, string to)
        {
            if (!String.IsNullOrEmpty(from) && !String.IsNullOrEmpty(to))
            {
                DateTime fromTime = Convert.ToDateTime(from);
                DateTime toTime = Convert.ToDateTime(to);

                DateTime fromTimeStamp = DateTime.Now.Date + new TimeSpan(fromTime.Hour, fromTime.Minute, 00);
                DateTime toTimeStamp = DateTime.Now.Date + new TimeSpan(toTime.Hour, toTime.Minute, 00);

                Session session = db.Sessions.FirstOrDefault();
                if (session == null)
                {
                    session = new Models.Core.Session() { StartTime = fromTimeStamp, EndTime = toTimeStamp };
                    db.Entry(session).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    session.StartTime = fromTimeStamp;
                    session.EndTime = toTimeStamp;
                    db.Entry(session).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                return Content("Session timings updated");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        }
    }
}