using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Sku).HasColumnName("sku").HasMaxLength(50).IsRequired();
            entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.UnitPrice).HasColumnName("unit_price").HasPrecision(12, 2);
            entity.Property(p => p.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(80).IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.ToTable("inventory_movements");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Id).HasColumnName("id");
            entity.Property(m => m.ProductId).HasColumnName("product_id");
            entity.Property(m => m.UserId).HasColumnName("user_id");
            entity.Property(m => m.MovementType).HasColumnName("movement_type").HasMaxLength(3).IsRequired();
            entity.Property(m => m.Quantity).HasColumnName("quantity");
            entity.Property(m => m.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
        });
    }
}