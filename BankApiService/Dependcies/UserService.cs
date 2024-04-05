using BankApiService.Controllers;

namespace BankApiService.Dependcies
{
    public class UserService : IUserService
    {
        private UserDB _userDB;
        private ILogger<UserService> _logger;
        private AccountsDB _accountsDB;

        private const string Name = "Name";

        public UserService(UserDB userDB, ILogger<UserService> logger, AccountsDB accountsDB)
        {
            _logger = logger;
            _userDB = userDB;
            _accountsDB = accountsDB;
        }

        public void AddUser(User userToAdd)
        {
            _userDB.AddUser(userToAdd);
            _logger.LogInformation($"User {userToAdd.Name} added");
        }

        public void DeleteUser(User userToDelete)
        {
            _userDB.DeleteUser(userToDelete);
        }

        public void UpdateUser(User userToupdate)
        {
            _userDB.UpdateUser(userToupdate);
        }

        public void GetUser(User userToGet)
        {
            _userDB.GetUser(userToGet);
        }
    }

    public interface IUserService
    {
    }
}