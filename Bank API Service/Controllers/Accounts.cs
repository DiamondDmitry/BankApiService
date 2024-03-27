using Bank_API_Service.CsvHelperService;
using Bank_API_Service.Enums;
using Bank_API_Service.IdService;
using Bank_API_Service.Models;
using Bank_API_Service.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Bank_API_Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        private const string _accountFileName = "accounts.csv";
        private const string _transactionFileName = "transactions.csv";

        [HttpGet]
        public ActionResult<List<Account>> GetAccounts()
        {
            try
            {
                var accountList = CsvService<Account>.ReadFromCsv(_accountFileName);

                foreach ( var account in accountList ) 
                {
                    account.Transactions = TransactionService.GetTransactionsById(account.Id, _transactionFileName);
                }
                return Ok(accountList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById([FromRoute] int id)
        {
            var account = CsvService<Account>.GetEntityById(id, _accountFileName);

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
            var nextId = IdHelper.GetNextId();
            account.Id = nextId;

            var listAccounts = new List<Account>();
            listAccounts.Add(account);

            try
            {
                CsvService<Account>.WriteToCSV(listAccounts, _accountFileName);
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
            var accountToDeposit = CsvService<Account>.GetEntityById(id, _accountFileName);

            accountToDeposit.Balance += depositRequest.Amount;

            if (accountToDeposit.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            accountToDeposit.Transactions = TransactionService.GetTransactionsById(accountToDeposit.Id, _transactionFileName);

            var transaction = new Transaction
            {
                Id = IdHelper.GetNextTransactionId(),
                Amount = depositRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Deposit,
                AccountId = accountToDeposit.Id,
                OldBalance = accountToDeposit.Balance - depositRequest.Amount,
                NewBalance = accountToDeposit.Balance
            };

            accountToDeposit.Transactions.Add(transaction);

            CsvService<Account>.UpdateEntityInformation(accountToDeposit, _accountFileName);
            CsvService<Transaction>.WriteToCSV(new List<Transaction>() { transaction }, _transactionFileName);

            return Ok(accountToDeposit);
        }

        [HttpPost("{id}/withdraw")]
        public ActionResult<Account> WithdrawFromAccount(
        [FromRoute] int id,
        [FromBody] WithdrawRequest withdrawRequest)
        {
            var accountFromWithdraw = CsvService<Account>.GetEntityById(id, _accountFileName);

            accountFromWithdraw.Balance -= withdrawRequest.Amount;

            if (accountFromWithdraw.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            accountFromWithdraw.Transactions = TransactionService.GetTransactionsById(accountFromWithdraw.Id, _transactionFileName);

            var transaction = new Transaction
            {
                Id = IdHelper.GetNextTransactionId(),
                Amount = withdrawRequest.Amount,
                Date = DateTime.Now,
                TransactionType = TransactionType.Withdraw,
                AccountId = accountFromWithdraw.Id,
                OldBalance = accountFromWithdraw.Balance + withdrawRequest.Amount,
                NewBalance = accountFromWithdraw.Balance
            };

            accountFromWithdraw.Transactions.Add(transaction);
            CsvService<Account>.UpdateEntityInformation(accountFromWithdraw, _accountFileName);
            CsvService<Transaction>.WriteToCSV(new List<Transaction>() { transaction }, _transactionFileName);

            return Ok(accountFromWithdraw);
        }

        [HttpPut("{id}")]
        public ActionResult<Account> UpdateOwnerName(
            [FromRoute] int id,
            [FromBody] UpdateOwnerNameRequest updateRequest
            )
        {
            var account = CsvService<Account>.GetEntityById(id, _accountFileName);

            if (account.Id == -1)
            {
                return BadRequest($"Account ID: {id} not found.");
            }

            account.Owner = updateRequest.Owner;

            CsvService<Account>.UpdateEntityInformation(account, _accountFileName);

            return Accepted();
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteById([FromRoute]int id)
        {
            CsvService<Account>.DeleteEntity(id, _accountFileName);
            return Ok();
        }
    }
}
