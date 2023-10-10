using System;
using System.Collections.Generic;

namespace QLSB_APIs.Models.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public int? Status { get; set; }

    public string? Token { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime ResetPasswordExpiry { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Favoritefield> Favoritefields { get; set; } = new List<Favoritefield>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
