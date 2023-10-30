using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Fieldimage
{
    public int Id { get; set; }

    public int? FieldId { get; set; }

    public string PublicId { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public virtual Footballfield? Field { get; set; }
}
