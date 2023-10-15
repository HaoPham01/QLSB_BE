namespace QLSB_APIs.DTO
{
    public class VnPayDTO
    {
        public string Url { get; set; }
    }

    public class VnPayResultDTO
    {
        public string vnp_SecureHash { get; set; }
        public string vnp_TransactionStatus { get; set; }
        public string vnp_ResponseCode { get; set; }
        public int vnp_TxnRef { get; set; }
        public int vnp_Amount { get; set; }

    }

    public class ResultDTO
    {
        public string message { get; set; }

    }
}
