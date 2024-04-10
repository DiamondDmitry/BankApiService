using BankApiService.Enums;
using System.Text.Json.Serialization;

namespace BankApiService.Models
{
    public class Transaction : EntityBase
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public int Amount { get; set; }

        // Foreign key
        public int AccountId { get; set; }
        public int OldBalance { get; set; }
        public int NewBalance { get; set; }
        public TransactionType TransactionType { get; set; }

        // Navigation properties

        [JsonIgnore]
        public Account Account { get; set; }

    }
}
