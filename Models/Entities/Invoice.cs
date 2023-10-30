using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? BookingId { get; set; }

    public int? AdminId { get; set; }

    public decimal PayOnline { get; set; }

    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Booking? Booking { get; set; }
}
