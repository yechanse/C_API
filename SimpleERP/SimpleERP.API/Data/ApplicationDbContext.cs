using Microsoft.EntityFrameworkCore;
using SimpleERP.API.Models;

namespace SimpleERP.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        // Customer 테이블 제거 (사용하지 않음)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SKU).IsUnique();
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.SKU).HasMaxLength(20);
            });

            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(12,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                
                // CustomerId는 단순히 User.Id를 참조하는 정수 필드로만 처리
                entity.Property(e => e.CustomerId).IsRequired();
            });

            // OrderItem entity configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(12,2)");
                
                // Foreign key relationships
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Customer 엔티티 설정 제거 - 사용하지 않음

            // Seed data for testing
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Static DateTime values (고정된 날짜 사용)
            var baseDate = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop Computer",
                    Description = "High-performance business laptop",
                    Price = 1299.99m,
                    StockQuantity = 25,
                    Category = "Electronics",
                    SKU = "LAP001",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(-30),
                    UpdatedAt = baseDate.AddDays(-5)
                },
                new Product
                {
                    Id = 2,
                    Name = "Office Chair",
                    Description = "Ergonomic office chair with lumbar support",
                    Price = 299.99m,
                    StockQuantity = 50,
                    Category = "Furniture",
                    SKU = "CHR001",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(-25),
                    UpdatedAt = baseDate.AddDays(-3)
                },
                new Product
                {
                    Id = 3,
                    Name = "Wireless Mouse",
                    Description = "Bluetooth wireless mouse with precision tracking",
                    Price = 49.99m,
                    StockQuantity = 100,
                    Category = "Electronics",
                    SKU = "MOU001",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(-20),
                    UpdatedAt = baseDate.AddDays(-2)
                },
                new Product
                {
                    Id = 4,
                    Name = "Standing Desk",
                    Description = "Adjustable height standing desk",
                    Price = 599.99m,
                    StockQuantity = 15,
                    Category = "Furniture",
                    SKU = "DSK001",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(-15),
                    UpdatedAt = baseDate.AddDays(-1)
                },
                new Product
                {
                    Id = 5,
                    Name = "Monitor 24 inch",
                    Description = "4K Ultra HD monitor for professionals",
                    Price = 399.99m,
                    StockQuantity = 30,
                    Category = "Electronics",
                    SKU = "MON001",
                    IsActive = true,
                    CreatedAt = baseDate.AddDays(-10),
                    UpdatedAt = baseDate
                }
            );

            // Seed Mock Users (for testing)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 100, // Use high ID to avoid conflicts with real users
                    Username = "john_doe",
                    Email = "john@example.com",
                    PasswordHash = "$2a$11$mock.hash.for.testing.purposes.only",
                    CreatedAt = baseDate.AddDays(-20),
                    IsActive = true
                },
                new User
                {
                    Id = 101,
                    Username = "jane_smith",
                    Email = "jane@example.com", 
                    PasswordHash = "$2a$11$mock.hash.for.testing.purposes.only",
                    CreatedAt = baseDate.AddDays(-15),
                    IsActive = true
                },
                new User
                {
                    Id = 102,
                    Username = "bob_wilson",
                    Email = "bob@example.com",
                    PasswordHash = "$2a$11$mock.hash.for.testing.purposes.only",
                    CreatedAt = baseDate.AddDays(-10),
                    IsActive = true
                }
            );

            // Seed Mock Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderNumber = "ORD202412001",
                    CustomerId = 100, // john_doe
                    TotalAmount = 1649.98m, // Laptop + Mouse
                    Status = "Delivered",
                    OrderDate = baseDate.AddDays(-15),
                    ShippedDate = baseDate.AddDays(-13),
                    DeliveredDate = baseDate.AddDays(-10),
                    ShippingAddress = "123 Main St, Tech City, TC 12345",
                    Notes = "Priority delivery requested"
                },
                new Order
                {
                    Id = 2,
                    OrderNumber = "ORD202412002",
                    CustomerId = 101, // jane_smith
                    TotalAmount = 899.98m, // Chair + Standing Desk
                    Status = "Shipped",
                    OrderDate = baseDate.AddDays(-10),
                    ShippedDate = baseDate.AddDays(-8),
                    ShippingAddress = "456 Business Ave, Corporate City, CC 67890",
                    Notes = "Office setup order"
                },
                new Order
                {
                    Id = 3,
                    OrderNumber = "ORD202412003",
                    CustomerId = 102, // bob_wilson
                    TotalAmount = 799.98m, // Monitor + Standing Desk
                    Status = "Processing",
                    OrderDate = baseDate.AddDays(-5),
                    ShippingAddress = "789 Developer Rd, Code City, CD 54321",
                    Notes = "Home office upgrade"
                },
                new Order
                {
                    Id = 4,
                    OrderNumber = "ORD202412004",
                    CustomerId = 100, // john_doe (repeat customer)
                    TotalAmount = 349.98m, // Chair + Mouse
                    Status = "Pending",
                    OrderDate = baseDate.AddDays(-2),
                    ShippingAddress = "123 Main St, Tech City, TC 12345",
                    Notes = "Additional equipment"
                },
                new Order
                {
                    Id = 5,
                    OrderNumber = "ORD202412005",
                    CustomerId = 101, // jane_smith (repeat customer)
                    TotalAmount = 1299.99m, // Laptop only
                    Status = "Processing",
                    OrderDate = baseDate.AddDays(-1),
                    ShippingAddress = "456 Business Ave, Corporate City, CC 67890",
                    Notes = "New team member setup"
                }
            );

            // Seed Mock Order Items
            modelBuilder.Entity<OrderItem>().HasData(
                // Order 1 items (john_doe)
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 1299.99m, TotalPrice = 1299.99m },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 3, Quantity = 7, UnitPrice = 49.99m, TotalPrice = 349.93m },
                
                // Order 2 items (jane_smith)
                new OrderItem { Id = 3, OrderId = 2, ProductId = 2, Quantity = 1, UnitPrice = 299.99m, TotalPrice = 299.99m },
                new OrderItem { Id = 4, OrderId = 2, ProductId = 4, Quantity = 1, UnitPrice = 599.99m, TotalPrice = 599.99m },
                
                // Order 3 items (bob_wilson)
                new OrderItem { Id = 5, OrderId = 3, ProductId = 5, Quantity = 2, UnitPrice = 399.99m, TotalPrice = 799.98m },
                
                // Order 4 items (john_doe)
                new OrderItem { Id = 6, OrderId = 4, ProductId = 2, Quantity = 1, UnitPrice = 299.99m, TotalPrice = 299.99m },
                new OrderItem { Id = 7, OrderId = 4, ProductId = 3, Quantity = 1, UnitPrice = 49.99m, TotalPrice = 49.99m },
                
                // Order 5 items (jane_smith)
                new OrderItem { Id = 8, OrderId = 5, ProductId = 1, Quantity = 1, UnitPrice = 1299.99m, TotalPrice = 1299.99m }
            );
        }
    }
}