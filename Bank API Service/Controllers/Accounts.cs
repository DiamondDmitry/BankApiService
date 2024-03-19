using Bank_API_Service.CsvHelperService;
using Bank_API_Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bank_API_Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Account>> GetAccounts()
        {
            //сделать трай кетч
            var accountList = CsvService.ReadFromCsv();
            return Ok(accountList);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById([FromRoute]int id)
        {
            var account = CsvService.GetAccountById(id);

            if (account.Id == -1) 
            {
                return BadRequest($"Account ID: {id} not found.");         
            }
            return Ok(account);
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount(Account account)
        {
            var random = new Random();
            account.Number = random.Next(100, 99999);

            var listAccounts = new List<Account>();
            listAccounts.Add(account);

            try
            {
                CsvService.WriteToCSV(listAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(account);
        }
      }
}
