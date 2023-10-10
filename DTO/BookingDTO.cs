﻿namespace QLSB_APIs.DTO
{
    public class BookingDTO
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }

        public int FieldId { get; set; }

        public decimal PriceBooking { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}