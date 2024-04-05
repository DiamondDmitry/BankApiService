using BankApiService.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace BankApiService.CsvHelperService
{
    public static class TransactionService
    {
        public static List<Transaction> GetTransactionsById(int id, string fileName)
        {
            var service = new CsvService<Transaction>();
            var allTransactions = service.ReadFromCsv(fileName);
            var transactions = new List<Transaction>();

            foreach (var transaction in allTransactions)
            {
                if (transaction.AccountId == id)
                {
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
    }
}
