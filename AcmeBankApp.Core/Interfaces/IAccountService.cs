using AcmeBankApp.Core.Models;
using System.Collections.Generic;

namespace AcmeBankApp.Core.Interfaces
{
    public interface IAccountService
    {
        List<Account> GetAccountsByUserId(int userId);
        Account GetAccountById(int accountId);
        List<Transaction> GetTransactionsByAccountId(int accountId);
        void Transfer(TransferModel transfer);
        decimal GetAccountBalance(int accountId);
    }
}
