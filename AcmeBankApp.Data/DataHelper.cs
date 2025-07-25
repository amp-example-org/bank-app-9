using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace AcmeBankApp.Data
{
    /// <summary>
    /// Legacy static data helper class - contains security vulnerabilities typical of 2018 era
    /// </summary>
    public static class DataHelper
    {
        private static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
        }

        #region Legacy ADO.NET Methods with Security Issues

        /// <summary>
        /// WARNING: This method is vulnerable to SQL injection - kept for legacy compatibility
        /// </summary>
        public static DataTable ExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                // Legacy pattern - swallow exceptions and return empty DataTable
                // TODO: Should log this properly
                dt = new DataTable();
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dt;
        }

        /// <summary>
        /// Slightly better method that uses parameters - but still has issues
        /// </summary>
        public static DataTable ExecuteParameterizedQuery(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    conn.Open();
                    adapter.Fill(dt);
                }
                catch
                {
                    // Still swallowing exceptions - legacy pattern
                    return new DataTable();
                }
            }
            return dt;
        }

        /// <summary>
        /// Execute non-query with SQL injection vulnerability - legacy method
        /// </summary>
        public static int ExecuteNonQuery(string sql)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Legacy logging - writes sensitive data to log files
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sql} - {ex.Message}");
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Get scalar value - also vulnerable
        /// </summary>
        public static object ExecuteScalar(string sql)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        #endregion

        #region Legacy Helper Methods

        /// <summary>
        /// Legacy method to validate user - contains SQL injection vulnerability
        /// This is how authentication was often done in 2018 before proper practices
        /// </summary>
        public static bool ValidateUserLegacy(string userName, string password)
        {
            // SECURITY ISSUE: Direct string concatenation allows SQL injection
            string sql = $"SELECT COUNT(*) FROM Users WHERE UserName = '{userName}' AND PasswordHash = '{password}'";
            object result = ExecuteScalar(sql);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Get user by username - legacy version with SQL injection
        /// </summary>
        public static DataTable GetUserByUserNameLegacy(string userName)
        {
            // Another SQL injection vulnerability
            string sql = $"SELECT * FROM Users WHERE UserName = '{userName}'";
            return ExecuteQuery(sql);
        }

        #endregion
    }
}
