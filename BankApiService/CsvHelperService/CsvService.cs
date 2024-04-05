using BankApiService.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace BankApiService.CsvHelperService
{
    public class CsvService<T> where T : EntityBase, new()
    {
        public void WriteToCsv(List<T> listToWrite, string filename)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !File.Exists(filename),
            };

            using (var stream = File.Open(filename, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(listToWrite);
            }
        }
        public List<T> ReadFromCsv(string filename) 
        {
            if (!File.Exists(filename))
            {
                return new List<T>();
            }

            using (var reader = new StreamReader(filename))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Read all records and return as list of Person objects
                return csv.GetRecords<T>().ToList();
            }
        }

        public T GetEntityById(int id, string filename)
        {
            var list = ReadFromCsv(filename);

            foreach (var account in list)
            {
                if (account.Id == id)
                {
                    return account;
                }
            }
            return new T() { Id = -1 };
        }

        public void DeleteEntity(int id, string filename)
        {
            // Get all accounts
            var list = ReadFromCsv(filename);

            // Delete account with id from list all acounts
            var entityToDelete = list.FirstOrDefault(x => x.Id == id);
            list.Remove(entityToDelete);

            // Rewrite file using new list of accounts
            OverwriteAccountsToCSV(list, filename);
        }

        public void OverwriteAccountsToCSV(List<T> entites, string filename)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
            using (var stream = File.Open(filename, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(entites);
            }
        }

        public void UpdateEntityInformation(T entityToUpdate, string filename)
        {
            //Получить все аккаутны
            var list = ReadFromCsv(filename);

            //Найти аккаунт с указанным id

            var account = list.FirstOrDefault(acc => acc.Id == entityToUpdate.Id);

            //Удалить аккаунт перед обновлением
            list.Remove(account);

            //Добавить обновленный аккаунт в список всех аккаунтов
            list.Add(entityToUpdate);

            //Записать обновленный спиок в файл
            OverwriteAccountsToCSV(list, filename);
        }
    }
}
