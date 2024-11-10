using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class AppDbContext(DbContextOptions dbContextOptions)
  : IdentityDbContext<AppUser>(dbContextOptions) {
  public DbSet<Stock?> Stocks { get; set; }
  public DbSet<Comment> Comments { get; set; }
  public DbSet<Portfolio> Portfolios { get; set; }

  protected override void OnModelCreating(ModelBuilder builder) {
    base.OnModelCreating(builder);

    builder.Entity<Portfolio>(
      e => e.HasKey(f => new { f.AppUserId, f.StockId }));

    builder.Entity<Portfolio>()
      .HasOne(u => u.AppUser)
      .WithMany(u => u.Portfolios)
      .HasForeignKey(p => p.AppUserId);

    builder.Entity<Portfolio>()
      .HasOne(u => u.Stock)
      .WithMany(u => u.Portfolios)
      .HasForeignKey(p => p.StockId);

    IList<IdentityRole> roles = new List<IdentityRole> {
      new IdentityRole {
        Name = "Admin",
        NormalizedName = "ADMIN",
      },
      new IdentityRole {
        Name = "User",
        NormalizedName = "USER",
      },
    };
    builder.Entity<IdentityRole>().HasData(roles);
  }
}