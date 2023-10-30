using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Review
{
    public int ReviewId { get; set; }

    public int BookingId { get; set; }

    public string? Comment { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
