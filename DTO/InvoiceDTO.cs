namespace QLSB_APIs.DTO
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }

        public string? adminEmail { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
