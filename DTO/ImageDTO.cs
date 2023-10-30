namespace QLSB_APIs.DTO
{
    public class ImageDTO
    {
        public int Id { get; set; }

        public int? FieldId { get; set; }

        public string PublicId { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}
