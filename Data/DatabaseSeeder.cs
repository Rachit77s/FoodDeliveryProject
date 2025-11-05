using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Data;

/// <summary>
/// Database seeder for initial sample data.
/// Populates database with users, riders, and restaurants on first run.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database with sample data if it's empty.
    /// This method is called automatically on application startup.
    /// </summary>
    /// <param name="context">Database context</param>
    public static void SeedDatabase(FoodDeliveryDbContext context)
    {
        // Check if database already has data
        if (context.Users.Any() || context.Riders.Any() || context.Restaurants.Any())
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
            return;
        }

        Console.WriteLine("Seeding database with sample data...");

        SeedUsers(context);
        SeedRiders(context);
        SeedRestaurants(context);

        context.SaveChanges();
        Console.WriteLine("Database seeding completed successfully!");
    }

    private static void SeedUsers(FoodDeliveryDbContext context)
    {
        var users = new List<User>
        {
            new User
            {
                Name = "Rahul Sharma",
                Email = "rahul.sharma@example.com",
                Phone = "+91-9876543210",
                Address = new Address
                {
                    Street = "B-24, C-Scheme, Near Metro Station",
                    City = "Jaipur",
                    ZipCode = "302001",
                    Location = new Location { Lat = 26.9000, Lon = 75.7900 }
                }
            },
            new User
            {
                Name = "Rachit",
                Email = "rachit@example.com",
                Phone = "+91-9765432109",
                Address = new Address
                {
                    Street = "Plot 45, Vaishali Nagar, Near D-Park",
                    City = "Jaipur",
                    ZipCode = "302021",
                    Location = new Location { Lat = 26.9200, Lon = 75.7650 }
                }
            }
        };

        context.Users.AddRange(users);
        Console.WriteLine($"  ? Added {users.Count} users");
    }

    private static void SeedRiders(FoodDeliveryDbContext context)
    {
        var riders = new List<Rider>
        {
            new Rider
            {
                Name = "Vikram Rider",
                Email = "vikram.rider@example.com",
                Phone = "+91-9543210987",
                VehicleNumber = "RJ-14-AB-1234",
                CurrentLocation = new Location { Lat = 26.9050, Lon = 75.7850 },
                RiderStatus = RiderStatus.Available
            },
            new Rider
            {
                Name = "Mohan Rider",
                Email = "mohan.rider@example.com",
                Phone = "+91-9432109876",
                VehicleNumber = "RJ-14-CD-5678",
                CurrentLocation = new Location { Lat = 26.8950, Lon = 75.8000 },
                RiderStatus = RiderStatus.Available
            },
            new Rider
            {
                Name = "Pintu Rider",
                Email = "rajesh.rider@example.com",
                Phone = "+91-9321098765",
                VehicleNumber = "RJ-14-EF-9012",
                CurrentLocation = new Location { Lat = 26.9150, Lon = 75.7700 },
                RiderStatus = RiderStatus.Available
            }
        };

        context.Riders.AddRange(riders);
        Console.WriteLine($"  ? Added {riders.Count} riders");
    }

    private static void SeedRestaurants(FoodDeliveryDbContext context)
    {
        var restaurants = new List<Restaurant>
        {
            // Restaurant 1: Kebab & Curries Company
            new Restaurant
            {
                Name = "Kebab & Curries Company",
                Phone = "+91-141-4005500",
                Address = new Address
                {
                    Street = "Ground Floor, Clarks Amer Hotel, Jawahar Lal Nehru Marg",
                    City = "Jaipur",
                    ZipCode = "302017",
                    Location = new Location { Lat = 26.9124, Lon = 75.7873 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 8.0,
                AveragePreparationTimeMinutes = 35,
                Rating = 4.3,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Mutton Rogan Josh", Price = 495.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 40 },
                    new MenuItem { Name = "Chicken Tikka Masala", Price = 425.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Paneer Lababdar", Price = 375.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Dal Makhani", Price = 295.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Seekh Kebab (6 pcs)", Price = 450.00, Available = true, CuisineType = CuisineType.Mughlai, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Butter Naan", Price = 65.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 15 },
                    new MenuItem { Name = "Chicken Biryani", Price = 485.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 40 }
                }
            },

            // Restaurant 2: Shikaar Bagh
            new Restaurant
            {
                Name = "Shikaar Bagh - Rambagh Palace",
                Phone = "+91-141-2211919",
                Address = new Address
                {
                    Street = "Taj Rambagh Palace, Bhawani Singh Road",
                    City = "Jaipur",
                    ZipCode = "302005",
                    Location = new Location { Lat = 26.8972, Lon = 75.8058 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 6.0,
                AveragePreparationTimeMinutes = 45,
                Rating = 4.8,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Laal Maas (Signature Rajasthani Mutton)", Price = 1250.00, Available = true, CuisineType = CuisineType.Mughlai, PreparationTimeMinutes = 50 },
                    new MenuItem { Name = "Safed Maas (Royal White Lamb Curry)", Price = 1350.00, Available = true, CuisineType = CuisineType.Mughlai, PreparationTimeMinutes = 50 },
                    new MenuItem { Name = "Jungli Murgh (Wild Game Style Chicken)", Price = 950.00, Available = true, CuisineType = CuisineType.Mughlai, PreparationTimeMinutes = 45 },
                    new MenuItem { Name = "Gatte Ki Sabzi (Rajasthani Specialty)", Price = 625.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Ker Sangri (Desert Beans & Berries)", Price = 575.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Tandoori Roti (Whole Wheat)", Price = 95.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 15 },
                    new MenuItem { Name = "Rajasthani Thali (Complete Meal)", Price = 1850.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 50 }
                }
            },

            // Restaurant 3: ORA Kitchen & Bar
            new Restaurant
            {
                Name = "ORA Kitchen & Bar",
                Phone = "+91-141-5105100",
                Address = new Address
                {
                    Street = "Hotel Clarks Amer, Jawahar Lal Nehru Marg",
                    City = "Jaipur",
                    ZipCode = "302017",
                    Location = new Location { Lat = 26.9120, Lon = 75.7875 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 7.5,
                AveragePreparationTimeMinutes = 30,
                Rating = 4.5,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Wood Fired Pizza Margherita", Price = 525.00, Available = true, CuisineType = CuisineType.Pizza, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Chicken Peri Peri Pizza", Price = 675.00, Available = true, CuisineType = CuisineType.Pizza, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Pasta Arrabiata", Price = 495.00, Available = true, CuisineType = CuisineType.Pizza, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Grilled Chicken with Herbs", Price = 725.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Asian Wok Noodles", Price = 475.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Thai Green Curry", Price = 625.00, Available = true, CuisineType = CuisineType.SouthIndian, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Mediterranean Mezze Platter", Price = 895.00, Available = true, CuisineType = CuisineType.NorthIndian, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "ORA Special Burger", Price = 425.00, Available = true, CuisineType = CuisineType.Burger, PreparationTimeMinutes = 20 }
                }
            },

            // Restaurant 4: WOW! Momo & Chinese
            new Restaurant
            {
                Name = "WOW! Momo & Chinese",
                Phone = "+91-141-2345678",
                Address = new Address
                {
                    Street = "Shop 12, Crystal Palm Mall, 22 Godam Circle",
                    City = "Jaipur",
                    ZipCode = "302006",
                    Location = new Location { Lat = 26.9050, Lon = 75.8200 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 6.5,
                AveragePreparationTimeMinutes = 25,
                Rating = 4.2,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Veg Steamed Momos (8 pcs)", Price = 150.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Chicken Fried Momos (8 pcs)", Price = 180.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Veg Hakka Noodles", Price = 180.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Chicken Schezwan Noodles", Price = 220.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Veg Manchurian Dry", Price = 195.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Chilli Chicken", Price = 240.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Veg Fried Rice", Price = 170.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Spring Rolls (6 pcs)", Price = 160.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 }
                }
            },

            // Restaurant 5: Chinese Wok
            new Restaurant
            {
                Name = "Chinese Wok",
                Phone = "+91-141-2987654",
                Address = new Address
                {
                    Street = "F-18, Malviya Nagar, Near Jawahar Circle",
                    City = "Jaipur",
                    ZipCode = "302017",
                    Location = new Location { Lat = 26.8520, Lon = 75.8110 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 7.0,
                AveragePreparationTimeMinutes = 28,
                Rating = 4.1,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Singapore Noodles", Price = 210.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Burnt Garlic Rice", Price = 195.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Crispy Chilli Potato", Price = 175.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 20 },
                    new MenuItem { Name = "Chicken in Black Bean Sauce", Price = 260.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Veg Triple Schezwan Rice", Price = 230.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 25 },
                    new MenuItem { Name = "Honey Chilli Potato", Price = 190.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 22 },
                    new MenuItem { Name = "Chicken Lollipop (6 pcs)", Price = 240.00, Available = true, CuisineType = CuisineType.Chinese, PreparationTimeMinutes = 25 }
                }
            },

            // Restaurant 6: Behrouz Biryani
            new Restaurant
            {
                Name = "Behrouz Biryani",
                Phone = "+91-141-3456789",
                Address = new Address
                {
                    Street = "Shop 8, Vaishali Nagar, Near D-Park",
                    City = "Jaipur",
                    ZipCode = "302021",
                    Location = new Location { Lat = 26.9180, Lon = 75.7680 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 8.0,
                AveragePreparationTimeMinutes = 40,
                Rating = 4.4,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Dum Gosht Biryani (Mutton)", Price = 450.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 45 },
                    new MenuItem { Name = "Chicken Biryani", Price = 380.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 40 },
                    new MenuItem { Name = "Subz-e-Biryani (Veg)", Price = 320.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Paneer Biryani", Price = 350.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Kebab Platter", Price = 480.00, Available = true, CuisineType = CuisineType.Mughlai, PreparationTimeMinutes = 30 },
                    new MenuItem { Name = "Firni (Dessert)", Price = 120.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 5 }
                }
            },

            // Restaurant 7: Biryani By Kilo
            new Restaurant
            {
                Name = "Biryani By Kilo",
                Phone = "+91-141-4567890",
                Address = new Address
                {
                    Street = "C-11, Tilak Nagar, Near Bus Stand",
                    City = "Jaipur",
                    ZipCode = "302004",
                    Location = new Location { Lat = 26.9280, Lon = 75.7750 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 7.5,
                AveragePreparationTimeMinutes = 45,
                Rating = 4.5,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Hyderabadi Dum Biryani (Chicken)", Price = 420.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 45 },
                    new MenuItem { Name = "Lucknowi Biryani (Mutton)", Price = 520.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 50 },
                    new MenuItem { Name = "Kolkata Biryani (Chicken with Egg)", Price = 440.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 45 },
                    new MenuItem { Name = "Veg Biryani", Price = 340.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 40 },
                    new MenuItem { Name = "Shahi Korma", Price = 380.00, Available = true, CuisineType = CuisineType.Mughlai, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Gulab Jamun (4 pcs)", Price = 80.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 5 }
                }
            },

            // Restaurant 8: Biryani Blues
            new Restaurant
            {
                Name = "Biryani Blues",
                Phone = "+91-141-5678901",
                Address = new Address
                {
                    Street = "Shop 5, Mansarovar Plaza, Sector 4",
                    City = "Jaipur",
                    ZipCode = "302020",
                    Location = new Location { Lat = 26.8750, Lon = 75.7820 }
                },
                IsOpen = true,
                DeliveryRadiusKm = 6.0,
                AveragePreparationTimeMinutes = 38,
                Rating = 4.3,
                Menu = new List<MenuItem>
                {
                    new MenuItem { Name = "Chicken Dum Biryani", Price = 390.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 40 },
                    new MenuItem { Name = "Mutton Biryani", Price = 480.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 45 },
                    new MenuItem { Name = "Egg Biryani", Price = 280.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 35 },
                    new MenuItem { Name = "Paneer Tikka Biryani", Price = 360.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 38 },
                    new MenuItem { Name = "Chicken 65 Biryani", Price = 410.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 40 },
                    new MenuItem { Name = "Raita (Small)", Price = 50.00, Available = true, CuisineType = CuisineType.Biryani, PreparationTimeMinutes = 5 }
                }
            }
        };

        context.Restaurants.AddRange(restaurants);
        Console.WriteLine($"  ? Added {restaurants.Count} restaurants with menu items");
    }
}
