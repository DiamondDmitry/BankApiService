using Microsoft.Extensions.Logging;

namespace BankApiService.Dependcies.LifeCycle
{
    public class RequestService
    {
        private readonly TransientDep _transientDep;
        private readonly ScopedDep _scopedDep;
        private readonly SingletonDep _singletonDep;
        private readonly ILogger<RequestService> _logger;

        public RequestService(
            TransientDep transientDep,
            ScopedDep scopedDep,
            SingletonDep singletonDep,
            ILogger<RequestService> logger)
        {
            _transientDep = transientDep;
            _scopedDep = scopedDep;
            _singletonDep = singletonDep;
            _logger = logger;
        }

        public void Log()
        {
            _logger.LogInformation($"SCOPED ID: {_scopedDep.Id}");
            _logger.LogInformation($"TRANSIENT ID: {_transientDep.Id}");
            _logger.LogInformation($"SINGLETON ID: {_singletonDep.Id}");
        }


    }
}