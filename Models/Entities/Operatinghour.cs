using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Operatinghour
{
    public int? FieldId { get; set; }

    public int? DayOfWeek { get; set; }

    public TimeSpan? OpeningTime { get; set; }

    public TimeSpan? ClosingTime { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Footballfield? Field { get; set; }
}
