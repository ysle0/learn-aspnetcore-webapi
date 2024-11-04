using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class MySqlContext(DbContextOptions dbContextOptions)
  : DbContext(dbContextOptions) {
  public DbSet<Stock?> Stocks { get; set; }
  public DbSet<Comment> Comments { get; set; }
}