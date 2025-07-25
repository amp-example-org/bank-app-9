using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AcmeBankApp.Core.Interfaces;
using AcmeBankApp.Core.Models;

namespace AcmeBankApp.Data.Services
{
    public class AccountService : IAccountService
    {
        public List<Account> GetAccountsByUserId(int userId)
        {
            using (var context = new AcmeBankContext())
            {
                return context.Accounts
                    .Where(a => a.UserId == userId && a.IsActive)
                    .ToList();
            }
        }

        public Account GetAccountById(int accountId)
        {
            using (var context = new AcmeBankContext())
            {
                return context.Accounts.Find(accountId);
            }
        }

        public List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            // Legacy pattern - mix of EF and raw ADO.NET in same service
            var useLegacyQuery = System.Configuration.ConfigurationManager.AppSettings["UseLegacyTransactionQuery"] == "true";
            
            if (useLegacyQuery)
            {
                return GetTransactionsLegacy(accountId);
            }
            else
            {
                using (var context = new AcmeBankContext())
                {
                    return context.Transactions
                        .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(100) // Legacy: hardcoded limit
                        .ToList();
                }
            }
        }

        private List<Transaction> GetTransactionsLegacy(int accountId)
        {
            var transactions = new List<Transaction>();
            
            // Legacy ADO.NET query with potential SQL injection
            string sql = $"SELECT TOP 100 * FROM Transactions WHERE FromAccountId = {accountId} OR ToAccountId = {accountId} ORDER BY TransactionDate DESC";
            
            DataTable dt = DataHelper.ExecuteQuery(sql);
            
            foreach (DataRow row in dt.Rows)
            {
                transactions.Add(new Transaction
                {
                    TransactionId = Convert.ToInt32(row["TransactionId"]),
                    FromAccountId = Convert.ToInt32(row["FromAccountId"]),
                    ToAccountId = row["ToAccountId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ToAccountId"]),
                    TransactionType = row["TransactionType"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    Description = row["Description"].ToString(),
                    TransactionDate = Convert.ToDateTime(row["TransactionDate"]),
                    Status = row["Status"].ToString(),
                    InternalNotes = row["InternalNotes"].ToString()
                });
            }
            
            return transactions;
        }

        public void Transfer(TransferModel transfer)
        {
            // Legacy transaction handling without proper error handling
            using (var context = new AcmeBankContext())
            {
                var fromAccount = context.Accounts.Find(transfer.FromAccountId);
                var toAccount = context.Accounts.Find(transfer.ToAccountId);
                
                if (fromAccount != null && toAccount != null)
                {
                    // Legacy: No transaction isolation, potential race conditions
                    fromAccount.Balance -= transfer.Amount;
                    toAccount.Balance += transfer.Amount;
                    
                    var transaction = new Transaction
                    {
                        FromAccountId = transfer.FromAccountId,
                        ToAccountId = transfer.ToAccountId,
                        TransactionType = "Transfer",
                        Amount = transfer.Amount,
                        Description = transfer.Description,
                        TransactionDate = DateTime.Now,
                        Status = "Completed",
                        InternalNotes = $"Transfer from {fromAccount.AccountNumber} to {toAccount.AccountNumber}" // Potential data leak
                    };
                    
                    context.Transactions.Add(transaction);
                    context.SaveChanges(); // No transaction scope - can lead to inconsistent state
                }
            }
        }

        public decimal GetAccountBalance(int accountId)
        {
            // Legacy: Sometimes use EF, sometimes raw SQL for same operation
            var useRawSql = DateTime.Now.Millisecond % 2 == 0; // Random choice - typical legacy inconsistency
            
            if (useRawSql)
            {
                string sql = $"SELECT Balance FROM Accounts WHERE AccountId = {accountId}";
                object result = DataHelper.ExecuteScalar(sql);
                return result != null ? Convert.ToDecimal(result) : 0;
            }
            else
            {
                using (var context = new AcmeBankContext())
                {
                    var account = context.Accounts.Find(accountId);
                    return account?.Balance ?? 0;
                }
            }
        }
    }
}
