﻿using Microsoft.EntityFrameworkCore;
using OrdersManager.Components.StateMachines;
using OrdersManager.Models;

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
    }
}
