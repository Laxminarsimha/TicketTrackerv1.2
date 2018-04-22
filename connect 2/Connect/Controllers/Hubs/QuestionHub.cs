using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Connect.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Connect.Services;

namespace Connect.Controllers.Hubs
{
    public class QuestionHub : Hub
    {
        static HashSet<string> users = new HashSet<string>();
        ApplicationDbContext db = new ApplicationDbContext();

        public void Questions()
        {
            string msg = "<script type='text/javascript'>questions();</script>";
            Clients.All.questions(msg);
        }

        public void Answers()
        {
            string msg = "<script type='text/javascript'>answers();</script>";
            Clients.All.answers(msg);
        }

        public void Users()
        {
            Clients.All.users(users.Count);
        }

        public void Qcount()
        {
            Clients.All.qcount(String.Empty);
        }
        public void Broadcast()
        {
            Clients.All.qcount(String.Empty);
        }
        public void SessionExtend()
        {
            string msg = "<script type='text/javascript'>loadSessionTime();</script>";
            Clients.All.answers(msg);
        }
        public void ReadOnly()
        {
            Clients.All.answers(TimeService.READ_ONLY);
        }

        public override Task OnConnected()
        {
            users.Add(Context.User.Identity.GetUserId());
            Clients.All.users(users.Count);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            users.Remove(Context.User.Identity.GetUserId());
            Clients.All.users(users.Count);
            return base.OnDisconnected(stopCalled);
        }
    }
}