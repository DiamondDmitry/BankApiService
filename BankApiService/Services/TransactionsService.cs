using BankApiService.Context;
using BankApiService.Models;

namespace BankApiService.Services
{
    public interface ITransactionsService
    {
        void AddTransaction(Transaction transaction);
    }

    public class TransactionsService : ITransactionsService
    {
        private readonly BankContext _context;

        public TransactionsService(BankContext context)
        {
            _context = context;
        }
        public void AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
    }
}
