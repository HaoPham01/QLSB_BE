namespace QLSB_APIs.DTO
{
    public class NewsDTO
    {
        public int NewsId { get; set; }

        public int AdminId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Url { get; set; }

        public string? PublicId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
