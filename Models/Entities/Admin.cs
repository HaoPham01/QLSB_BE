using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Role { get; set; }

    public int? Status { get; set; }

    public string? Token { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Footballfield> Footballfields { get; set; } = new List<Footballfield>();

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
