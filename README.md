# Food Delivery Platform - Polaris

A production-ready food delivery platform built with .NET 8, featuring intelligent rider assignment, AI-powered restaurant recommendations, and real-time order tracking with GPS-based calculations.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Database Schema & Scalability](#database-schema--scalability)
- [Technical Design](#technical-design)
- [API Documentation](#api-documentation)
- [Testing Guide](#testing-guide)
- [How It Works](#how-it-works)
- [Data Model](#data-model)
- [Examples](#examples)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)
- [Important Notes](#important-notes)
- [Future Enhancements](#future-enhancements)
- [Contributing](#contributing)

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

## Database Schema & Scalability

For comprehensive **database architecture, table structures, and scaling strategies**, see:

� **[Database Schema Documentation](docs/DATABASE_SCHEMA.md)**

This detailed guide covers:

- **Domain Model & Concepts** - Understanding Users, Restaurants, Riders, Orders and their relationships
- **Entity Relationships** - Complete ER diagrams showing all 9 relationship types with visual examples
- **Relationship Patterns** - Owned entities (Address, Location), aggregates, references, snapshot pattern
- **Table Mapping** - How domain concepts naturally map to physical database tables
- **All 6 Tables Documented** - Users, Restaurants, Riders, Orders, MenuItem, OrderItem with SQL schemas
- **Indexes & Performance** - Primary keys, foreign keys, composite indexes, geospatial optimization
- **Data Types & Constraints** - Rationale for DECIMAL, FLOAT, DATETIME2, NVARCHAR choices
- **Scalability Roadmap** - Phase 1 (prototype) → Phase 4 (10M users, 100K restaurants)
- **Sharding Strategies** - Geographic, user ID range, and hybrid approaches
- **Table Partitioning** - Orders partitioned by month for performance
- **Caching Strategy** - Redis integration, TTL policies, cache invalidation patterns
- **Query Optimization** - Eliminating N+1 queries, eager loading, batch operations
- **Monitoring & Maintenance** - Key metrics, slow query detection, index fragmentation

**Quick Facts:**

**Current Capacity (Phase 1):**
- Single SQL Server
- <10,000 users, <1,000 restaurants
- 5,000 orders/day

**Target Capacity (Phase 4):**
- 30 geographic shards
- 10M users, 100K restaurants
- 1M orders/day (12 orders/second avg, 50 peak)
- 100,000 queries/second (with caching)

**Storage Estimates:**
- User: ~300 bytes → 10M users = 3 GB
- Restaurant: ~8 KB (with menu) → 100K = 800 MB
- Order: ~550 bytes → 100M/year = 55 GB/year

**Key Relationships:**
```
User ──[has]──► Address ──[has]──► Location
Restaurant ──[has]──► Address ──[has]──► Location
Restaurant ──[has]──► Menu ──[contains]──► MenuItem (1:N)
Order ──[connects]──► User, Restaurant, Rider, OrderItems
```

## Technical Design

### Understanding the System Logic

For detailed explanations of **assumptions, algorithms, and business logic**, see:

🔧 **[Technical Design Documentation](docs/TECHNICAL_DESIGN.md)**

This comprehensive document covers:

- **Rider Assignment Algorithm** - How we select the nearest available rider (Haversine distance + sorting)
- **Restaurant Recommendation Engine** - Multi-stage filtering pipeline with ranking logic
- **Distance Calculation** - Haversine formula implementation and accuracy analysis
- **Delivery Time Estimation** - Speed assumptions (20 km/h) and buffer calculations (5 min)
- **Order Placement Flow** - Complete lifecycle from cart to delivery
- **Validation Rules** - What we check and why
- **Performance Optimizations** - Early filtering, database queries, spatial indexing plans
- **Edge Cases & Error Handling** - No riders available, out of range, race conditions
- **Future Improvements** - Predictive routing, dynamic pricing, ML enhancements

**Quick Examples from Technical Design:**

**Rider Selection Logic:**
```
1. Get all available riders (status = Available)
2. Calculate distance from each rider to restaurant (Haversine)
3. Sort by distance ascending
4. Select nearest rider
5. Update: rider.status = Busy, order.status = Preparing
```

**Recommendation Ranking:**
```
Sort by:
  1. Total time (40% weight) - Fastest delivery
  2. Rating (35% weight) - Quality
  3. Distance (25% weight) - Proximity tie-breaker
```

## API Documentation

### Quick Reference

For **complete and detailed API documentation** with request/response examples, error codes, and code samples, see:

� **[Complete API Documentation](docs/API_DOCUMENTATION.md)**

### API Overview

| Endpoint | Methods | Description |
|----------|---------|-------------|
| `/api/users` | GET, POST, PUT, DELETE | User registration, profile management, order history |
| `/api/restaurants` | GET, POST, PUT, DELETE | Restaurant management, menu operations, order acceptance |
| `/api/riders` | GET, POST, PUT, DELETE, PATCH | Rider management, location updates, availability status |
| `/api/orders` | GET, POST, PUT, DELETE | Order placement, tracking, and management |
| `/api/recommendations` | GET | AI-powered restaurant recommendations |

### Interactive Documentation

- **Swagger UI**: https://localhost:7233/swagger
- Try out endpoints directly from your browser
- View real-time request/response schemas
- Test with pre-populated sample data

### Quick API Operations

**Most Common Endpoints:**

| Operation | Endpoint | Method |
|-----------|----------|--------|
| Register user | `/api/users` | POST |
| Get recommendations | `/api/recommendations?userId=1&cuisineType=NorthIndian` | GET |
| Place order | `/api/orders` | POST |
| Accept order | `/api/restaurants/{id}/orders/{orderId}/accept` | POST |
| Update rider location | `/api/riders/{id}/location` | PUT |
| Track order | `/api/orders/{id}` | GET |

> 💡 **Tip:** For detailed request/response examples, authentication details, error codes, and code samples in multiple languages, refer to the [Complete API Documentation](docs/API_DOCUMENTATION.md).

---

## Testing Guide

### Hands-On Testing Tutorial

Want to **learn by doing**? Follow our comprehensive step-by-step testing guide to test the complete order flow from start to finish!

📚 **[Complete Testing Guide](docs/TESTING_GUIDE.md)** - 50+ pages of hands-on tutorial

### What's Covered

The testing guide walks you through **9 complete phases** of the order lifecycle:

1. **Setup Test Data** - Create users, restaurants, riders, and menu items
2. **Browse Restaurants** - Get AI-powered recommendations based on location/cuisine
3. **Preview Order** - Calculate cart total and delivery estimates
4. **Place Order** - Submit order with automatic rider assignment
5. **Restaurant Accepts** - Confirm and start preparing the order
6. **Rider Picks Up** - Collect food and update location
7. **Delivery in Progress** - Track rider movement to customer
8. **Order Delivered** - Complete delivery and process payment
9. **Verify Flow** - Check order history from all perspectives

### Key Features

✅ **Step-by-Step Swagger Instructions** - Click-by-click guidance for every API call  
✅ **Copy-Paste JSON Examples** - Ready-to-use request payloads  
✅ **Behind-the-Scenes Explanations** - Understand what happens at each step  
✅ **Common Scenarios** - Test edge cases (closed restaurant, no riders, unavailable items)  
✅ **Troubleshooting** - Solutions for 7+ common problems  
✅ **Quick Reference Table** - All endpoints at a glance  

### Quick Start Testing

```bash
# 1. Start the application
dotnet run

# 2. Open Swagger UI in browser
https://localhost:5001/swagger

# 3. Follow the testing guide
# docs/TESTING_GUIDE.md

# 4. Complete the full order flow in 30-45 minutes!
```

### Perfect For

- 🆕 **New developers** wanting to understand the system quickly
- 🧪 **QA testers** learning the testing workflow  
- 🎓 **Anyone** who prefers hands-on learning
- 🎯 **Demonstrations** of platform capabilities

### Sample Test Scenario

Here's a quick example from the guide:

```json
// Step 1: Create a test user
POST /api/users
{
  "name": "Test Customer",
  "email": "customer@test.com",
  "phone": "+919876543210",
  "address": {
    "street": "123 Main Street",
    "city": "Mumbai",
    "zipCode": "400001",
    "location": {
      "latitude": 19.0760,
      "longitude": 72.8777
    }
  }
}

// Step 2: Get restaurant recommendations
POST /api/recommendations
{
  "userId": 1,
  "cuisineType": "Italian",
  "maxDeliveryTimeMinutes": 45
}

// Step 3: Place an order
POST /api/orders
{
  "userId": 1,
  "restaurantId": 5,
  "items": [
    { "menuItemId": 101, "quantity": 2 },
    { "menuItemId": 102, "quantity": 1 }
  ],
  "deliveryAddress": {
    "latitude": 19.0760,
    "longitude": 72.8777
  }
}

// ... and 6 more phases!
```

> 💡 **Tip:** The testing guide includes troubleshooting for common issues, time estimates for each phase, and tips for testing edge cases.

---

## Examples
| Place order | `/api/orders` | POST |
| Accept order | `/api/restaurants/{id}/orders/{orderId}/accept` | POST |
| Update rider location | `/api/riders/{id}/location` | PUT |
| Track order | `/api/orders/{id}` | GET |

> 💡 **Tip:** For detailed request/response examples, authentication details, error codes, and code samples in multiple languages, refer to the [Complete API Documentation](docs/API_DOCUMENTATION.md).

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

