using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Core.Models;
using AcmeBankApp.Data.Services;

namespace AcmeBankApp.Tests
{
    /// <summary>
    /// Legacy unit tests for UserService - demonstrates typical 2018 testing practices
    /// </summary>
    [TestClass]
    public class UserServiceTests
    {
        private IUserService _userService;

        [TestInitialize]
        public void Setup()
        {
            // Legacy: Direct instantiation instead of DI container
            _userService = new UserService();
        }

        [TestMethod]
        public void GetUserByUserName_ValidUser_ReturnsUser()
        {
            // Arrange
            string userName = "demo";

            // Act
            var result = _userService.GetUserByUserName(userName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result.UserName);
            Assert.AreEqual("Demo", result.FirstName);
        }

        [TestMethod]
        public void GetUserByUserName_InvalidUser_ReturnsNull()
        {
            // Arrange
            string userName = "nonexistent";

            // Act
            var result = _userService.GetUserByUserName(userName);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValidateUser_CorrectCredentials_ReturnsTrue()
        {
            // Arrange
            string userName = "demo";
            string password = "password123"; // Legacy: Plain text password

            // Act
            bool result = _userService.ValidateUser(userName, password);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateUser_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            string userName = "demo";
            string password = "wrongpassword";

            // Act
            bool result = _userService.ValidateUser(userName, password);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateUser_IncorrectUserName_ReturnsFalse()
        {
            // Arrange
            string userName = "nonexistent";
            string password = "password123";

            // Act
            bool result = _userService.ValidateUser(userName, password);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetUserById_ValidId_ReturnsUser()
        {
            // Arrange
            int userId = 1; // Assuming demo user has ID 1

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual("demo", result.UserName);
        }

        [TestMethod]
        public void GetUserById_InvalidId_ReturnsNull()
        {
            // Arrange
            int userId = 999; // Non-existent ID

            // Act
            var result = _userService.GetUserById(userId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CreateUser_ValidUser_CreatesSuccessfully()
        {
            // Arrange
            var newUser = new User
            {
                UserName = "testuser" + DateTime.Now.Ticks, // Ensure unique
                Email = "test" + DateTime.Now.Ticks + "@test.com",
                PasswordHash = "testpassword", // Legacy: Plain text
                FirstName = "Test",
                LastName = "User",
                SecurityQuestion = "Test question?",
                SecurityAnswer = "Test answer"
            };

            // Act & Assert - No exception should be thrown
            try
            {
                _userService.CreateUser(newUser);
                
                // Verify user was created
                var createdUser = _userService.GetUserByUserName(newUser.UserName);
                Assert.IsNotNull(createdUser);
                Assert.AreEqual(newUser.UserName, createdUser.UserName);
                Assert.AreEqual(newUser.Email, createdUser.Email);
            }
            catch (Exception ex)
            {
                Assert.Fail($"User creation failed: {ex.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))] // Legacy: Not specific exception type
        public void CreateUser_DuplicateUserName_ThrowsException()
        {
            // Arrange
            var duplicateUser = new User
            {
                UserName = "demo", // Existing username
                Email = "duplicate@test.com",
                PasswordHash = "testpassword",
                FirstName = "Duplicate",
                LastName = "User"
            };

            // Act - Should throw exception
            _userService.CreateUser(duplicateUser);
        }

        [TestMethod]
        public void UpdateUser_ValidUser_UpdatesSuccessfully()
        {
            // Arrange
            var user = _userService.GetUserByUserName("demo");
            Assert.IsNotNull(user, "Demo user should exist for this test");
            
            var originalFirstName = user.FirstName;
            user.FirstName = "Updated Demo";

            // Act
            _userService.UpdateUser(user);

            // Assert
            var updatedUser = _userService.GetUserById(user.UserId);
            Assert.AreEqual("Updated Demo", updatedUser.FirstName);
            
            // Cleanup - restore original name
            user.FirstName = originalFirstName;
            _userService.UpdateUser(user);
        }

        [TestMethod]
        public void UpdateLastLogin_ValidUserId_UpdatesSuccessfully()
        {
            // Arrange
            var user = _userService.GetUserByUserName("demo");
            Assert.IsNotNull(user, "Demo user should exist for this test");
            
            var originalLastLogin = user.LastLogin;

            // Act
            _userService.UpdateLastLogin(user.UserId);

            // Assert
            var updatedUser = _userService.GetUserById(user.UserId);
            Assert.IsNotNull(updatedUser.LastLogin);
            Assert.IsTrue(updatedUser.LastLogin > originalLastLogin);
        }

        #region Legacy Test Patterns

        [TestMethod]
        public void TestMethod_Legacy_Naming_Convention()
        {
            // Legacy: Poor test naming and single assertion
            var user = _userService.GetUserByUserName("demo");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [Ignore] // Legacy: Tests ignored instead of fixed
        public void BrokenTest_ShouldBeFixed()
        {
            // This test would fail but is ignored instead of being fixed
            Assert.AreEqual(1, 2, "This test is broken but ignored");
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            // Legacy: Basic cleanup
            _userService = null;
        }
    }
}
