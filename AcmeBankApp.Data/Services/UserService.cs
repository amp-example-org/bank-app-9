using System;
using System.Linq;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Core.Models;

namespace AcmeBankApp.Data.Services
{
    public class UserService : IUserService
    {
        // Legacy anti-pattern - creating new context for each operation
        public User GetUserByUserName(string userName)
        {
            using (var context = new AcmeBankContext())
            {
                return context.Users.FirstOrDefault(u => u.UserName == userName);
            }
        }

        public User GetUserById(int userId)
        {
            using (var context = new AcmeBankContext())
            {
                return context.Users.Find(userId);
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            // Legacy pattern - multiple ways to validate users
            // Sometimes using EF, sometimes raw ADO.NET
            var useModernAuth = System.Configuration.ConfigurationManager.AppSettings["UseModernAuth"] == "true";
            
            if (useModernAuth)
            {
                using (var context = new AcmeBankContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.UserName == userName && u.IsActive);
                    if (user != null)
                    {
                        // Legacy: Plain text password comparison (security issue)
                        return user.PasswordHash == password;
                    }
                }
            }
            else
            {
                // Fallback to legacy method with SQL injection vulnerability
                return DataHelper.ValidateUserLegacy(userName, password);
            }
            
            return false;
        }

        public void CreateUser(User user)
        {
            using (var context = new AcmeBankContext())
            {
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;
                // Legacy: Store password as plain text hash
                user.PasswordHash = user.PasswordHash;
                
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public void UpdateUser(User user)
        {
            using (var context = new AcmeBankContext())
            {
                var existingUser = context.Users.Find(user.UserId);
                if (existingUser != null)
                {
                    existingUser.Email = user.Email;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.SecurityQuestion = user.SecurityQuestion;
                    existingUser.SecurityAnswer = user.SecurityAnswer;
                    
                    context.SaveChanges();
                }
            }
        }

        public void UpdateLastLogin(int userId)
        {
            using (var context = new AcmeBankContext())
            {
                var user = context.Users.Find(userId);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }
    }
}
