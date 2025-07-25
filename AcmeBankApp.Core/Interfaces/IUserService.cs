using AcmeBankApp.Core.Models;

namespace AcmeBankApp.Core.Interfaces
{
    public interface IUserService
    {
        User GetUserByUserName(string userName);
        User GetUserById(int userId);
        bool ValidateUser(string userName, string password);
        void CreateUser(User user);
        void UpdateUser(User user);
        void UpdateLastLogin(int userId);
    }
}
