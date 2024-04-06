using BankApiService.CsvHelperService;
using BankApiService.Dependcies;
using BankApiService.Dependcies.LifeCycle;
using BankApiService.Enums;
using BankApiService.IdService;
using BankApiService.Models;
using BankApiService.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Security.Principal;
using Transaction = BankApiService.Models.Transaction;

namespace BankApiService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly CsvService<Account> csvAccountService;
        private readonly CsvService<Transaction> csvTransactionService;
        private readonly ILogger<Accounts> _logger;
        private readonly IConfiguration _configuration;
        private readonly SecondTestService _secondTestService;

        public Accounts(
                CsvService<Account> csvService,
                CsvService<Transaction> csvService1,
                ILogger<Accounts> logger,
                IConfiguration configuration,
                SecondTestService secondTestService)
        {
            csvAccountService = csvService;
            csvTransactionService = csvService1;
            _logger = logger;
            _configuration = configuration;
            _secondTestService = secondTestService;
        }

        private const string _accountFileName = "accounts.csv";
        private const string _transactionFileName = "transactions.csv";
        private const string _accountIdFileName = "id.txt";
        private const string _transactionIdFileName = "t_id.txt";

        [HttpGet("ping")]
        public ActionResult Ping()
        {
            var pinginformation = _configuration.Get<PingInformation>();
            _secondTestService.callWarning();

            return Ok(pinginformation);
        }

        [HttpGet]
        public ActionResult<List<Account>> GetAccounts()
        {
            _logger.LogWarning("Getting all accounts");

            try
            {
                var accountList = csvAccountService.ReadFromCsv(_accountFileName);

                foreach ( var account in accountList ) 
                {
                    account.Transactions = TransactionService.GetTransactionsById(account.Id, _transactionFileName);
                }
                return Ok(accountList);

                _logger.LogError("Successfully got all accounts.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            var account = csvAccountService.GetEntityById(id, _accountFileName);

            if (account.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            account.Transactions = TransactionService.GetTransactionsById(account.Id, _transactionFileName);
            return Ok(account);
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] CreateAccountRequest accountRequest)
        {
            var random = new Random();
            var account = new Account();

            account.Number = random.Next(100, 99999);
            account.Owner = accountRequest.Owner;
            var nextId = IdHelper.GetNextId(_accountIdFileName);
            account.Id = nextId;

            var listAccounts = new List<Account>();
            listAccounts.Add(account);

            try
            {
                csvAccountService.WriteToCsv(listAccounts, _accountFileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(account);
        }

        [HttpPost("{id}/deposit")]
        public ActionResult<Account> DepositToAccount(
            [FromRoute] int id,
            [FromBody] DepositRequest depositRequest) 
        {
            var accountToDeposit = csvAccountService.GetEntityById(id, _accountFileName);

            if (accountToDeposit.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            accountToDeposit.Balance += depositRequest.Amount;

            accountToDeposit.Transactions = TransactionService.GetTransactionsById(accountToDeposit.Id, _transactionFileName);

            var transaction = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = depositRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Deposit,
                AccountId = accountToDeposit.Id,
                OldBalance = accountToDeposit.Balance - depositRequest.Amount,
                NewBalance = accountToDeposit.Balance
            };

            accountToDeposit.Transactions.Add(transaction);

            csvAccountService.UpdateEntityInformation(accountToDeposit, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() { transaction }, _transactionFileName);

            return Ok(accountToDeposit);
        }

        [HttpPost("{id}/withdraw")]
        public ActionResult<Account> WithdrawFromAccount(
        [FromRoute] int id,
        [FromBody] WithdrawRequest withdrawRequest)
        {
            var accountFromWithdraw = csvAccountService.GetEntityById(id, _accountFileName);

            if (accountFromWithdraw.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            accountFromWithdraw.Balance -= withdrawRequest.Amount;

            accountFromWithdraw.Transactions = TransactionService.GetTransactionsById(accountFromWithdraw.Id, _transactionFileName);

            var transaction = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = withdrawRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Withdraw,
                AccountId = accountFromWithdraw.Id,
                OldBalance = accountFromWithdraw.Balance + withdrawRequest.Amount,
                NewBalance = accountFromWithdraw.Balance
            };

            accountFromWithdraw.Transactions.Add(transaction);
            csvAccountService.UpdateEntityInformation(accountFromWithdraw, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() { transaction }, _transactionFileName);

            return Ok(accountFromWithdraw);
        }

        [HttpPost("transfer")]
        public ActionResult<Account> Transfer(
        [FromBody] TransferRequest transferRequest)
        {
            var accountFrom = csvAccountService.GetEntityById(transferRequest.FromId, _accountFileName);
            var accountTo = csvAccountService.GetEntityById(transferRequest.ToId, _accountFileName);

            if (accountFrom.Id == -1)
            {
                return BadRequest($"Account ID: {transferRequest.FromId} not found.");
            }
            else if (accountTo.Id == -1)
            {
                return BadRequest($"Account ID: {transferRequest.ToId} not found.");
            }

            accountFrom.Balance -= transferRequest.Amount;
            accountTo.Balance += transferRequest.Amount;

            accountFrom.Transactions = TransactionService.GetTransactionsById(accountFrom.Id, _transactionFileName);
            accountTo.Transactions = TransactionService.GetTransactionsById(accountTo.Id, _transactionFileName);

            var transactionFrom = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = transferRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Transfer,
                AccountId = accountFrom.Id,
                OldBalance = accountFrom.Balance + transferRequest.Amount,
                NewBalance = accountFrom.Balance
            };

            accountFrom.Transactions.Add(transactionFrom);
            csvAccountService.UpdateEntityInformation(accountFrom, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() { transactionFrom }, _transactionFileName);


            var transactionTo = new Transaction
            {
                Id = IdHelper.GetNextId(_transactionIdFileName),
                Amount = transferRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Transfer,
                AccountId = accountTo.Id,
                OldBalance = accountTo.Balance - transferRequest.Amount,
                NewBalance = accountTo.Balance
            };

            accountTo.Transactions.Add(transactionTo);
            csvAccountService.UpdateEntityInformation(accountTo, _accountFileName);
            csvTransactionService.WriteToCsv(new List<Transaction>() { transactionTo }, _transactionFileName);


            return Ok(accountFrom);
        }


        [HttpPut("{id}")]
        public ActionResult<Account> UpdateOwnerName(
            [FromRoute] int id,
            [FromBody] UpdateOwnerNameRequest updateRequest
            )
        {
            var account = csvAccountService.GetEntityById(id, _accountFileName);

            if (account.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            account.Owner = updateRequest.Owner;

            csvAccountService.UpdateEntityInformation(account, _accountFileName);

            return Accepted();
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteById([FromRoute]int id)
        {
            csvAccountService.DeleteEntity(id, _accountFileName);
            return Ok();
        }
    }
}
