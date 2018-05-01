using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Microsoft.Reporting.WebForms;
using System.IO;
using Connect.Models;
using Util;
using System.Collections;
using System.Web.Security;
using System.Web.Http;
using Connect.Controllers;
using System.Configuration;

namespace Connect.Reports
{
    [Admin]
    public partial class ReportViewer : System.Web.UI.Page
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public class QuestionDTO
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public string User { get; set; }
            public bool Important { get; set; }
            public int Rating { get; set; }
            public int Likes { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var session = db.Sessions.FirstOrDefault();

            var questions = new List<QuestionDTO>();
            foreach (var q in db.Questions.ToList())
            {
                questions.Add(new QuestionDTO()
                {
                    Id = q.Id,
                    Question = q.question,
                    Answer = q.answer,
                    Important = q.Important,
                    Rating = q.Rating,
                    User = q.createdBy.UserName,
                    Likes = q.Likes.Count
                });
            }

            var reportPath = Server.MapPath(@"Templates\Session.rdlc");
            //reportViewer.LocalReport.ReportPath = reportPath;


            //ReportDataSource questionsDs = new ReportDataSource();
            //questionsDs.Name = "QuestionsDataSet";
            //questionsDs.Value = questions.ToDataTable();
            //reportViewer.LocalReport.DataSources.Add(questionsDs);

            //ReportParameter[] reportParams = new ReportParameter[] {
            //    new ReportParameter("Date",session.StartTime.ToString("dd-MM-yyyy")),
            //    new ReportParameter("From",session.StartTime.ToString("hh:mm tt")),
            //    new ReportParameter("To",session.EndTime.ToString("hh:mm tt")),
            //    new ReportParameter("Logo",ConfigurationManager.AppSettings["REPORT_LOGO"]),
            //    new ReportParameter("TopDots",@"http://192.168.45.32/images/report_top.jpg")
            //};
            //reportViewer.LocalReport.EnableExternalImages = true;
            //reportViewer.LocalReport.EnableHyperlinks = true;
            //reportViewer.LocalReport.SetParameters(reportParams);
        }
    }
}