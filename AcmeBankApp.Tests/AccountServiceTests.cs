using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Core.Models;
using AcmeBankApp.Data.Services;

namespace AcmeBankApp.Tests
{
    /// <summary>
    /// Legacy unit tests for AccountService
    /// </summary>
    [TestClass]
    public class AccountServiceTests
    {
        private IAccountService _accountService;

        [TestInitialize] 
        public void Setup()
        {
            _accountService = new AccountService();
        }

        [TestMethod]
        public void GetAccountsByUserId_ValidUserId_ReturnsAccounts()
        {
            // Arrange
            int userId = 1; // Demo user

            // Act
            var accounts = _accountService.GetAccountsByUserId(userId);

            // Assert
            Assert.IsNotNull(accounts);
            Assert.IsTrue(accounts.Count > 0);
            Assert.IsTrue(accounts.All(a => a.UserId == userId));
        }

        [TestMethod]
        public void GetAccountsByUserId_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            int userId = 999; // Non-existent user

            // Act
            var accounts = _accountService.GetAccountsByUserId(userId);

            // Assert
            Assert.IsNotNull(accounts);
            Assert.AreEqual(0, accounts.Count);
        }

        [TestMethod]
        public void GetAccountById_ValidAccountId_ReturnsAccount()
        {
            // Arrange
            int accountId = 1; // Demo user's first account

            // Act
            var account = _accountService.GetAccountById(accountId);

            // Assert
            Assert.IsNotNull(account);
            Assert.AreEqual(accountId, account.AccountId);
        }

        [TestMethod]
        public void GetAccountById_InvalidAccountId_ReturnsNull()
        {
            // Arrange
            int accountId = 999; // Non-existent account

            // Act
            var account = _accountService.GetAccountById(accountId);

            // Assert
            Assert.IsNull(account);
        }

        [TestMethod]
        public void GetTransactionsByAccountId_ValidAccountId_ReturnsTransactions()
        {
            // Arrange
            int accountId = 1; // Demo user's account

            // Act
            var transactions = _accountService.GetTransactionsByAccountId(accountId);

            // Assert
            Assert.IsNotNull(transactions);
            // May or may not have transactions, but shouldn't be null
        }

        [TestMethod]
        public void GetAccountBalance_ValidAccountId_ReturnsBalance()
        {
            // Arrange
            int accountId = 1; // Demo user's account

            // Act
            decimal balance = _accountService.GetAccountBalance(accountId);

            // Assert
            Assert.IsTrue(balance >= 0); // Balance should be non-negative for demo account
        }

        [TestMethod]
        public void Transfer_ValidTransfer_CompletesSuccessfully()
        {
            // Arrange
            var transfer = new TransferModel
            {
                FromAccountId = 1, // Demo checking
                ToAccountId = 2,   // Demo savings
                Amount = 10.00m,   // Small amount for testing
                Description = "Unit test transfer"
            };

            // Get initial balances
            decimal initialFromBalance = _accountService.GetAccountBalance(transfer.FromAccountId);
            decimal initialToBalance = _accountService.GetAccountBalance(transfer.ToAccountId);

            // Act
            try
            {
                _accountService.Transfer(transfer);

                // Assert
                decimal finalFromBalance = _accountService.GetAccountBalance(transfer.FromAccountId);
                decimal finalToBalance = _accountService.GetAccountBalance(transfer.ToAccountId);

                Assert.AreEqual(initialFromBalance - transfer.Amount, finalFromBalance, "From balance should decrease by transfer amount");
                Assert.AreEqual(initialToBalance + transfer.Amount, finalToBalance, "To balance should increase by transfer amount");

                // Cleanup - reverse the transfer
                var reverseTransfer = new TransferModel
                {
                    FromAccountId = transfer.ToAccountId,
                    ToAccountId = transfer.FromAccountId,
                    Amount = transfer.Amount,
                    Description = "Unit test cleanup"
                };
                _accountService.Transfer(reverseTransfer);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Transfer failed: {ex.Message}");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))] // Legacy: Generic exception expectation
        public void Transfer_InvalidFromAccount_ThrowsException()
        {
            // Arrange
            var transfer = new TransferModel
            {
                FromAccountId = 999, // Non-existent account
                ToAccountId = 2,
                Amount = 10.00m,
                Description = "Invalid transfer test"
            };

            // Act - Should throw exception
            _accountService.Transfer(transfer);
        }

        #region Legacy Test Methods with Issues

        [TestMethod]
        public void TestAccountService_MultipleAssertions() // Legacy: Poor test naming
        {
            // Legacy: Multiple unrelated assertions in one test
            var accounts = _accountService.GetAccountsByUserId(1);
            Assert.IsNotNull(accounts);
            
            var account = _accountService.GetAccountById(1);
            Assert.IsNotNull(account);
            
            var balance = _accountService.GetAccountBalance(1);
            Assert.IsTrue(balance >= 0);
        }

        [TestMethod]
        public void AccountService_HardcodedValues_Test()
        {
            // Legacy: Test with hardcoded values that might break
            var account = _accountService.GetAccountById(1);
            Assert.AreEqual("1001234567", account.AccountNumber); // Hardcoded assumption
            Assert.AreEqual("Checking", account.AccountType);     // Hardcoded assumption
        }

        [TestMethod]
        [Ignore] // Legacy: Ignored test that should be fixed
        public void Transfer_LargeAmount_ShouldValidateLimit()
        {
            // This test was ignored because it's flaky or the feature isn't implemented
            var transfer = new TransferModel
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 1000000.00m, // Very large amount
                Description = "Large transfer test"
            };
            
            // Should validate against transfer limits but test is incomplete
            Assert.Fail("Test not implemented properly");
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            _accountService = null;
        }
    }
}
