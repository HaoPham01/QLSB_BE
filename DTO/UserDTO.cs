namespace QLSB_APIs.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public int Status { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
