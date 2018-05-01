using Connect.Models;
using Connect.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Connect.Services
{
    public class TimeService
    {
        public static readonly string READ_ONLY = "ReadOnly";

        public bool ValidTime()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //var session = db.Sessions.FirstOrDefault();
            //if (session != null)
            //{
            //    var now = DateTime.Now;
            //    if ((now > session.StartTime) && (now < session.EndTime))
            //        return true;
            //    return false;
            //}
            return false;
        }
        static public bool IsValidPostingTime()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //var session = db.Sessions.FirstOrDefault();
                //if (session != null)
                //    return DateTime.Now < session.PostingLimitTime;
            }
            return false;
        }
        public Session CurrentSession()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
                return db.Sessions.FirstOrDefault();
        }
        public bool IsToday(Session session)
        {
            return session.StartTime.Date == DateTime.Today;
        }
        public int Duration()
        {
            var session = CurrentSession();
            if (session != null && IsToday(session))
            {
                return (int)(session.EndTime - DateTime.Now).TotalSeconds;
            }
            return -1;
        }
    }
}