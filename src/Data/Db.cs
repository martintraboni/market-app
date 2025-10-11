using Microsoft.EntityFrameworkCore;
using Minimarket;
using Minimarket.Models;

public class MinimarketContext : DbContext
{
    public DbSet<Product> Productos { get; set; }
    public DbSet<User> Usuarios { get; set; }
    public DbSet<Sale> Ventas { get; set; }
    public DbSet<SaleItem> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(Config.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}