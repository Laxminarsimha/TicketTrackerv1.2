using Connect.Models;
using Connect.Models.Core;
using Connect.Models.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Connect.Controllers.Hubs;
using System.Text;
using System.IO;
using System.Web.UI;
using Util;
using System.Net;
using Connect.Services;

namespace Connect.Controllers
{
    [TimeFilter]
    [System.Web.Http.Authorize]
    public class QuestionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Post(string text)
        {
            if (!TimeService.IsValidPostingTime())
                return Content(TimeService.READ_ONLY);
            if (text.Length > 2 && User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(currentUserId);
                Question question = new Question { question = text, dateCreated = DateTime.Now, createdBy = user };
                db.Entry(question).State = System.Data.Entity.EntityState.Added;
                db.Questions.Add(question);
                db.SaveChanges();
                var lastPostedQuestion = PartialView("~/Views/Question/_Question.cshtml", question).RenderToString();

                var context = GlobalHost.ConnectionManager.GetHubContext<QuestionHub>();
                context.Clients.All.questions(lastPostedQuestion);

                context.Clients.All.qcount(Json(AnsweredCount()));

                return new HttpStatusCodeResult(200, "Question Added");
            }
            return new HttpStatusCodeResult(400, "Not valid");
        }

        public ActionResult Reply(int id, string answer, bool important, int rating)
        {
            if (!string.IsNullOrEmpty(answer) && User.Identity.IsAuthenticated)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                Question question = db.Questions.Find(id);
                question.answer = answer;
                question.answeredBy = user;
                question.Important = important;
                question.Rating = rating;
                question.dateAnswered = DateTime.Now;
                db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                db.Entry(question).Reference(q => q.createdBy).Load();
                db.SaveChanges();
                var context = GlobalHost.ConnectionManager.GetHubContext<QuestionHub>();
                ViewBag.questions = new List<Question>() { question };

                var lastAnsweredQuestion = PartialView("~/Views/Question/_Answers.cshtml").RenderToString();
                context.Clients.All.answers(lastAnsweredQuestion);

                context.Clients.All.qcount(Json(AnsweredCount()));

                return new HttpStatusCodeResult(200, "Replay Added");
            }
            return new HttpStatusCodeResult(400, "Not valid");
        }

        public ActionResult MyQuestions()
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                string currentUserId = User.Identity.GetUserId();
                var questions = (from q in db.Questions where q.createdBy.Id == currentUserId orderby q.dateCreated descending select q).ToList();
                ViewBag.questions = questions;
                return PartialView("~/Views/Question/_Answers.cshtml");
            }
            return new HttpStatusCodeResult(400, "Not valid");
        }

        public ActionResult Similar(string text)
        {
            var questions = db.Questions.Where(q => q.question.ToLower().StartsWith(text.ToLower())).ToList();
            ViewBag.questions = questions;
            return PartialView("~/Views/Question/_Questions.cshtml");
        }

        public ActionResult Questions(int? previous)
        {
            var questions = (from q in db.Questions
                             where q.answer == null && q.Id > (previous ?? 0)
                             orderby q.Id descending
                             select q).ToList();
            var recentQuestion = questions.FirstOrDefault();
            ViewBag.questions = questions;
            ViewBag.recentQuestionIndex = recentQuestion != null ? recentQuestion.Id : 0;
            return PartialView("~/Views/Question/_Questions.cshtml");
        }

        public ActionResult Answers()
        {
            var questions = (from q in db.Questions
                             where q.answer != null
                             orderby q.dateAnswered descending
                             select q).Take(25).ToList();
            ViewBag.questions = questions;
            return PartialView("~/Views/Question/_Answers.cshtml");
        }

        public ActionResult respondEditor(int id)
        {
            ViewBag.question = db.Questions.Find(id);
            return PartialView("~/Views/Question/_respondEditor.cshtml");
        }

        public ActionResult UnAnsCount()
        {
            return Json(UnAnsweredCount());
        }

        public ActionResult AnsCount()
        {
            return Json(AnsweredCount());
        }
        public ActionResult Broadcast(string msg)
        {
            GlobalHost.ConnectionManager.GetHubContext<QuestionHub>().Clients.All.broadcast(msg);
            return Content("Message broadcasted.");
        }

        private List<int> AnsweredCount()
        {
            int answered = (from q in db.Questions where q.answer != null select q).Count();
            int total = db.Questions.Count();
            return new List<int> { answered, total };
        }
        private List<int> UnAnsweredCount()
        {
            int answered = (from q in db.Questions where q.answer == null select q).Count();
            int total = db.Questions.Count();
            return new List<int> { answered, total };
        }

        public ActionResult Phrases()
        {
            ViewBag.phrases = db.Phrases.OrderByDescending(p => p.Id).ToList();
            return PartialView("~/Views/Question/_phrases.cshtml");
        }
        public ActionResult AddPhrase(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                var existingPhrase = db.Phrases.Where(p => p.Text.Equals(text, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (existingPhrase == null)
                {
                    Phrase phrase = new Phrase { Text = text };
                    db.Phrases.Add(phrase);
                    db.Entry(phrase).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                }
                return Content("Added");
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Like(int id)
        {
            string currentUserId = User.Identity.GetUserId();
            Question question = db.Questions.Find(id);
            if (!String.IsNullOrEmpty(currentUserId) && question != null && question.createdBy.Id != currentUserId)
            {
                var alreadyLiked = (from l in db.Likes where l.User.Id == currentUserId && l.Question.Id == question.Id select l).FirstOrDefault();
                if (alreadyLiked == null)
                {
                    Like like = new Like() { UserId = currentUserId, Question = question };
                    db.Entry(like).State = System.Data.Entity.EntityState.Added;
                    db.Likes.Add(like);
                    db.SaveChanges();
                }
                var likeCount = (from l in db.Likes where l.Question.Id == question.Id select l).Count();
                return Content(likeCount.ToString());
            }
            return new HttpStatusCodeResult(400, "Not valid");
        }
    }
}