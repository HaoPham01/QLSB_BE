using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLSB_APIs.Models.Entities;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Favoritefield> Favoritefields { get; set; }

    public virtual DbSet<Fieldimage> Fieldimages { get; set; }

    public virtual DbSet<Footballfield> Footballfields { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Operatinghour> Operatinghours { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Pricebooking> Pricebookings { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;database=qlsanbong;uid=root;pwd=''");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PRIMARY");

            entity.ToTable("admin");

            entity.Property(e => e.AdminId).HasColumnType("int(11)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RefreshTokenExpiryTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(10)");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PRIMARY");

            entity.ToTable("booking");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.HasIndex(e => e.FieldId, "booking_ibfk_2");

            entity.Property(e => e.BookingId).HasColumnType("int(11)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.FieldId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.PriceBooking)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.StartTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasColumnType("int(10)");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.Field).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_ibfk_1");
        });

        modelBuilder.Entity<Favoritefield>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PRIMARY");

            entity.ToTable("favoritefield");

            entity.HasIndex(e => e.FieldId, "FieldId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.FavoriteId).HasColumnType("int(11)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.FieldId).HasColumnType("int(11)");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Field).WithMany(p => p.Favoritefields)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("favoritefield_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Favoritefields)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("favoritefield_ibfk_1");
        });

        modelBuilder.Entity<Fieldimage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fieldimages");

            entity.HasIndex(e => e.FieldId, "FieldId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.FieldId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");

            entity.HasOne(d => d.Field).WithMany(p => p.Fieldimages)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fieldimages_ibfk_1");
        });

        modelBuilder.Entity<Footballfield>(entity =>
        {
            entity.HasKey(e => e.FieldId).HasName("PRIMARY");

            entity.ToTable("footballfield");

            entity.HasIndex(e => e.AdminId, "AdminId");

            entity.Property(e => e.FieldId).HasColumnType("int(11)");
            entity.Property(e => e.AdminId).HasColumnType("int(11)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.FieldName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(10)");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Admin).WithMany(p => p.Footballfields)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("footballfield_ibfk_1");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PRIMARY");

            entity.ToTable("news");

            entity.HasIndex(e => e.AdminId, "AdminId");

            entity.Property(e => e.NewsId).HasColumnType("int(11)");
            entity.Property(e => e.AdminId).HasColumnType("int(11)");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.Admin).WithMany(p => p.News)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("news_ibfk_1");
        });

        modelBuilder.Entity<Operatinghour>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("operatinghour");

            entity.HasIndex(e => e.FieldId, "FieldId");

            entity.Property(e => e.ClosingTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("time");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.DayOfWeek)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.FieldId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.OpeningTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("time");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Field).WithMany()
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("operatinghour_ibfk_1");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PRIMARY");

            entity.ToTable("price");

            entity.HasIndex(e => e.FieldId, "FieldId");

            entity.Property(e => e.PriceId).HasColumnType("int(11)");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("time");
            entity.Property(e => e.FieldId)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Price1)
                .HasPrecision(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("Price");
            entity.Property(e => e.StartTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("time");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Field).WithMany(p => p.Prices)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("price_ibfk_1");
        });

        modelBuilder.Entity<Pricebooking>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("pricebooking");

            entity.Property(e => e.Price).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.FieldId, "FieldId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.ReviewId).HasColumnType("int(11)");
            entity.Property(e => e.Comment)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.FieldId).HasColumnType("int(11)");
            entity.Property(e => e.Rating).HasColumnType("int(11)");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Field).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reviews_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reviews_ibfk_2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnType("int(11)");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.RefreshTokenExpiryTime)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("datetime");
            entity.Property(e => e.ResetPasswordExpiry).HasColumnType("datetime");
            entity.Property(e => e.ResetPasswordToken)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
