using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace MVC_Sample.Controllers.Hubs
{
    public class DashboardHub:Hub
    {
        static HashSet<string> MyPMETicketsCount = new HashSet<string>();

        public void MyPMETickets()
        {
            Clients.All.MyPMETicketsCount(MyPMETicketsCount.Count);
        }

        public void Broadcast()
        {
            Clients.All.qcount(String.Empty);
        }


    }
}