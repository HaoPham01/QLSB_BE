using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.DTO
{
    public class ServiceDTO
    {
        public int SvId { get; set; }
        public int? BookingId { get; set; }

        public int? Quantity { get; set; }

        public string? Type { get; set; }

        public decimal? Price { get; set; }

    }
}
