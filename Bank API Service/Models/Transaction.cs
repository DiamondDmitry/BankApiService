using Bank_API_Service.Enums;

namespace Bank_API_Service.Models
{
    public class Transaction : EntityBase
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public int Amount { get; set; }
        public int AccountId { get; set; }
        public int OldBalance { get; set; }
        public int NewBalance { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
