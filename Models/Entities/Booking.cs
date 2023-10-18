using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? FieldId { get; set; }

    public decimal? PriceBooking { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int Status { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Footballfield? Field { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual User? User { get; set; }
}
