using Microsoft.EntityFrameworkCore;
using Order = OrdersManager.SharedModels.Order;
using OrderItem = OrdersManager.SharedModels.OrderItem;
using OrderState = OrdersManager.SharedModels.OrderState;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderState> OrderStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderState>()
            .HasKey(o => o.CorrelationId);

        modelBuilder.Entity<OrderState>()
            .Property(o => o.CorrelationId)
            .HasColumnType("uniqueidentifier");

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
           .HasOne(o => o.OrderStatus)
           .WithOne()
           .HasForeignKey<Order>(o => o.OrderStateId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .Property(o => o.UnitPrice)
            .HasColumnType("decimal(18,2)");
    }
}
