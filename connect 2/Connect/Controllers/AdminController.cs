using Connect.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util;

namespace Connect.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.totalQuestions = db.Questions.Count();

                var topRatedQuestions = (from q in db.Questions
                                         group q by q.Rating into groupByRating
                                         orderby groupByRating.Key descending
                                         select new { rating = groupByRating.Key <= 0 ? "No Rating" : groupByRating.Key + " Rating", count = groupByRating.Count() });
                ViewBag.topRatedQuestions = new ArrayList();
                foreach (var entry in topRatedQuestions)
                    ViewBag.topRatedQuestions.Add(new { user = entry.rating, count = entry.count }.ToExpando());

                var topGoodQuestions = (from q in db.Questions
                                        where q.Important == true
                                        group q by q.createdBy into groupByUser
                                        orderby groupByUser.Count() descending
                                        select new { user = groupByUser.Key, count = groupByUser.Count() }).Take(5);
                ViewBag.topGoodQuestions = new ArrayList();
                foreach (var entry in topGoodQuestions)
                    ViewBag.topGoodQuestions.Add(new { user = entry.user.UserName, count = entry.count }.ToExpando());

                var topUsersAsked = (from q in db.Questions
                                     group q by q.createdBy into groupByUsers
                                     orderby groupByUsers.Count() descending
                                     select new { user = groupByUsers.Key, count = groupByUsers.Count() }).Take(5);
                ViewBag.groupByUsersList = new ArrayList();
                foreach (var entry in topUsersAsked)
                    ViewBag.groupByUsersList.Add(new { user = entry.user.UserName, count = entry.count }.ToExpando());

                var topUsersGotAnswers = (from q in db.Questions
                                          where q.answer != null
                                          group q by q.createdBy into groupByUsers
                                          orderby groupByUsers.Count() descending
                                          select new { user = groupByUsers.Key, count = groupByUsers.Count() }).Take(5);
                ViewBag.topUsersGotAnswers = new ArrayList();
                foreach (var entry in topUsersGotAnswers)
                    ViewBag.topUsersGotAnswers.Add(new { user = entry.user.UserName, count = entry.count }.ToExpando());

                var session = db.Sessions.FirstOrDefault();
                ViewBag.session = session;
            }
            return View();
        }

        public ActionResult DayWiseCount()
        {
            ArrayList dayWiseCount = new ArrayList();
            DateTime today = DateTime.Now;
            dayWiseCount.Add(new ArrayList { "Date", "Posted", "Answered" });
            for (int i = 15; i >= 0; i--)
            {
                DateTime beforeDate = DateTime.Now.AddDays(-i);
                int postedCount = (from t in db.Questions
                                   where DbFunctions.TruncateTime(t.dateCreated) == DbFunctions.TruncateTime(beforeDate)
                                   select t).Count();
                int answeredCount = (from t in db.Questions
                                     where DbFunctions.TruncateTime(t.dateAnswered) == DbFunctions.TruncateTime(beforeDate)
                                     select t).Count();
                dayWiseCount.Add(new ArrayList { beforeDate.ToString("dd-MM"), postedCount, answeredCount });
            }
            JsonResult jsonResult = new JsonResult();
            jsonResult.Data = dayWiseCount;
            return jsonResult;
        }
    }
}