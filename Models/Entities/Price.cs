using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Price
{
    public int PriceId { get; set; }

    public int? FieldId { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    public decimal? Price1 { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Footballfield? Field { get; set; }
}
