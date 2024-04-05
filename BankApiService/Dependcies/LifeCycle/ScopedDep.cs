namespace BankApiService.Dependcies.LifeCycle
{
    public class ScopedDep
    {
        public Guid Id;

        public ScopedDep()
        {
            Id = Guid.NewGuid();
        }
    }
}