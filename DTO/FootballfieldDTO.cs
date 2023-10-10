namespace QLSB_APIs.DTO
{
    public class FootballfieldDTO
    {
        public int FieldId { get; set; }

        public int AdminId { get; set; }

        public string? FieldName { get; set; }

        public string? Type { get; set; }

        public int?  Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}   
