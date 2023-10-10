using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Bookingdetail
{
    public int DetailId { get; set; }

    public int BookingId { get; set; }

    public int FieldId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Footballfield Field { get; set; } = null!;
}
