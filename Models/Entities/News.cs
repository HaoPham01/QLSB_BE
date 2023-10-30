using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class News
{
    public int NewsId { get; set; }

    public int AdminId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Url { get; set; }

    public string? PublicId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public virtual Admin Admin { get; set; } = null!;
}
