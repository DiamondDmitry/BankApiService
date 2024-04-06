namespace BankApiService.CsvHelperService
{
    public class FirstTestService
    {
        private readonly ILogger<FirstTestService> _logger;
        public FirstTestService(ILogger<FirstTestService> logger) 
        { 
            _logger = logger;
        }

        public void LoggerWarning()
        {
            _logger.LogWarning("Logging method called successfully!");
        }
    }
}
