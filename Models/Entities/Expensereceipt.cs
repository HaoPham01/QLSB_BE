using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Expensereceipt
{
    public int ErId { get; set; }

    public int? FieldId { get; set; }

    public int? AdminId { get; set; }

    public string? Type { get; set; }

    public string? Content { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Footballfield? Field { get; set; }
}
