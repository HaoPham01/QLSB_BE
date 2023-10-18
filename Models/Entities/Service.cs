using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Service
{
    public int SvId { get; set; }

    public int? BookingId { get; set; }

    public int? Quantity { get; set; }

    public string? Type { get; set; }

    public decimal? Price { get; set; }

    public virtual Booking? Booking { get; set; }
}
