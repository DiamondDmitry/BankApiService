using BankApiService.Enums;
using BankApiService.Models;
using BankApiService.Requests;
using BankApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankApiService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private readonly ILogger<Accounts> _logger;
        // DB Sqlite
        private readonly IAccountsService _accountsService;
        private readonly ITransactionsService _transactionsService;




        public Accounts(
                ILogger<Accounts> logger,
                IAccountsService accountsService,
                ITransactionsService transactionsService)
        {
            _logger = logger;
            _accountsService = accountsService;
            _transactionsService = transactionsService;
        }

        [HttpGet("ping")]
        public ActionResult Ping()
        {
           return Ok("pong");
        }

        [HttpGet]
        public ActionResult<List<Account>> GetAccounts()
        {
            _logger.LogWarning("Getting all accounts");

            try
            {
                var accountsList = _accountsService.GetAccounts();
                return Ok(accountsList);
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            var account = _accountsService.GetAccountById(id);

            if (account == null)
            { 
                return BadRequest($"Account with ID: {id} not found.");
            }

            return Ok(account);
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] CreateAccountRequest accountRequest)
        {
            var random = new Random();
            var account = new Account();

            account.Number = random.Next(100, 99999);
            account.Owner = accountRequest.Owner;

            try
            {
                _accountsService.AddAccount(account);
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
            var accountToDeposit = _accountsService.GetAccountById(id);

            if (accountToDeposit == null)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }
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

            _accountsService.UpdateAccount(accountToDeposit);
            _transactionsService.AddTransaction(transaction);

            return Ok(accountToDeposit);
        }

        [HttpPost("{id}/withdraw")]
        public ActionResult<Account> WithdrawFromAccount(
        [FromRoute] int id,
        [FromBody] WithdrawRequest withdrawRequest)
        {
            var accountWithdrawFrom = _accountsService.GetAccountById(id);

            if (accountWithdrawFrom == null)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }
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

            _accountsService.UpdateAccount(accountWithdrawFrom);
            _transactionsService.AddTransaction(transaction);

            return Ok(accountWithdrawFrom);
        }

        [HttpPost("transfer")]
        public ActionResult<Account> Transfer([FromBody] TransferRequest transferRequest)
        {
            var accountFrom = _accountsService.GetAccountById(transferRequest.FromId);
            var accountTo = _accountsService.GetAccountById(transferRequest.ToId);

            if (accountFrom == null )
            {
                return BadRequest($"Account with ID: {transferRequest.FromId} not found.");
            }
            else if (accountTo == null)
            {
                return BadRequest($"Account with ID: {transferRequest.ToId} not found.");
            }

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

            _accountsService.UpdateAccount(accountFrom);
            _transactionsService.AddTransaction(transactionFrom);

            var transactionTo = new Transaction
            {
                Amount = transferRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Transfer,
                AccountId = accountTo.Id,
                OldBalance = accountTo.Balance - transferRequest.Amount,
                NewBalance = accountTo.Balance
            };

            _accountsService.UpdateAccount(accountTo);
            _transactionsService.AddTransaction(transactionTo);

            return Ok(accountFrom);
        }


        [HttpPut("{id}")]
        public ActionResult<Account> UpdateOwnerName(
            [FromRoute] int id,
            [FromBody] UpdateOwnerNameRequest updateRequest
            )
        {
            var operationResult = _accountsService.UpdateOwnerName(id, updateRequest.Owner);

            if (operationResult == OperationResult.Failure)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }
            return Accepted();
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteById([FromRoute]int id)
        {
            var operationResult = _accountsService.DeleteAccount(id);

            if (operationResult == OperationResult.Failure)
            {
                return BadRequest($"Account with ID: {id} not found.");
            }
            return Ok();
        }
    }
}
