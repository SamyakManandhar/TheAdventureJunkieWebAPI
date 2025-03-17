using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Data;

public partial class TheAdventureJunkieDbContext : DbContext
{
    public TheAdventureJunkieDbContext()
    {
    }

    public TheAdventureJunkieDbContext(DbContextOptions<TheAdventureJunkieDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<GlobalState> GlobalStates { get; set; }

    public virtual DbSet<Leases057f2c8d8f25e7361685581043> Leases057f2c8d8f25e7361685581043s { get; set; }

    public virtual DbSet<Leases0dc0bc6384d6d9b71685581043> Leases0dc0bc6384d6d9b71685581043s { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("TheAdventureJunkieDbContextConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Events_CategoryId");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Events).HasForeignKey(d => d.CategoryId);
        });

        modelBuilder.Entity<GlobalState>(entity =>
        {
            entity.HasKey(e => new { e.UserFunctionId, e.UserTableId }).HasName("PK__GlobalSt__A1FEF6DDCBFA4650");

            entity.ToTable("GlobalState", "az_func");

            entity.Property(e => e.UserFunctionId)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserFunctionID");
            entity.Property(e => e.UserTableId).HasColumnName("UserTableID");
            entity.Property(e => e.LastAccessTime)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Leases057f2c8d8f25e7361685581043>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Leases_0__C3905BCF84F0E6AD");

            entity.ToTable("Leases_057f2c8d8f25e736_1685581043", "az_func");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.AzFuncAttemptCount).HasColumnName("_az_func_AttemptCount");
            entity.Property(e => e.AzFuncChangeVersion).HasColumnName("_az_func_ChangeVersion");
            entity.Property(e => e.AzFuncLeaseExpirationTime).HasColumnName("_az_func_LeaseExpirationTime");
        });

        modelBuilder.Entity<Leases0dc0bc6384d6d9b71685581043>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Leases_0__C3905BCFD9BB0104");

            entity.ToTable("Leases_0dc0bc6384d6d9b7_1685581043", "az_func");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.AzFuncAttemptCount).HasColumnName("_az_func_AttemptCount");
            entity.Property(e => e.AzFuncChangeVersion).HasColumnName("_az_func_ChangeVersion");
            entity.Property(e => e.AzFuncLeaseExpirationTime).HasColumnName("_az_func_LeaseExpirationTime");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.AddressLine1).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.OrderTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PhoneNumber).HasMaxLength(25);
            entity.Property(e => e.State).HasMaxLength(10);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasIndex(e => e.EventId, "IX_OrderDetails_EventId");

            entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Event).WithMany(p => p.OrderDetails).HasForeignKey(d => d.EventId);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasForeignKey(d => d.OrderId);
        });

        modelBuilder.Entity<ShoppingCartItem>(entity =>
        {
            entity.HasIndex(e => e.EventId, "IX_ShoppingCartItems_EventId");

            entity.HasOne(d => d.Event).WithMany(p => p.ShoppingCartItems).HasForeignKey(d => d.EventId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
