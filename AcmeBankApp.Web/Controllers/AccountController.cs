using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Core.Models;
using AcmeBankApp.Web.Helpers;

namespace AcmeBankApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        // Parameterless constructor for when Autofac is disabled
        public AccountController() : this(null)
        {
        }

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        #region Authentication Actions

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            
            // Legacy: Pass sensitive information to view
            ViewBag.MaxFailedAttempts = System.Configuration.ConfigurationManager.AppSettings["MaxFailedLoginAttempts"];
            
            return View(new LoginModel());
        }

        [HttpPost]
        // Legacy: No [ValidateAntiForgeryToken] - CSRF vulnerability
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Legacy: Get client IP for logging (but method is unreliable)
                string clientIP = Request.UserHostAddress;
                
                // Legacy authentication with security issues - Demo mode
                bool isValid = (model.UserName == "demo" && model.Password == "password123") ||
                              (model.UserName == "jdoe" && model.Password == "mypassword") ||
                              (model.UserName == "admin" && model.Password == "admin123");
                
                if (isValid)
                {
                    // Create demo user object
                    var user = new User 
                    { 
                        UserId = 1, 
                        UserName = model.UserName, 
                        FirstName = model.UserName == "demo" ? "Demo" : model.UserName == "jdoe" ? "John" : "Admin",
                        LastName = model.UserName == "demo" ? "User" : model.UserName == "jdoe" ? "Doe" : "User"
                    };
                    
                    // Legacy: Store sensitive user info in session
                    Session["UserId"] = user.UserId;
                    Session["UserName"] = user.UserName;
                    Session["FullName"] = $"{user.FirstName} {user.LastName}";
                    
                    // Legacy Forms Authentication
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    
                    // Update last login (disabled for demo)
                    // _userService.UpdateLastLogin(user.UserId);
                    
                    // Legacy logging with sensitive data
                    LogHelper.LogAuthenticationAttempt(model.UserName, true, clientIP);
                    LogHelper.LogUserActivity(model.UserName, "Login", $"LoginTime: {DateTime.Now}");
                    
                    // Legacy redirect handling
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        // Redirect to the Angular SPA route - use relative path for virtual directory
                        return Redirect("~/app");
                    }
                }
                else
                {
                    // Legacy: Log failed attempts with potentially sensitive info
                    LogHelper.LogAuthenticationAttempt(model.UserName, false, clientIP);
                    
                    // Legacy: Generic error message (good practice, but inconsistent with other parts)
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            return PerformLogout();
        }

        [HttpGet]
        public ActionResult Logout(string confirm)
        {
            return PerformLogout();
        }

        private ActionResult PerformLogout()
        {
            var userName = Session["UserName"]?.ToString() ?? "Unknown";
            
            // Legacy session cleanup
            Session.Clear();
            Session.Abandon();
            
            FormsAuthentication.SignOut();
            
            LogHelper.LogUserActivity(userName, "Logout", $"LogoutTime: {DateTime.Now}");
            
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Registration (Legacy)

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        // Legacy: No CSRF protection
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Legacy: Store password as plain text hash (security issue)
                    model.PasswordHash = model.PasswordHash; // Assumes it's already "hashed"
                    
                    _userService.CreateUser(model);
                    
                    LogHelper.LogUserActivity(model.UserName, "Registration", "New user registered");
                    
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Legacy error handling
                    LogHelper.Error(ex, "User registration failed");
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                }
            }

            return View(model);
        }

        #endregion

        #region Legacy Profile Management

        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            var userId = (int)Session["UserId"];
            var user = _userService.GetUserById(userId);
            
            return View(user);
        }

        [HttpPost]
        // Legacy: No CSRF protection
        public ActionResult Profile(User model)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.UserId = (int)Session["UserId"];
                    _userService.UpdateUser(model);
                    
                    LogHelper.LogUserActivity(model.UserName, "ProfileUpdate", "User profile updated");
                    
                    ViewBag.Message = "Profile updated successfully!";
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "Profile update failed");
                    ModelState.AddModelError("", "Profile update failed. Please try again.");
                }
            }

            return View(model);
        }

        #endregion

        #region Legacy Utility Methods

        /// <summary>
        /// Legacy action that exposes user information via GET - security issue
        /// </summary>
        [HttpGet]
        public ActionResult GetUserInfo(string userName)
        {
            // Security issue: No authorization check, exposes user data via GET
            var user = _userService.GetUserByUserName(userName);
            
            if (user != null)
            {
                var userInfo = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.CreatedDate,
                    user.LastLogin,
                    // Security issue: Exposing security question/answer
                    user.SecurityQuestion,
                    user.SecurityAnswer
                };
                
                LogHelper.LogUserActivity("System", "UserInfoAccess", $"User info accessed for: {userName}");
                
                return Json(userInfo, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { error = "User not found" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
