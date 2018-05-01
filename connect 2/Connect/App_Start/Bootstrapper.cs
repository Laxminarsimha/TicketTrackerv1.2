using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
//using Microsoft.Practices.Unity.Mvc;
using System.Web.Mvc;
using Connect.Services;
using Connect.Controllers;
using System.Web.Http;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;

namespace ProductTracking.App_Start
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<AccountController>(new InjectionConstructor());

            //service classes
            container.RegisterType<TimeService, TimeService>();
            container.RegisterType<SettingsService, SettingsService>();

            RegisterTypes(container);
            return container;
        }
        public static void RegisterTypes(IUnityContainer container)
        {

        }
    }
}