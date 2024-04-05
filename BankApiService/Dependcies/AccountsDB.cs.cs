namespace BankApiService.Dependcies
{
    public class AccountsDB
    {
        private readonly ConnectionManager _connectionManager;

        public AccountsDB(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        internal void GetAccounts(User userToGet)
        {
            throw new NotImplementedException();
        }
    }
}