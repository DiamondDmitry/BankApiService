namespace BankApiService.Dependcies
{
    public class ConnectionManager
    {
        private string connectionString;

        public ConnectionManager(IConfiguration configuration)
        {
            connectionString = configuration["configString"];
        }
    }
}