# Food Delivery Platform

A scalable food delivery platform built with .NET 8, featuring intelligent rider assignment, restaurant recommendations, and real-time order tracking.

## ?? What Does This Application Do?

This platform connects three types of users through a RESTful API:

1. **Customers** - Browse restaurants, place orders, track deliveries
2. **Restaurants** - Manage menus, accept orders, coordinate with riders
3. **Delivery Riders** - Receive assignments, update location, complete deliveries

### Core Features

- **Smart Restaurant Recommendations** - Get suggestions based on cuisine preference, location, and desired delivery time
- **Intelligent Rider Assignment** - Automatically assigns nearest available rider when restaurant accepts order
- **Real-time Order Tracking** - Complete lifecycle from placement to delivery
- **Delivery Time Estimation** - Calculates time based on distance and restaurant preparation time
- **GPS-based Distance Calculations** - Uses Haversine formula for accurate distance measurement

## ??? Technology Stack

- **.NET 8** with C# 12.0
- **Entity Framework Core 8** for database access
- **SQL Server** for data storage
- **Swagger/OpenAPI** for API documentation
- **Clean Architecture** (Controllers ? Services ? Repositories)

## ?? What You Need to Run This

### Required Software

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** - To build and run the application
2. **SQL Server** - One of the following:
   - [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads)
   - SQL Server Express (free)
   - SQL Server LocalDB (free, comes with Visual Studio)

### Optional Tools

- **Visual Studio 2022** or **VS Code** - For development
- **SQL Server Management Studio** - For database management
- **Postman** or **curl** - For API testing

## ?? How to Run the Application

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd FoodDeliveryPolaris
```

### Step 2: Configure Database

Open `appsettings.json` and verify/update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FoodDeliveryPolaris;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**Note**: Adjust `Server=localhost` to your SQL Server instance name if different.

### Step 3: Setup Database

```bash
# Apply database migrations
dotnet ef database update
```

This creates all necessary tables in SQL Server.

### Step 4: Run the Application

```bash
dotnet run
```

You should see output like:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7233
```

### Step 5: Access the Application

Open your browser and go to:
- **Swagger UI**: https://localhost:7233/swagger
- **API Base**: https://localhost:7233/api

## ?? Project Structure

```
FoodDeliveryPolaris/
??? Controllers/           # API endpoints and HTTP routing
?   ??? UsersController.cs
?   ??? RestaurantsController.cs
?   ??? RidersController.cs
?   ??? OrdersController.cs
?   ??? RecommendationsController.cs
??? Services/              # Business logic and orchestration
?   ??? UserService.cs
?   ??? RestaurantService.cs
?   ??? RiderService.cs
?   ??? OrderService.cs
?   ??? FoodRecommendationService.cs
??? Repositories/          # Data access layer
?   ??? InMemoryUserRepository.cs
?   ??? InMemoryRestaurantRepository.cs
?   ??? InMemoryRiderRepository.cs
?   ??? InMemoryOrderRepository.cs
??? Models/                # Domain entities
?   ??? User.cs
?   ??? Restaurant.cs
?   ??? Rider.cs
?   ??? Order.cs
?   ??? MenuItem.cs
?   ??? OrderItem.cs
?   ??? Address.cs
?   ??? Location.cs
??? DTOs/                  # Data Transfer Objects
?   ??? UserDTOs.cs
?   ??? RestaurantDTOs.cs
?   ??? RiderDTOs.cs
?   ??? OrderDTOs.cs
?   ??? FoodRecommendationRequest.cs
??? Validators/            # Input validation
?   ??? UserValidator.cs
?   ??? RestaurantValidator.cs
?   ??? RiderValidator.cs
?   ??? AddressValidator.cs
?   ??? ValidationHelpers.cs
??? Utils/                 # Utility classes
?   ??? LocationUtils.cs          # GPS distance calculations (Haversine)
?   ??? DeliveryCalculator.cs     # Delivery time estimation
??? Exceptions/            # Custom exceptions
?   ??? NotFoundException.cs
?   ??? ValidationException.cs
?   ??? DuplicateEntityException.cs
??? Data/                  # Database context
?   ??? FoodDeliveryDbContext.cs
??? Migrations/            # EF Core migrations
```

## ?? What APIs Are Available?

| Category | What It Does |
|----------|--------------|
| **Users** (`/api/users`) | Register users, manage profiles, view order history |
| **Restaurants** (`/api/restaurants`) | Register restaurants, manage menus, accept orders |
| **Riders** (`/api/riders`) | Register riders, update GPS location, manage deliveries |
| **Orders** (`/api/orders`) | Place orders, track status, view details |
| **Recommendations** (`/api/recommendations`) | Get restaurant suggestions by cuisine and time |

**Complete API documentation** with interactive testing available at `/swagger` when running.

## ?? Quick Example

Once the application is running, try this:

**Get Restaurant Recommendations:**

```bash
curl "https://localhost:7233/api/recommendations?userId=1&cuisineType=NorthIndian&maxTimeMinutes=60"
```

**Place an Order:**

```bash
curl -X POST "https://localhost:7233/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "restaurantId": 1,
    "items": [{"menuItemId": 1, "quantity": 2}]
  }'
```

More examples and interactive testing in Swagger UI.

## ?? How It Works

### Intelligent Rider Assignment

When a restaurant accepts an order:
1. System finds all available riders
2. Calculates GPS distance from each rider to restaurant (Haversine formula)
3. Assigns nearest rider automatically
4. Updates rider status to "Busy"

### Delivery Time Calculation

```
Total Time = Restaurant Prep Time + Travel Time + 5 min buffer
Travel Time = (Distance in km / 20 km/h) × 60
```

Example: Restaurant 3 km away with 25 min prep time = 25 + 9 + 5 = 39 minutes

### Restaurant Recommendations

Filters restaurants by:
- Open/closed status
- Within delivery radius
- Matching cuisine type
- Can deliver within requested time

Then sorts by: fastest delivery ? highest rating ? nearest distance

## ?? Data Model

The application manages these main entities:

- **User** - Customer with delivery address and order history
- **Restaurant** - Profile, menu items, delivery radius, operating hours
- **Rider** - Delivery partner with real-time GPS location
- **Order** - Items, status, assigned rider, delivery details
- **MenuItem** - Dishes with prices, cuisine types, preparation times

## ?? Important Notes

**Current Limitations:**
- Uses user ID for authentication (no JWT/OAuth yet)
- No payment gateway integration
- No SMS/email notifications
- Data stored in SQL Server

**Not for Production Use** - This is a demonstration/learning project. Production deployment requires additional security, authentication, and infrastructure.

## ?? Future Enhancements

For production deployment, consider adding:
- JWT authentication & authorization
- Payment gateway integration (Stripe/Razorpay)
- Real-time notifications (SMS/email)
- Redis caching for performance
- WebSocket/SignalR for live tracking
- Message queue for async processing
- Geographic data partitioning for scale

## ?? Troubleshooting

**Database connection fails:**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure SQL Server allows TCP/IP connections

**Migration fails:**
- Install EF Core tools: `dotnet tool install --global dotnet-ef`
- Clear migrations and recreate if needed

**Port already in use:**
- Change port in `Properties/launchSettings.json`
- Or stop other applications using port 7233

