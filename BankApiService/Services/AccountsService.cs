using BankApiService.Context;
using BankApiService.Models;

namespace BankApiService.Services
{
    public interface IAccountsService
    {
        public void AddAccount(Account account);
        public void UpdateAccount(Account account);
        public void DeleteAccount(int id);
        public Account GetAccountById(int id);
        public List<Account> GetAccounts();

    }

    public class AccountsService : IAccountsService
    {
        private readonly BankContext _context;

        public AccountsService(BankContext context)
        {
            _context = context;
        }

        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void DeleteAccount(int id)
        {
            _context.Accounts.Remove(GetAccountById(id));
            _context.SaveChanges();
        }

        public Account GetAccountById(int id)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);
            return account;
        }

        public List<Account> GetAccounts()
        {
            var result = _context.Accounts.ToList();
            return result;
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
        }
    }
}