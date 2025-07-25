using System;
using NLog;

namespace AcmeBankApp.Web.Helpers
{
    /// <summary>
    /// Legacy static logging helper - wraps NLog with static methods
    /// This is an anti-pattern but was common in 2018-2019
    /// </summary>
    public static class LogHelper
    {
        private static Logger _logger;

        public static void InitializeLogging()
        {
            _logger = LogManager.GetCurrentClassLogger();
            Info("Logging initialized");
        }

        #region Static Logging Methods (Anti-pattern)

        public static void Info(string message)
        {
            _logger?.Info(message);
        }

        public static void Debug(string message)
        {
            _logger?.Debug(message);
        }

        public static void Warn(string message)
        {
            _logger?.Warn(message);
        }

        public static void Error(string message)
        {
            _logger?.Error(message);
        }

        public static void Error(Exception exception, string message = "")
        {
            _logger?.Error(exception, message);
        }

        public static void Fatal(string message)
        {
            _logger?.Fatal(message);
        }

        #endregion

        #region Legacy Methods with Security Issues

        /// <summary>
        /// Legacy method that logs sensitive user information - security issue
        /// </summary>
        public static void LogUserActivity(string userName, string activity, string details = "")
        {
            // Security issue: Logging potentially sensitive user data
            Info($"User Activity - User: {userName}, Activity: {activity}, Details: {details}");
        }

        /// <summary>
        /// Legacy method that logs SQL queries with parameters - security issue
        /// </summary>
        public static void LogSqlQuery(string query, object parameters = null)
        {
            var logSensitiveData = System.Configuration.ConfigurationManager.AppSettings["LogSensitiveData"] == "true";
            
            if (logSensitiveData)
            {
                // Security issue: Logging SQL queries with potentially sensitive parameters
                Debug($"SQL Query: {query}, Parameters: {parameters}");
            }
        }

        /// <summary>
        /// Legacy method for logging authentication attempts
        /// </summary>
        public static void LogAuthenticationAttempt(string userName, bool success, string ipAddress)
        {
            var message = success ? 
                $"Successful login for user: {userName} from IP: {ipAddress}" :
                $"Failed login attempt for user: {userName} from IP: {ipAddress}";
            
            Info(message);
        }

        #endregion
    }
}
