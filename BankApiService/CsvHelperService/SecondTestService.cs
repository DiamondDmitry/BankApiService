namespace BankApiService.CsvHelperService
{
    public class SecondTestService
    {
        private readonly FirstTestService _services;
        public SecondTestService(FirstTestService firstTestService)
        {
            _services = firstTestService;
        }

        public void callWarning()
        {
            _services.LoggerWarning();
        }
    }
}
