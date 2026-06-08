using Microsoft.EntityFrameworkCore;
using BabyfloServer.Models;

namespace BabyfloServer.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Core E-Commerce Tables
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

        // Payment and Account Tables
        public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
            
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Set numeric precision constraints safely for decimals
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderItem>().Property(i => i.PriceAtPurchase).HasColumnType("decimal(18,2)");

            // 2. Clear out manual relationship maps that were causing compilation errors.
            // Entity Framework automatically tracks dependencies using your standard list declarations!
        }
    }
}