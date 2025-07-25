using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AcmeBankApp.Web.App_Start;
using AcmeBankApp.Web.Helpers;

namespace AcmeBankApp.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Legacy Autofac setup
            try
            {
                // Initialize legacy logging first
                LogHelper.InitializeLogging();
                LogHelper.Info("AcmeBankApp starting up...");
                
                AutofacConfig.ConfigureContainer();
                LogHelper.Info("Startup completed successfully (Autofac disabled)");
            }
            catch (Exception ex)
            {
                // Fallback logging if LogHelper fails
                System.Diagnostics.Debug.WriteLine("Startup error: " + ex.ToString());
                throw;
            }
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            if (exception != null)
            {
                // Legacy error handling - logs sensitive information
                LogHelper.Error($"Unhandled exception: {exception.ToString()}");
                
                // Legacy: Don't clear the error, let it bubble up with full details
                // Server.ClearError();
            }
        }

        protected void Session_Start()
        {
            // Legacy session initialization
            LogHelper.Info($"New session started: {Session.SessionID}");
        }

        protected void Session_End()
        {
            // Legacy session cleanup
            LogHelper.Info($"Session ended: {Session.SessionID}");
        }
    }
}
