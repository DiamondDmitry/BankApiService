using BankApiService.Context;
using BankApiService.Enums;
using BankApiService.Models;
using BankApiService.Requests;
using Microsoft.EntityFrameworkCore;

namespace BankApiService.Services
{
    public interface IAccountsService
    {
        public Account AddAccount(CreateAccountRequest accountRequest);
        public OperationResult UpdateOwnerName(int id, string ownerName);
        public OperationResult DeleteAccount(int id);
        public Account GetAccountById(int id);
        public List<Account> GetAccounts();
        void UpdateAccount(Account account);
        public Account DepositeAccount(Account accountToDeposit, DepositRequest depositRequest);
        public Account WithdrawAccount(Account accountWithdrawFrom, WithdrawRequest withdrawRequest);
        public Account Transfer(TransferRequest transferRequest);
    }

    public class AccountsService : IAccountsService
    {
        private readonly BankContext _context;

        public AccountsService(BankContext context)
        {
            _context = context;
        }

        public Account AddAccount(CreateAccountRequest accountRequest)
        {
            var random = new Random();
            var account = new Account();

            account.Number = random.Next(100, 99999);
            account.Owner = accountRequest.Owner;

            _context.Accounts.Add(account);
            _context.SaveChanges();
            return account;
        }

        public Account GetAccountById(int id)
        {
            var account = _context.Accounts
                .Include(x => x.Transactions)
                .FirstOrDefault(x => x.Id == id);
            return account;
        }

        public List<Account> GetAccounts()
        {
            var result = _context.Accounts
                .Include(x => x.Transactions)
                .ToList();
            return result;
        }

        public OperationResult UpdateOwnerName(int id, string ownerName)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);

            if (account != null)
            {
                account.Owner = ownerName;
                _context.Accounts.Update(account);
                _context.SaveChanges();
                return OperationResult.Success;
            }
            return OperationResult.Failure;
        }

        public OperationResult DeleteAccount(int id)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);
            if (account != null)
            {
                _context.Accounts.Remove(GetAccountById(id));
                _context.SaveChanges();
                return OperationResult.Success;
            }
            return OperationResult.Failure;
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public Account DepositeAccount(Account accountToDeposit, DepositRequest depositRequest)
        {

            accountToDeposit.Balance += depositRequest.Amount;

            var transaction = new Transaction
            {
                Amount = depositRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Deposit,
                AccountId = accountToDeposit.Id,
                OldBalance = accountToDeposit.Balance - depositRequest.Amount,
                NewBalance = accountToDeposit.Balance
            };

            accountToDeposit.Transactions.Add(transaction);

            _context.Accounts.Update(accountToDeposit);
            _context.SaveChanges();

            return accountToDeposit;
        }

        public Account WithdrawAccount(Account accountWithdrawFrom, WithdrawRequest withdrawRequest)
        {
            accountWithdrawFrom.Balance -= withdrawRequest.Amount;

            var transaction = new Transaction
            {
                Amount = withdrawRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Withdraw,
                AccountId = accountWithdrawFrom.Id,
                OldBalance = accountWithdrawFrom.Balance + withdrawRequest.Amount,
                NewBalance = accountWithdrawFrom.Balance
            };

            accountWithdrawFrom.Transactions.Add(transaction);

            _context.Accounts.Update(accountWithdrawFrom);
            _context.SaveChanges();

            return accountWithdrawFrom;
        }

        public Account Transfer(TransferRequest transferRequest)
        {
            var accountFrom = _context.Accounts.FirstOrDefault(x => x.Id == transferRequest.FromId);
            var accountTo = _context.Accounts.FirstOrDefault(x => x.Id == transferRequest.ToId);

            accountFrom.Balance -= transferRequest.Amount;
            accountTo.Balance += transferRequest.Amount;

            var transactionFrom = new Transaction
            {
                Amount = transferRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Transfer,
                AccountId = accountFrom.Id,
                OldBalance = accountFrom.Balance + transferRequest.Amount,
                NewBalance = accountFrom.Balance
            };

            accountFrom.Transactions.Add(transactionFrom);

            var transactionTo = new Transaction
            {
                Amount = transferRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Transfer,
                AccountId = accountTo.Id,
                OldBalance = accountTo.Balance - transferRequest.Amount,
                NewBalance = accountTo.Balance
            };

            accountTo.Transactions.Add(transactionTo);

            _context.Accounts.Update(accountFrom);
            _context.Accounts.Update(accountTo);
            _context.SaveChanges();

            return accountFrom;

        }
    }
}