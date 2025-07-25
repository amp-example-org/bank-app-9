using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Core.Models;
using AcmeBankApp.Web.Helpers;

namespace AcmeBankApp.Web.Controllers
{
    /// <summary>
    /// Legacy API controller for Angular app - has multiple security issues
    /// </summary>
    public class ApiController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        // Parameterless constructor for when Autofac is disabled
        public ApiController() : this(null, null)
        {
        }

        public ApiController(IAccountService accountService, IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        #region Account Data API

        [HttpGet]
        public ActionResult GetAccounts()
        {
            // Legacy: Minimal authentication check
            if (Session["UserId"] == null)
            {
                return Json(new { error = "Not authenticated" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userId = (int)Session["UserId"];
                var accounts = _accountService.GetAccountsByUserId(userId);
                
                LogHelper.LogUserActivity(Session["UserName"]?.ToString(), "GetAccounts", $"Retrieved {accounts.Count} accounts");
                
                return Json(new { success = true, data = accounts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Failed to get accounts");
                return Json(new { error = "Failed to retrieve accounts" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTransactions(int accountId)
        {
            // Security issue: No ownership verification
            try
            {
                var transactions = _accountService.GetTransactionsByAccountId(accountId);
                
                LogHelper.LogUserActivity(Session["UserName"]?.ToString(), "GetTransactions", 
                    $"Retrieved transactions for account {accountId}");
                
                return Json(new { success = true, data = transactions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Failed to get transactions");
                return Json(new { error = "Failed to retrieve transactions" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetBalance(int accountId)
        {
            try
            {
                var balance = _accountService.GetAccountBalance(accountId);
                return Json(new { success = true, balance = balance }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Failed to get balance");
                return Json(new { error = "Failed to retrieve balance" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Transfer API

        [HttpPost]
        // Legacy: No [ValidateAntiForgeryToken] - CSRF vulnerability
        public ActionResult Transfer(TransferModel model)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { error = "Not authenticated" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { error = "Invalid transfer data" });
            }

            try
            {
                // Legacy: No authorization check - user could transfer from any account
                _accountService.Transfer(model);
                
                LogHelper.LogUserActivity(Session["UserName"]?.ToString(), "Transfer", 
                    $"Transferred ${model.Amount} from {model.FromAccountId} to {model.ToAccountId}");
                
                return Json(new { success = true, message = "Transfer completed successfully" });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Transfer failed");
                return Json(new { error = "Transfer failed. Please try again." });
            }
        }

        #endregion

        #region User Profile API

        [HttpGet]
        public ActionResult GetProfile()
        {
            if (Session["UserId"] == null)
            {
                return Json(new { error = "Not authenticated" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userId = (int)Session["UserId"];
                var user = _userService.GetUserById(userId);
                
                // Security issue: Returning sensitive information
                var profile = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.SecurityQuestion,
                    // Security issue: Even returning security answer
                    user.SecurityAnswer
                };
                
                return Json(new { success = true, data = profile }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Failed to get profile");
                return Json(new { error = "Failed to retrieve profile" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        // Legacy: No CSRF protection
        public ActionResult UpdateProfile(User model)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { error = "Not authenticated" });
            }

            try
            {
                model.UserId = (int)Session["UserId"];
                _userService.UpdateUser(model);
                
                LogHelper.LogUserActivity(Session["UserName"]?.ToString(), "ProfileUpdate", "Profile updated via API");
                
                return Json(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Profile update failed");
                return Json(new { error = "Failed to update profile" });
            }
        }

        #endregion

        #region Legacy Utility APIs

        /// <summary>
        /// Legacy API that exposes system configuration - security issue
        /// </summary>
        [HttpGet]
        public ActionResult GetConfig()
        {
            var config = new
            {
                MaxTransferAmount = System.Configuration.ConfigurationManager.AppSettings["MaxTransferAmount"],
                SessionTimeout = System.Configuration.ConfigurationManager.AppSettings["SessionTimeoutMinutes"],
                // Security issue: Exposing API keys
                BankApiKey = System.Configuration.ConfigurationManager.AppSettings["BankApiKey"],
                PaymentServiceUrl = System.Configuration.ConfigurationManager.AppSettings["PaymentServiceUrl"]
            };
            
            LogHelper.Info("Configuration accessed via API");
            return Json(config, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Legacy search API with SQL injection vulnerability
        /// </summary>
        [HttpGet]
        public ActionResult SearchTransactions(string query)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { error = "Not authenticated" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Security issue: Direct SQL injection vulnerability
                var sql = $"SELECT * FROM Transactions WHERE Description LIKE '%{query}%'";
                var results = AcmeBankApp.Data.DataHelper.ExecuteQuery(sql);
                
                LogHelper.LogUserActivity(Session["UserName"]?.ToString(), "SearchTransactions", 
                    $"Searched for: {query}");
                
                return Json(new { success = true, data = results }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Search failed");
                return Json(new { error = "Search failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
