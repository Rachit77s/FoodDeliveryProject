# Food Delivery Platform - Polaris

A production-ready food delivery platform built with .NET 8, featuring intelligent rider assignment, AI-powered restaurant recommendations, and real-time order tracking with GPS-based calculations.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [How It Works](#how-it-works)
- [Data Model](#data-model)
- [Examples](#examples)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)

## Overview

This platform connects three types of users through a RESTful API:

1. **Customers** - Browse restaurants, place orders, track deliveries
2. **Restaurants** - Manage menus, accept orders, coordinate with riders
3. **Delivery Riders** - Receive assignments, update location, complete deliveries

### Core Features

- **Smart Restaurant Recommendations** - AI-powered suggestions based on cuisine preferences, location proximity, and delivery time constraints
- **Intelligent Rider Assignment** - Automatic assignment of the nearest available rider using GPS distance calculations
- **Real-time Order Tracking** - Complete order lifecycle management with status updates
- **Delivery Time Estimation** - Accurate ETA calculation using distance, preparation time, and traffic buffer
- **GPS Distance Calculation** - Haversine formula implementation for precise location-based calculations
- **Clean Architecture** - Separation of concerns with Controllers, Services, and Repositories pattern

## Technology Stack

**Backend Framework:**
- .NET 8 / C# 12.0
- ASP.NET Core Web API

**Database:**
- Entity Framework Core 8
- SQL Server (2019+)

**Documentation & Testing:**
- Swagger / OpenAPI 3.0
- Postman compatible

**Architecture Pattern:**
- Clean Architecture
- Repository Pattern
- Dependency Injection

## Prerequisites

### Required Software

1. .NET 8 SDK
2. SQL Server (2019+, Express, or LocalDB)

### Optional Tools

- Visual Studio 2022 (v17.8+) or Visual Studio Code
- SQL Server Management Studio (SSMS)
- Postman / Insomnia / curl for API testing
- Git for version control

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Rachit77s/FoodDeliveryProject.git
cd FoodDeliveryPolaris
```

### 2. Configure Database Connection

Edit `appsettings.json` and update the connection string for your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FoodDeliveryPolaris;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**Connection String Options:**

- **LocalDB**: `Server=(localdb)\\mssqllocaldb;Database=FoodDeliveryPolaris;Trusted_Connection=True;`
- **SQL Express**: `Server=localhost\\SQLEXPRESS;Database=FoodDeliveryPolaris;Trusted_Connection=True;TrustServerCertificate=True`
- **Azure SQL**: `Server=tcp:yourserver.database.windows.net,1433;Database=FoodDeliveryPolaris;User ID=yourusername;Password=yourpassword;`

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

Create the database schema:

```bash
# Install EF Core tools if not already installed
dotnet tool install --global dotnet-ef

# Apply migrations to create database tables
dotnet ef database update
```

**Note:** Sample data (users, riders, restaurants, and menu items) will be automatically seeded when you run the application for the first time. The seeder runs only once and checks if data already exists to avoid duplicates.

### 5. Run the Application

```bash
dotnet run
```

Expected console output:

```
Now listening on: https://localhost:7233
Applying database migrations...
Database migrations applied successfully.
Checking if database seeding is needed...
Seeding database with sample data...
Database seeding completed successfully!
Application started. Press Ctrl+C to shut down.
```

> **First Run:** On the first run in Development mode, the application automatically applies any pending migrations and seeds the database with sample users, riders, restaurants, and menu items. Subsequent runs will skip seeding if data already exists.

### 6. Access the Application

- **Swagger UI**: https://localhost:7233/swagger (Interactive API documentation)
- **Base API**: https://localhost:7233/api

**Available Sample Data After First Run:**
- 5 Users with addresses across different cities
- 10 Delivery riders with various locations
- 5 Restaurants with complete menus (50+ items)

You can immediately start testing the API with these pre-populated IDs (e.g., userId=1, restaurantId=1).

## Project Structure

```
FoodDeliveryPolaris/
├── Controllers/           # API endpoints and HTTP routing
│   ├── UsersController.cs
│   ├── RestaurantsController.cs
│   ├── RidersController.cs
│   ├── OrdersController.cs
│   └── RecommendationsController.cs
├── Services/              # Business logic and orchestration
│   ├── UserService.cs
│   ├── RestaurantService.cs
│   ├── RiderService.cs
│   ├── OrderService.cs
│   └── FoodRecommendationService.cs
├── Repositories/          # Data access layer
│   ├── UserRepository.cs
│   ├── RestaurantRepository.cs
│   ├── RiderRepository.cs
│   └── OrderRepository.cs
├── Models/                # Domain entities
│   ├── User.cs
│   ├── Restaurant.cs
│   ├── Rider.cs
│   ├── Order.cs
│   ├── MenuItem.cs
│   ├── OrderItem.cs
│   ├── Address.cs
│   └── Location.cs
├── DTOs/                  # Data Transfer Objects
│   ├── UserDTOs.cs
│   ├── RestaurantDTOs.cs
│   ├── RiderDTOs.cs
│   ├── OrderDTOs.cs
│   ├── MenuItemDTOs.cs
│   └── FoodRecommendationRequest.cs
├── Validators/            # Input validation
│   ├── UserValidator.cs
│   ├── RestaurantValidator.cs
│   ├── RiderValidator.cs
│   └── AddressValidator.cs
├── Utils/                 # Utility classes
│   ├── LocationUtils.cs          # GPS distance calculations (Haversine)
│   └── DeliveryCalculator.cs     # Delivery time estimation
├── Exceptions/            # Custom exceptions
│   ├── NotFoundException.cs
│   ├── ValidationException.cs
│   └── DuplicateEntityException.cs
├── Data/                  # Database context and seeding
│   ├── FoodDeliveryDbContext.cs
│   └── DatabaseSeeder.cs
└── Migrations/            # EF Core migrations
```

## API Documentation

### Available Endpoints

| Endpoint | Methods | Description |
|----------|---------|-------------|
| `/api/users` | GET, POST, PUT | User registration, profile management, order history |
| `/api/restaurants` | GET, POST, PUT | Restaurant management, menu operations, order acceptance |
| `/api/riders` | GET, POST, PUT, PATCH | Rider management, location updates, availability status |
| `/api/orders` | GET, POST, PUT | Order placement, tracking, and management |
| `/api/recommendations` | GET | AI-powered restaurant recommendations |

### Key API Operations

#### Users
- `POST /api/users` - Register new user
- `GET /api/users/{id}` - Get user profile
- `GET /api/users/{id}/orders` - Get user order history

#### Restaurants
- `GET /api/restaurants` - List all restaurants
- `POST /api/restaurants` - Register new restaurant
- `GET /api/restaurants/{id}/menu` - Get restaurant menu
- `POST /api/restaurants/{id}/accept-order` - Accept an order

#### Riders
- `GET /api/riders` - List all riders
- `POST /api/riders` - Register new rider
- `PATCH /api/riders/{id}/location` - Update rider GPS location
- `PATCH /api/riders/{id}/status` - Update rider availability

#### Orders
- `POST /api/orders` - Place new order
- `GET /api/orders/{id}` - Get order details
- `PUT /api/orders/{id}/status` - Update order status

#### Recommendations
- `GET /api/recommendations?userId={id}&cuisineType={type}&maxTimeMinutes={minutes}` - Get personalized restaurant recommendations

For detailed request/response schemas, visit the Swagger UI at `/swagger`.

## Examples

### Get Restaurant Recommendations

```bash
curl "https://localhost:7233/api/recommendations?userId=1&cuisineType=NorthIndian&maxTimeMinutes=60"
```

**Response:**
```json
[
  {
    "restaurantId": 1,
    "name": "Spice Garden",
    "cuisineType": "NorthIndian",
    "rating": 4.5,
    "distanceKm": 2.3,
    "estimatedDeliveryMinutes": 25,
    "isOpen": true
  }
]
```

### Place an Order

```bash
curl -X POST https://localhost:7233/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "restaurantId": 1,
    "deliveryAddress": {
      "street": "123 Main St",
      "city": "Mumbai",
      "state": "Maharashtra",
      "zipCode": "400001"
    },
    "items": [
      {
        "menuItemId": 1,
        "quantity": 2,
        "specialInstructions": "Extra spicy"
      }
    ]
  }'
```

**Response:**
```json
{
  "orderId": 101,
  "userId": 1,
  "restaurantId": 1,
  "status": "Pending",
  "totalAmount": 450.00,
  "estimatedDeliveryTime": "2025-11-06T14:30:00Z",
  "items": [
    {
      "menuItemId": 1,
      "itemName": "Butter Chicken",
      "quantity": 2,
      "price": 225.00
    }
  ]
}
```

## How It Works

### Order Lifecycle

1. **Order Placement** - Customer selects items and places order (Status: `Pending`)
2. **Restaurant Acceptance** - Restaurant reviews and accepts order (Status: `Confirmed`)
3. **Rider Assignment** - System automatically assigns nearest available rider (Status: `Preparing`)
4. **Pickup** - Rider picks up the order from restaurant (Status: `PickedUp`)
5. **Delivery** - Rider delivers to customer (Status: `Delivered`)

### Intelligent Rider Assignment

When a restaurant accepts an order:

1. **Query Available Riders** - System finds all riders with status `Available`
2. **Calculate Distances** - Uses Haversine formula to compute distance from each rider to restaurant
3. **Select Optimal Rider** - Chooses the nearest rider
4. **Update Status** - Marks rider as `Busy` and order as `Preparing`
5. **Notification** - (Future: Send push notification to rider)

### Delivery Time Estimation

The system calculates estimated delivery time using multiple factors:

```
Total Time = Preparation Time + Travel Time + Buffer

Where:
- Preparation Time: Restaurant's cooking time (e.g., 20 minutes)
- Travel Time: (Distance in KM / Average Speed 20 km/h) * 60
- Buffer: Safety margin (5 minutes)
```

**Example:**
```
Restaurant prep time: 20 min
Distance: 5 km
Travel time: (5 / 20) * 60 = 15 min
Buffer: 5 min
Total: 40 minutes
```

### Restaurant Recommendations Algorithm

The recommendation engine uses a multi-criteria approach:

1. **Filter Criteria:**
   - Restaurant must be currently open
   - Within specified radius (default: 10 km)
   - Matches requested cuisine type
   - Can deliver within requested time window

2. **Ranking Factors:**
   - Estimated total delivery time (weight: 40%)
   - Restaurant rating (weight: 35%)
   - Distance from user (weight: 25%)

3. **Results:** Returns top 10 restaurants sorted by composite score

## Data Model

### Core Entities

#### User
```csharp
- Id (int)
- Name (string)
- Email (string)
- PhoneNumber (string)
- Address (Address)
- CreatedAt (DateTime)
```

#### Restaurant
```csharp
- Id (int)
- Name (string)
- CuisineType (enum)
- Location (Location - GPS coordinates)
- Address (Address)
- Rating (decimal)
- IsOpen (bool)
- AveragePreparationTimeMinutes (int)
- MenuItems (List<MenuItem>)
```

#### Rider
```csharp
- Id (int)
- Name (string)
- PhoneNumber (string)
- VehicleType (string)
- CurrentLocation (Location - GPS coordinates)
- Status (enum: Available, Busy, Offline)
- Rating (decimal)
```

#### Order
```csharp
- Id (int)
- UserId (int)
- RestaurantId (int)
- RiderId (int?)
- Status (enum: Pending, Confirmed, Preparing, PickedUp, Delivered, Cancelled)
- Items (List<OrderItem>)
- TotalAmount (decimal)
- DeliveryAddress (Address)
- OrderedAt (DateTime)
- EstimatedDeliveryTime (DateTime)
```

#### MenuItem
```csharp
- Id (int)
- RestaurantId (int)
- Name (string)
- Description (string)
- Price (decimal)
- IsAvailable (bool)
- Category (string)
```

### Entity Relationships

```
User (1) -----> (*) Order
Restaurant (1) -----> (*) Order
Restaurant (1) -----> (*) MenuItem
Rider (1) -----> (*) Order
Order (1) -----> (*) OrderItem
MenuItem (1) -----> (*) OrderItem
```

## Configuration

### Application Settings

Key configuration options in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DeliverySettings": {
    "MaxDeliveryRadiusKm": 10,
    "AverageSpeedKmPerHour": 20,
    "BufferMinutes": 5
  }
}
```

### Environment Variables

For production deployment:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Server=...;Database=...;"
export ASPNETCORE_URLS="https://+:443;http://+:80"
```

## Troubleshooting

### Common Issues

#### Database Connection Fails

**Problem:** `Cannot open database "FoodDeliveryPolaris"`

**Solution:**
1. Verify SQL Server is running: `sqlcmd -S localhost -Q "SELECT @@VERSION"`
2. Check connection string in `appsettings.json`
3. Ensure database exists: `dotnet ef database update`

#### EF Core Tools Not Found

**Problem:** `dotnet ef command not found`

**Solution:**
```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

#### Port Already in Use

**Problem:** `Address already in use: localhost:7233`

**Solution:**
- Modify port in `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:7234;http://localhost:5234"
```

## Important Notes

- **No Authentication:** User IDs are passed directly in requests (suitable for prototyping only)
- **No Payment Processing:** Orders track amounts but don't process actual payments
- **No Real-time Notifications:** Status updates are pull-based (check order status manually)
- **Simplified Distance Calculation:** Uses Haversine formula (doesn't account for actual road routes)
- **No Concurrent Order Handling:** Riders handle one order at a time
- **Prototype Status:** Not production-hardened (missing rate limiting, advanced error handling, security features)

