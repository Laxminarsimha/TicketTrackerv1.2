using Connect.Controllers.Hubs;
using Connect.Services;
using FluentScheduler;
using Microsoft.AspNet.SignalR;
using System.Web.Hosting;

namespace Connect.Jobs
{
    public class SessionReadOnlyTriggerJob : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        public SessionReadOnlyTriggerJob()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;
                if (!TimeService.IsValidPostingTime())
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<QuestionHub>();
                    context.Clients.All.readOnly("ReadOnly");
                }
            }
        }

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
            HostingEnvironment.UnregisterObject(this);
        }
    }
}