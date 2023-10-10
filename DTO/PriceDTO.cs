namespace QLSB_APIs.DTO
{
    public class PriceDTO
    {
        public int PriceId { get; set; }
        public int FieldId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public decimal Price1 { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
