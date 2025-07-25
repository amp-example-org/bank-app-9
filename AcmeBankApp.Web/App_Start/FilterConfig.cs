using System.Web.Mvc;

namespace AcmeBankApp.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
            // Legacy: No [ValidateAntiForgeryToken] filter applied globally
            // This is a security issue - CSRF attacks are possible
            
            // Legacy: No [RequireHttps] filter for production
            // Another security issue
        }
    }
}
