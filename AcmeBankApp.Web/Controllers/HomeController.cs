using System.Web.Mvc;
using AcmeBankApp.Web.Helpers;

namespace AcmeBankApp.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Legacy MVC Actions

        public ActionResult Index()
        {
            LogHelper.Info("Home page accessed");
            
            // Legacy: Pass sensitive config data to view
            ViewBag.AppVersion = "1.0.0";
            ViewBag.Environment = System.Configuration.ConfigurationManager.AppSettings["Environment"] ?? "Production";
            ViewBag.BankApiKey = System.Configuration.ConfigurationManager.AppSettings["BankApiKey"]; // Security issue
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Acme Bank - Legacy Banking Application";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Acme Bank Support";
            return View();
        }

        #endregion

        #region Legacy Error Handling

        public ActionResult Error()
        {
            // Legacy: Expose detailed error information
            var error = TempData["ErrorMessage"]?.ToString() ?? "An unknown error occurred";
            ViewBag.ErrorDetails = error;
            
            LogHelper.Error($"Error page displayed: {error}");
            return View();
        }

        #endregion

        #region Legacy Utility Actions

        /// <summary>
        /// Legacy action that exposes system information - security issue
        /// </summary>
        public ActionResult SystemInfo()
        {
            var info = new
            {
                ServerTime = System.DateTime.Now,
                MachineName = System.Environment.MachineName,
                UserName = System.Environment.UserName,
                OSVersion = System.Environment.OSVersion.ToString(),
                CLRVersion = System.Environment.Version.ToString(),
                WorkingSet = System.Environment.WorkingSet,
                ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
            };
            
            LogHelper.Info("System info accessed");
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
