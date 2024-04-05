using BankApiService.Models;

namespace BankApiService.CsvHelperService
{
    public interface ICsvService<T> where T : EntityBase, new()
    {
        object GetEntityById(int id, string accountFileName);
        IEnumerable<object> ReadFromCsv(string accountFileName);
    }
}