using Connect.Services;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using Unity.Attributes;

namespace Connect.Controllers
{
    class ControllerAction
    {
        public string controller { get; set; }
        public string action { get; set; }
    }
    public class TimeFilterAttribute : ActionFilterAttribute
    {


        static List<ControllerAction> BLOCKED_LIST = new List<ControllerAction>()
        {
            new  ControllerAction{controller= "Home", action="Index" },
            new  ControllerAction{controller= "Home", action="Qestion" },
            new  ControllerAction{controller=  "Index", action = "Post" }
        };

        [Dependency("TimeService")]
        public TimeService TimeService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (!TimeService.ValidTime())
            {
                foreach (var blocked in BLOCKED_LIST)
                {
                    if (String.Equals(controllerName, blocked.controller, StringComparison.InvariantCultureIgnoreCase) &&
                        String.Equals(actionName, blocked.action, StringComparison.InvariantCultureIgnoreCase))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                        new
                        {
                            controller = "Time",
                            action = "Index"
                        }));

                    }
                }
            }
        }
    }
}