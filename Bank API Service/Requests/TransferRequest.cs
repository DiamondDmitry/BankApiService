namespace Bank_API_Service.Requests
{
    public class TransferRequest : DepositRequest
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
    }
}
