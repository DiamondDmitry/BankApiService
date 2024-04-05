namespace BankApiService.Dependcies.LifeCycle
{
    public class SingletonDep
    {
        public Guid Id;

        public SingletonDep()
        {
            Id = Guid.NewGuid();
        }
    }
}