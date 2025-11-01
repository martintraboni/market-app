using Microsoft.EntityFrameworkCore;
using Minimarket;
using Models;

public class MinimarketContext : DbContext
{
    public DbSet<Product> Productos { get; set; }
    public DbSet<User> Usuarios { get; set; }
    public DbSet<Sale> Ventas { get; set; }
    public DbSet<SaleItem> Items { get; set; }
    public DbSet<Category> Categorias { get; set; }
    public DbSet<Supplier> Proveedores { get; set; }
    public DbSet<Purchase> Compras { get; set; }
    public DbSet<PurchaseItem> DetalleCompras { get; set; }
    public DbSet<InventoryMovement> MovimientosInventario { get; set; }
    public DbSet<CashMovement> MovimientosCaja { get; set; }
    public DbSet<CashClose> CierresCaja { get; set; }
    public DbSet<AuditLog> Auditoria { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(Config.ConnectionString);

    // ...existing code...
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId);

        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId);

        modelBuilder.Entity<Purchase>()
            .HasOne(pu => pu.Supplier)
            .WithMany(su => su.Purchases)
            .HasForeignKey(pu => pu.SupplierId);

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Purchase)
            .WithMany(pu => pu.PurchaseItems)
            .HasForeignKey(pi => pi.PurchaseId);

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.PurchaseItems)
            .HasForeignKey(pi => pi.ProductId);

        modelBuilder.Entity<InventoryMovement>()
            .HasOne(im => im.Product)
            .WithMany()
            .HasForeignKey(im => im.ProductId);

        modelBuilder.Entity<CashMovement>()
            .HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.UserId);

        modelBuilder.Entity<CashMovement>()
            .HasOne(cm => cm.Sale)
            .WithMany(s => s.CashMovements)
            .HasForeignKey(cm => cm.SaleId);

        modelBuilder.Entity<CashClose>()
            .HasOne(cc => cc.User)
            .WithMany()
            .HasForeignKey(cc => cc.UserId);

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId);
    }
}