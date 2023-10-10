using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Review
{
    public int ReviewId { get; set; }

    public int FieldId { get; set; }

    public int UserId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Footballfield Field { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
