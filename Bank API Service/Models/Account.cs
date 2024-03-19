namespace Bank_API_Service.Models
{
    public class Account
    {
        public int Id { get; set; }
        public DateTime OpenDate { get; set; } = DateTime.Now;
        public int Number { get; set; }
        public string Owner { get; set; }
        public int Balance { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
