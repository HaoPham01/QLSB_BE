using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Footballfield
{
    public int FieldId { get; set; }

    public int AdminId { get; set; }

    public string? FieldName { get; set; }

    public string? Type { get; set; }

    public string? Content { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Expensereceipt> Expensereceipts { get; set; } = new List<Expensereceipt>();

    public virtual ICollection<Favoritefield> Favoritefields { get; set; } = new List<Favoritefield>();

    public virtual ICollection<Fieldimage> Fieldimages { get; set; } = new List<Fieldimage>();

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();
}
