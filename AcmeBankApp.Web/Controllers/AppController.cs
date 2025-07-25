using System.Web.Mvc;
using AcmeBankApp.Web.Helpers;

namespace AcmeBankApp.Web.Controllers
{
    /// <summary>
    /// Controller that hosts the Angular 7 SPA
    /// </summary>
    public class AppController : Controller
    {
        public ActionResult Index()
        {
            // Debug: Log the request details
            System.Diagnostics.Debug.WriteLine($"App.Index called: {Request.Url}");
            
            // Legacy: Basic authentication check
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Pass user information to Angular app via ViewBag
            ViewBag.UserId = Session["UserId"];
            ViewBag.UserName = Session["UserName"];
            ViewBag.FullName = Session["FullName"];
            
            // Legacy: Pass configuration to client-side
            ViewBag.MaxTransferAmount = System.Configuration.ConfigurationManager.AppSettings["MaxTransferAmount"];
            ViewBag.SessionTimeout = System.Configuration.ConfigurationManager.AppSettings["SessionTimeoutMinutes"];
            
            // Security issue: Pass API key to client
            ViewBag.ApiKey = System.Configuration.ConfigurationManager.AppSettings["BankApiKey"];
            
            // LogHelper.LogUserActivity(Session["UserName"]?.ToString(), "AngularAppAccess", "Accessed Angular SPA");
            
            return View();
        }

        // Test action for debugging
        public ActionResult Test()
        {
            return Content("App Controller is working! Route: " + Request.Url);
        }
    }
}
