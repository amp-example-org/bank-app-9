using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Data.Services;
using AcmeBankApp.Web.Helpers;

namespace AcmeBankApp.Web.App_Start
{
    public static class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register services
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerRequest();

            // Build container
            var container = builder.Build();
            
            // Set MVC dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            
            // Legacy anti-pattern: Also set up Service Locator for static access
            ServiceLocator.SetContainer(container);
            
            LogHelper.Info("Autofac container configured successfully");
        }
    }
}
