namespace BankApiService.Dependcies.LifeCycle
{
    public class TransientDep
    {
        public Guid Id;

        public TransientDep()
        {
            Id = Guid.NewGuid();
        }
    }
}