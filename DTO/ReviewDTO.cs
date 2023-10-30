namespace QLSB_APIs.DTO
{
    public class ReviewDTO
    {
        public int ReviewId { get; set; }

        public int BookingId { get; set; }

        public string? Comment { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
