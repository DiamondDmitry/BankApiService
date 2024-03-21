using Bank_API_Service.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Bank_API_Service.CsvHelperService
{
    public static class CsvService
    {
        public static void WriteToCSV(List<Account> listToWrite)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists("accounts.csv"),
            };

            using (var stream = File.Open("accounts.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static void ReWriteToCSV(List<Account> listToWrite)
        {
            File.Delete("accounts.csv");
            using (var writer = new StreamWriter("accounts.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(listToWrite);
            }
        }

        public static List<Account> ReadFromCsv() 
        {
            if (!File.Exists("accounts.csv"))
            {
                return new List<Account>();
            }

            using (var reader = new StreamReader("accounts.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Read all records and return as list of Person objects
                return csv.GetRecords<Account>().ToList();
            }

        }

        public static Account GetAccountById(int id)
        {
            var allAccount = ReadFromCsv();

            foreach (var account in allAccount)
            {
                if (account.Id == id)
                {
                    return account;
                }
            }
            return new Account() { Id = -1 };
        }

        public static void DeleteAccount(int id)
        {
            // Get all accounts
            var allAccounts = ReadFromCsv();

            // Delete account with id from list all acounts
            var accountToDelete = allAccounts.FirstOrDefault(x => x.Id == id);
            allAccounts.Remove(accountToDelete);

            // Rewrite file using new list of accounts
            CsvService.ReWriteToCSV(allAccounts);
        }

    }
}
