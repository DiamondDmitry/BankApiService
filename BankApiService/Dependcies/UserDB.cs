namespace BankApiService.Dependcies
{
    public class UserDB
    {
        private readonly ConnectionManager _connectionManager;

        public UserDB(ConnectionManager connectionManager, IConfiguration configuration)
        {
            _connectionManager = connectionManager;
        }

        public void AddUser(User userToAdd)
        {
            // Add user logic here
        }

        internal void DeleteUser(User userToDelete)
        {
            throw new NotImplementedException();
        }

        internal void GetUser(User userToGet)
        {
            throw new NotImplementedException();
        }

        internal void UpdateUser(User userToupdate)
        {
            throw new NotImplementedException();
        }
    }
}
