using FoodDeliveryPolaris.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryPolaris.Data;

/// <summary>
/// Database context for the Food Delivery Polaris application.
/// Manages entity configurations and database connections.
/// </summary>
public class FoodDeliveryDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the FoodDeliveryDbContext.
    /// </summary>
    /// <param name="options">Database context options.</param>
    public FoodDeliveryDbContext(DbContextOptions<FoodDeliveryDbContext> options) 
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the Riders DbSet.
    /// </summary>
    public DbSet<Rider> Riders { get; set; }

    /// <summary>
    /// Gets or sets the Restaurants DbSet.
    /// </summary>
    public DbSet<Restaurant> Restaurants { get; set; }

    /// <summary>
    /// Gets or sets the Orders DbSet.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Configures the model that was discovered by convention from entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment INT
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Email);
            
            // Configure owned Address
            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.ZipCode).HasMaxLength(20);
                
                // Configure nested owned Location
                address.OwnsOne(a => a.Location, location =>
                {
                    location.Property(l => l.Lat).IsRequired();
                    location.Property(l => l.Lon).IsRequired();
                });
            });
        });

        // Configure Rider entity
        modelBuilder.Entity<Rider>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment INT
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.VehicleNumber).HasMaxLength(50);
            entity.Property(e => e.RiderStatus).HasConversion<string>().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.RiderStatus);
            
            // Configure owned CurrentLocation
            entity.OwnsOne(e => e.CurrentLocation, location =>
            {
                location.Property(l => l.Lat).IsRequired();
                location.Property(l => l.Lon).IsRequired();
            });
        });

        // Configure Restaurant entity
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment INT
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.IsOpen).HasDefaultValue(true);
            entity.Property(e => e.DeliveryRadiusKm).HasDefaultValue(5.0);
            entity.Property(e => e.AveragePreparationTimeMinutes).HasDefaultValue(20);
            entity.Property(e => e.Rating).HasDefaultValue(4.0).HasColumnType("decimal(3,2)");
            
            // Configure owned Address
            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.ZipCode).HasMaxLength(20);
                
                // Configure nested owned Location
                address.OwnsOne(a => a.Location, location =>
                {
                    location.Property(l => l.Lat).IsRequired();
                    location.Property(l => l.Lon).IsRequired();
                });
            });

            // Configure owned collection of MenuItems
            entity.OwnsMany(e => e.Menu, menuItem =>
            {
                menuItem.Property(m => m.Id).ValueGeneratedOnAdd(); // Auto-increment INT for menu items
                menuItem.Property(m => m.Name).IsRequired().HasMaxLength(200);
                menuItem.Property(m => m.Price).HasColumnType("decimal(18,2)");
                menuItem.Property(m => m.Available).HasDefaultValue(true);
                menuItem.Property(m => m.CuisineType).HasConversion<string>().HasMaxLength(50);
                menuItem.Property(m => m.PreparationTimeMinutes).HasDefaultValue(20);
            });
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Auto-increment INT
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.RestaurantId).IsRequired();
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CompletedAt);
            entity.Property(e => e.Notes).HasMaxLength(500);

            // Configure owned DeliveryAddress
            entity.OwnsOne(e => e.DeliveryAddress, location =>
            {
                location.Property(l => l.Lat).IsRequired();
                location.Property(l => l.Lon).IsRequired();
            });

            // Configure owned collection of OrderItems
            entity.OwnsMany(e => e.Items, orderItem =>
            {
                orderItem.Property(oi => oi.MenuItemId).IsRequired();
                orderItem.Property(oi => oi.Name).IsRequired().HasMaxLength(200);
                orderItem.Property(oi => oi.Price).HasColumnType("decimal(18,2)");
                orderItem.Property(oi => oi.Quantity).IsRequired();
            });

            // Add indexes for better query performance
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RestaurantId);
            entity.HasIndex(e => e.RiderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.CompletedAt);
        });
    }
}
