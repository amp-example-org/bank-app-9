using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcmeBankApp.Data;

namespace AcmeBankApp.Tests
{
    /// <summary>
    /// Legacy tests for the DataHelper class - demonstrates testing of vulnerable code
    /// </summary>
    [TestClass]
    public class LegacyDataHelperTests
    {
        #region Legacy SQL Injection Tests (Testing Vulnerable Code)

        [TestMethod]
        public void ExecuteQuery_SimpleSelect_ReturnsData()
        {
            // Arrange
            string sql = "SELECT TOP 1 * FROM Users WHERE UserName = 'demo'";

            // Act
            DataTable result = DataHelper.ExecuteQuery(sql);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count > 0);
        }

        [TestMethod]
        public void ExecuteQuery_InvalidSql_ReturnsEmptyTable()
        {
            // Arrange
            string sql = "SELECT * FROM NonExistentTable";

            // Act
            DataTable result = DataHelper.ExecuteQuery(sql);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Rows.Count); // Returns empty table due to exception swallowing
        }

        [TestMethod]
        public void ValidateUserLegacy_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            string userName = "demo";
            string password = "password123";

            // Act
            bool result = DataHelper.ValidateUserLegacy(userName, password);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateUserLegacy_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            string userName = "demo";
            string password = "wrongpassword";

            // Act
            bool result = DataHelper.ValidateUserLegacy(userName, password);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetUserByUserNameLegacy_ValidUser_ReturnsData()
        {
            // Arrange
            string userName = "demo";

            // Act
            DataTable result = DataHelper.GetUserByUserNameLegacy(userName);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count > 0);
            Assert.AreEqual(userName, result.Rows[0]["UserName"].ToString());
        }

        [TestMethod]
        public void GetUserByUserNameLegacy_InvalidUser_ReturnsEmptyTable()
        {
            // Arrange
            string userName = "nonexistentuser";

            // Act
            DataTable result = DataHelper.GetUserByUserNameLegacy(userName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Rows.Count);
        }

        #endregion

        #region SQL Injection Vulnerability Tests (Educational)

        [TestMethod]
        [TestCategory("SecurityVulnerability")]
        public void ValidateUserLegacy_SqlInjection_DemonstratesVulnerability()
        {
            // This test demonstrates the SQL injection vulnerability
            // WARNING: This is for educational purposes only!
            
            // Arrange - SQL injection attempt
            string userName = "demo'; --";
            string password = "anything";

            // Act
            bool result = DataHelper.ValidateUserLegacy(userName, password);

            // Assert
            // This may return true due to SQL injection vulnerability
            // The injected SQL becomes: SELECT COUNT(*) FROM Users WHERE UserName = 'demo'; --' AND PasswordHash = 'anything'
            // The -- comments out the password check
            Console.WriteLine($"SQL Injection test result: {result}");
        }

        [TestMethod]
        [TestCategory("SecurityVulnerability")]
        public void GetUserByUserNameLegacy_SqlInjection_ReturnsUnexpectedData()
        {
            // This test demonstrates another SQL injection vulnerability
            
            // Arrange - SQL injection to return all users
            string maliciousInput = "demo' OR '1'='1";

            // Act
            DataTable result = DataHelper.GetUserByUserNameLegacy(maliciousInput);

            // Assert
            Assert.IsNotNull(result);
            // Due to SQL injection, this might return all users instead of just one
            Console.WriteLine($"Returned {result.Rows.Count} rows (expected 0-1)");
        }

        #endregion

        #region Parameterized Query Tests

        [TestMethod]
        public void ExecuteParameterizedQuery_WithParameters_ReturnsData()
        {
            // Arrange
            string sql = "SELECT * FROM Users WHERE UserName = @UserName";
            var parameters = new System.Data.SqlClient.SqlParameter[]
            {
                new System.Data.SqlClient.SqlParameter("@UserName", "demo")
            };

            // Act
            DataTable result = DataHelper.ExecuteParameterizedQuery(sql, parameters);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Rows.Count > 0);
        }

        [TestMethod]
        public void ExecuteParameterizedQuery_NoParameters_ReturnsData()
        {
            // Arrange
            string sql = "SELECT TOP 5 * FROM Users";

            // Act
            DataTable result = DataHelper.ExecuteParameterizedQuery(sql);

            // Assert
            Assert.IsNotNull(result);
        }

        #endregion

        #region Legacy Test Patterns

        [TestMethod]
        public void ExecuteScalar_Count_ReturnsNumber()
        {
            // Arrange
            string sql = "SELECT COUNT(*) FROM Users";

            // Act
            object result = DataHelper.ExecuteScalar(sql);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int));
            Assert.IsTrue((int)result > 0);
        }

        [TestMethod]
        public void ExecuteNonQuery_Update_ReturnsRowsAffected()
        {
            // Arrange - Update that shouldn't actually change anything
            string sql = "UPDATE Users SET LastLogin = LastLogin WHERE UserId = 999"; // Non-existent user

            // Act
            int result = DataHelper.ExecuteNonQuery(sql);

            // Assert
            Assert.AreEqual(0, result); // Should affect 0 rows
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))] // Legacy: Generic exception
        public void ExecuteScalar_InvalidSql_ThrowsException()
        {
            // Arrange
            string sql = "SELECT INVALID FROM NonExistentTable";

            // Act - Should throw exception
            DataHelper.ExecuteScalar(sql);
        }

        #endregion

        #region Legacy Performance Tests (Flawed)

        [TestMethod]
        [TestCategory("Performance")]
        public void DataHelper_PerformanceTest_Legacy()
        {
            // Legacy: Simple performance test without proper measurement
            var startTime = DateTime.Now;
            
            for (int i = 0; i < 10; i++)
            {
                var result = DataHelper.ExecuteQuery("SELECT TOP 1 * FROM Users");
                Assert.IsNotNull(result);
            }
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            
            Console.WriteLine($"Executed 10 queries in {duration.TotalMilliseconds}ms");
            
            // Legacy: No meaningful assertion about performance
            Assert.IsTrue(duration.TotalSeconds < 30, "Queries took too long");
        }

        #endregion
    }
}
