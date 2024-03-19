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

        public static List<Account> ReadFromCsv() 
        {
            // проверка на существование файла
            using (var reader = new StreamReader("accounts.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))

            return csv.GetRecords<Account>().ToList();
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
    }
}
