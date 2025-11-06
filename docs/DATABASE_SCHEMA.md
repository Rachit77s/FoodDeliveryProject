# Database Schema & Scalability Strategy

**Version:** 2.0  
**Purpose:** Complete database architecture documentation with scalability planning for millions of users

---

## Table of Contents

1. [Domain Model & Concepts](#domain-model--concepts)
2. [Entity Relationships](#entity-relationships)
3. [How Relationships Map to Tables](#how-relationships-map-to-tables)
4. [Complete Table Structures](#complete-table-structures)
5. [Indexes & Performance](#indexes--performance)
6. [Data Types & Constraints](#data-types--constraints)
7. [Scalability Strategy](#scalability-strategy)
8. [Sharding & Partitioning](#sharding--partitioning)
9. [Caching Strategy](#caching-strategy)
10. [Database Optimization Techniques](#database-optimization-techniques)
11. [Monitoring & Maintenance](#monitoring--maintenance)

---

## Domain Model & Concepts

### Understanding the Food Delivery Domain

Before diving into tables, let's understand the **real-world entities** and **how they interact** in a food delivery platform:

#### Core Actors

1. **User (Customer)**
   - A person who wants to order food
   - Has a delivery address with GPS coordinates
   - Can place multiple orders over time
   - Can search for nearby restaurants
   - Can view order history

2. **Restaurant**
   - A food establishment that accepts orders
   - Has a physical location with GPS coordinates
   - Offers a menu of items to order
   - Has operational settings (open/closed, delivery radius, prep time)
   - Receives orders from customers

3. **Rider (Delivery Partner)**
   - A person who delivers orders
   - Has a current GPS location (updated frequently)
   - Can be available, busy, or offline
   - Gets assigned to orders based on proximity
   - Delivers food from restaurant to customer

4. **Order**
   - A transaction request from a customer
   - Contains multiple menu items from one restaurant
   - Requires delivery to a specific address
   - Has a lifecycle (placed → preparing → picked up → delivered)
   - Connects customer, restaurant, and rider

#### Supporting Concepts

5. **Address**
   - Physical location information (street, city, zip)
   - Contains precise GPS coordinates (latitude, longitude)
   - Both users and restaurants have addresses
   - Used for distance calculations and delivery routing

6. **Location**
   - GPS coordinates (latitude, longitude)
   - Part of an Address (nested within)
   - Also used for rider's real-time position
   - Enables proximity-based searches and routing

7. **MenuItem**
   - Individual food item on a restaurant's menu
   - Has name, price, cuisine type, preparation time
   - Can be available or unavailable
   - Part of a restaurant's menu collection

8. **OrderItem**
   - Line item in an order
   - Links an order to menu items with quantity
   - Captures price at the time of order (snapshot)
   - Allows special instructions per item

### Domain Relationships

Let's visualize how these entities relate to each other:

```
┌─────────────────────────────────────────────────────────────────┐
│                    DOMAIN RELATIONSHIP MODEL                    │
└─────────────────────────────────────────────────────────────────┘

1. USER RELATIONSHIPS:
   
   User ──── has ───► Address ──── has ───► Location
   User ──── places ───► Order (1:N - one user, many orders)
   User ──── searches for ───► Restaurant (based on Location)

2. RESTAURANT RELATIONSHIPS:
   
   Restaurant ──── has ───► Address ──── has ───► Location
   Restaurant ──── has ───► Menu ──── contains ───► MenuItem (1:N)
   Restaurant ──── receives ───► Order (1:N - one restaurant, many orders)

3. RIDER RELATIONSHIPS:
   
   Rider ──── has ───► CurrentLocation (updated every 10-30 seconds)
   Rider ──── delivers ───► Order (1:N - one rider, many deliveries)

4. ORDER RELATIONSHIPS:
   
   Order ──── placed by ───► User (N:1 - many orders, one user)
   Order ──── to ───► Restaurant (N:1 - many orders, one restaurant)
   Order ──── delivered by ───► Rider (N:1 - many orders, one rider)
   Order ──── has ───► DeliveryAddress (Location)
   Order ──── contains ───► OrderItem (1:N - one order, many items)

5. ORDER ITEM RELATIONSHIPS:
   
   OrderItem ──── belongs to ───► Order (N:1)
   OrderItem ──── references ───► MenuItem (N:1 - snapshot at order time)

6. MENU ITEM RELATIONSHIPS:
   
   MenuItem ──── belongs to ───► Restaurant (N:1)
   MenuItem ──── appears in ───► OrderItem (1:N - one menu item, many orders)
```

### Key Domain Patterns

#### Pattern 1: Owned Entities (Composition)
```
User ──[owns]──► Address ──[owns]──► Location
```
- **Address** doesn't exist independently; it's part of a User
- **Location** doesn't exist independently; it's part of an Address
- In database terms: Embedded in the same table (no separate Address table)
- **Why?** Performance - no JOINs needed, address always loaded with user

#### Pattern 2: Aggregates (Parent-Child)
```
Restaurant ──[owns]──► List<MenuItem>
Order ──[owns]──► List<OrderItem>
```
- **MenuItem** belongs to exactly one Restaurant
- **OrderItem** belongs to exactly one Order
- In database terms: Separate tables with foreign keys
- **Why?** Flexibility - restaurants can have 10-100 menu items

#### Pattern 3: References (Associations)
```
Order ───[references]───► User
Order ───[references]───► Restaurant
Order ───[references]───► Rider
```
- **Order** links to User, Restaurant, and Rider via IDs
- Multiple orders can reference the same user/restaurant/rider
- In database terms: Foreign key relationships
- **Why?** Many-to-one relationships, data normalization

#### Pattern 4: Snapshot Pattern
```
OrderItem stores MenuItem details at order time
```
- **OrderItem** captures menu item name and price when order is placed
- Even if restaurant later changes menu item price, order shows historical price
- In database terms: Denormalization for historical accuracy
- **Why?** Audit trail, price changes don't affect past orders

### Use Case Flows

#### Use Case 1: User Searches for Nearby Restaurants

```
1. User ──[has]──► Address ──[has]──► Location (lat: 19.0760, lon: 72.8777)
2. System queries all Restaurants
3. For each Restaurant ──[has]──► Address ──[has]──► Location:
   - Calculate distance using Haversine formula
   - Filter by Restaurant.DeliveryRadiusKm
   - Filter by Restaurant.IsOpen = true
4. Return sorted list of nearby restaurants
```

**Database Operations:**
- Read User.Address_Location_Lat, User.Address_Location_Lon
- Read all Restaurant.Address_Location_Lat, Restaurant.Address_Location_Lon
- Filter in application layer (distance calculation)

#### Use Case 2: User Views Restaurant Menu

```
1. User selects a Restaurant
2. System loads Restaurant ──[has]──► Menu (collection of MenuItem)
3. Filter by MenuItem.Available = true
4. Filter by MenuItem.CuisineType (if user specified)
5. Return MenuItem list with Name, Price, PreparationTime
```

**Database Operations:**
- Read Restaurant by ID
- JOIN to MenuItem table (or eager load owned collection)
- Filter MenuItem.Available = true

#### Use Case 3: User Places an Order

```
1. User selects MenuItem(s) from Restaurant's Menu
2. System creates Order:
   Order ──[placed by]──► User (UserId)
   Order ──[to]──► Restaurant (RestaurantId)
   Order ──[has]──► DeliveryAddress (copy from User.Address.Location)
   Order ──[contains]──► OrderItem(s):
     - Each OrderItem ──[references]──► MenuItem (MenuItemId)
     - Each OrderItem stores Name, Price (snapshot)
3. System finds nearest Rider:
   - Query all Rider where Status = 'Available'
   - Calculate distance from each Rider.CurrentLocation to Restaurant.Address.Location
   - Assign nearest Rider to Order (set Order.RiderId)
4. Update Order.Status = 'Preparing'
5. Update Rider.Status = 'Busy'
```

**Database Operations:**
- INSERT into Orders table (UserId, RestaurantId, DeliveryAddress)
- INSERT into OrderItem table (multiple rows, one per item)
- SELECT all Riders where Status = 'Available'
- UPDATE Order set RiderId = X
- UPDATE Rider set Status = 'Busy'

---

## Entity Relationships

### Complete Entity-Relationship Diagram

---

## Entity Relationships

### Complete Entity-Relationship Diagram

```
┌───────────────────────────────────────────────────────────────────────┐
│                  ENTITY-RELATIONSHIP DIAGRAM (ERD)                     │
│                    Food Delivery Platform                              │
└───────────────────────────────────────────────────────────────────────┘

                        ┌──────────────┐
                        │    Users     │ (Independent Entity)
                        ├──────────────┤
                        │ PK: Id       │
                        │ Name         │
                        │ Email        │
                        │ Phone        │
                        │ ┌──────────┐ │ (Owned Entity - Embedded)
                        │ │ Address  │ │
                        │ │ • Street │ │
                        │ │ • City   │ │
                        │ │ • ZipCode│ │
                        │ │ ┌──────┐ │ │ (Owned Entity - Nested)
                        │ │ │Loctn │ │ │
                        │ │ │• Lat │ │ │
                        │ │ │• Lon │ │ │
                        │ │ └──────┘ │ │
                        │ └──────────┘ │
                        └───────┬──────┘
                                │
                                │ 1:N (One User, Many Orders)
                                │
                                ▼
                        ┌──────────────┐
                   ┌───►│   Orders     │◄───┐ (Central Aggregate Root)
                   │    ├──────────────┤    │
                   │    │ PK: Id       │    │
                   │    │ FK: UserId   │────┘
                   │    │ FK: RestId   │────┐
                   │    │ FK: RiderId  │──┐ │
                   │    │ Total        │  │ │
                   │    │ Status       │  │ │
                   │    │ PaymentStatus│  │ │
                   │    │ Notes        │  │ │
                   │    │ ┌──────────┐ │  │ │ (Owned Entity - Embedded)
                   │    │ │Delivery  │ │  │ │
                   │    │ │Address   │ │  │ │
                   │    │ │• Lat     │ │  │ │
                   │    │ │• Lon     │ │  │ │
                   │    │ └──────────┘ │  │ │
                   │    │ CreatedAt    │  │ │
                   │    │ UpdatedAt    │  │ │
                   │    │ CompletedAt  │  │ │
                   │    └──────┬───────┘  │ │
                   │           │          │ │
                   │           │ 1:N      │ │
                   │           │          │ │
                   │           ▼          │ │
                   │    ┌──────────────┐  │ │
                   │    │  OrderItems  │  │ │ (Owned Collection)
                   │    ├──────────────┤  │ │
                   │    │ PK: Id       │  │ │
                   │    │ FK: OrderId  │  │ │
                   │    │ MenuItemId   │  │ │ (Reference, not FK)
                   │    │ Name         │  │ │ (Snapshot)
                   │    │ Price        │  │ │ (Snapshot)
                   │    │ Quantity     │  │ │
                   │    └──────────────┘  │ │
                   │                      │ │
         1:N       │                      │ │       1:N
       ┌───────────┘                      │ └───────────┐
       │                                  │             │
       │                                  │             │
       ▼                                  ▼             ▼
┌──────────────┐                  ┌──────────────┐  ┌──────────────┐
│ Restaurants  │ (Independent)    │    Riders    │  │    Riders    │
├──────────────┤                  ├──────────────┤  ├──────────────┤
│ PK: Id       │                  │ PK: Id       │  │ (Same Entity)│
│ Name         │                  │ Name         │  │              │
│ Phone        │                  │ Email        │  │ Status:      │
│ IsOpen       │                  │ Phone        │  │ • Available  │
│ DeliveryRadKm│                  │ VehicleNum   │  │ • Busy       │
│ AvgPrepTime  │                  │ RiderStatus  │  │ • Offline    │
│ Rating       │                  │ ┌──────────┐ │  │              │
│ ┌──────────┐ │                  │ │ Current  │ │  │ Gets assigned│
│ │ Address  │ │ (Owned - Embed)  │ │ Location │ │  │ to nearest   │
│ │ • Street │ │                  │ │ • Lat    │ │  │ available    │
│ │ • City   │ │                  │ │ • Lon    │ │  │ order        │
│ │ • ZipCode│ │                  │ │ • Updated│ │  │              │
│ │ ┌──────┐ │ │                  │ └──────────┘ │  └──────────────┘
│ │ │Loctn │ │ │                  │ CreatedAt    │
│ │ │• Lat │ │ │                  │ UpdatedAt    │
│ │ │• Lon │ │ │                  └──────────────┘
│ │ └──────┘ │ │
│ └──────────┘ │
│ CreatedAt    │
│ UpdatedAt    │
└──────┬───────┘
       │
       │ 1:N (One Restaurant, Many MenuItems)
       │
       ▼
┌──────────────┐
│  MenuItems   │ (Owned Collection / Child Entity)
├──────────────┤
│ PK: Id       │
│ FK: RestId   │───┐ (Implicit - owned by Restaurant)
│ Name         │   │
│ Price        │   │ CASCADE DELETE
│ Available    │   │ (Delete restaurant → delete all menu items)
│ CuisineType  │   │
│ PrepTimeMin  │   │
└──────────────┘   │
                   │
                   └──► When Restaurant deleted, all MenuItems deleted


RELATIONSHIP SUMMARY:
══════════════════════════════════════════════════════════════

1. User ──[1:N]──► Order
   - One user can place many orders
   - Foreign Key: Order.UserId → Users.Id
   - CASCADE: NO (keep order history even if user deleted)

2. Restaurant ──[1:N]──► Order
   - One restaurant can receive many orders
   - Foreign Key: Order.RestaurantId → Restaurants.Id
   - CASCADE: NO (keep historical orders)

3. Rider ──[1:N]──► Order
   - One rider can deliver many orders
   - Foreign Key: Order.RiderId → Riders.Id
   - CASCADE: NO (keep delivery history)

4. Order ──[1:N]──► OrderItem
   - One order contains many line items
   - Foreign Key: OrderItem.OrderId → Orders.Id
   - CASCADE: YES (delete order → delete all items)

5. Restaurant ──[1:N]──► MenuItem
   - One restaurant has many menu items
   - Foreign Key: MenuItem.RestaurantId → Restaurants.Id
   - CASCADE: YES (delete restaurant → delete menu)

6. User ──[owns]──► Address ──[owns]──► Location
   - Composition relationship (owned entities)
   - NO separate Address/Location tables
   - Embedded in Users table as columns:
     • Address_Street, Address_City, Address_ZipCode
     • Address_Location_Lat, Address_Location_Lon

7. Restaurant ──[owns]──► Address ──[owns]──► Location
   - Composition relationship (owned entities)
   - NO separate Address/Location tables
   - Embedded in Restaurants table as columns

8. Rider ──[owns]──► CurrentLocation
   - Composition relationship (owned entity)
   - NO separate Location table
   - Embedded in Riders table as columns:
     • CurrentLocation_Lat, CurrentLocation_Lon, LocationUpdatedAt

9. Order ──[owns]──► DeliveryAddress (Location)
   - Composition relationship (owned entity)
   - Embedded in Orders table as columns:
     • DeliveryAddress_Lat, DeliveryAddress_Lon
```

### Relationship Cardinality Explained

| Relationship | Type | Cardinality | Example |
|--------------|------|-------------|---------|
| **User → Order** | One-to-Many | 1:N | User#1 has Orders [101, 102, 103] |
| **Restaurant → Order** | One-to-Many | 1:N | Restaurant#5 has Orders [101, 104, 108] |
| **Rider → Order** | One-to-Many | 1:N | Rider#7 delivered Orders [102, 105] |
| **Order → OrderItem** | One-to-Many | 1:N | Order#101 has OrderItems [1, 2, 3] |
| **Restaurant → MenuItem** | One-to-Many | 1:N | Restaurant#5 has MenuItems [20, 21, 22...] |
| **User → Address** | One-to-One | 1:1 (Owned) | User#1 owns Address (embedded) |
| **Restaurant → Address** | One-to-One | 1:1 (Owned) | Restaurant#5 owns Address (embedded) |
| **Address → Location** | One-to-One | 1:1 (Owned) | Address owns Location (embedded) |

### Why Some Entities Are NOT Separate Tables

**Question:** Why don't we have separate `Addresses` and `Locations` tables?

**Answer:** **Owned Entity Pattern (Composition)**

```
❌ BAD DESIGN (3 Tables, 2 JOINs):

Users Table        Addresses Table       Locations Table
┌─────────┐       ┌──────────────┐      ┌─────────────┐
│ Id: 1   │──1:1─►│ Id: 101      │─1:1─►│ Id: 1001    │
│ Name    │       │ UserId: 1    │      │ AddressId   │
│ Email   │       │ Street       │      │ Lat: 19.076 │
└─────────┘       │ City         │      │ Lon: 72.877 │
                  │ LocationId   │      └─────────────┘
                  └──────────────┘

Query to get user location:
  SELECT u.*, a.*, l.*
  FROM Users u
  JOIN Addresses a ON u.Id = a.UserId
  JOIN Locations l ON a.LocationId = l.Id
  WHERE u.Id = 1
  
Result: 2 JOINs, slower, more complex


✅ GOOD DESIGN (1 Table, 0 JOINs):

Users Table (Flattened)
┌─────────────────────────┐
│ Id: 1                   │
│ Name                    │
│ Email                   │
│ Address_Street          │ ← Address embedded
│ Address_City            │
│ Address_ZipCode         │
│ Address_Location_Lat    │ ← Location nested within Address
│ Address_Location_Lon    │
└─────────────────────────┘

Query to get user location:
  SELECT * FROM Users WHERE Id = 1
  
Result: 0 JOINs, faster, simpler
```

**When to Use Owned Entities:**
- ✅ Entity has no meaning outside parent (Address without User is meaningless)
- ✅ Always loaded with parent (you always need address when loading user)
- ✅ One-to-one relationship
- ✅ Performance-critical queries

**When to Use Separate Tables:**
- ✅ Entity can exist independently (MenuItem can exist without Order)
- ✅ One-to-many relationship (one restaurant, many menu items)
- ✅ Shared across multiple parents (if Address could belong to User AND Business)
- ✅ Large collections (restaurant has 50+ menu items)

---

## How Relationships Map to Tables

### Database Technology
- **RDBMS**: SQL Server (2019+)
- **ORM**: Entity Framework Core 8
- **Schema Generation**: Code-First with Migrations
- **Current Scale**: Prototype (< 10,000 records)
- **Target Scale**: Production (10M+ users, 100K+ restaurants)

### Mapping Domain Concepts to Physical Tables

| Domain Concept | Database Table | Type | Why? |
|----------------|----------------|------|------|
| **User** | `Users` | Independent Entity | Primary actor, many orders |
| **User.Address** | (Embedded in `Users`) | Owned Entity | Always needed with user, 1:1 |
| **Address.Location** | (Embedded in `Users`) | Owned Entity | Part of address, no independent meaning |
| **Restaurant** | `Restaurants` | Independent Entity | Primary actor, many orders/menu items |
| **Restaurant.Address** | (Embedded in `Restaurants`) | Owned Entity | Always needed with restaurant, 1:1 |
| **Restaurant.Menu** | `MenuItem` table | Child Entity | Large collection (10-100 items), needs indexing |
| **MenuItem** | `MenuItem` | Child Entity | Separate table for querying/filtering |
| **Rider** | `Riders` | Independent Entity | Primary actor, many deliveries |
| **Rider.CurrentLocation** | (Embedded in `Riders`) | Owned Entity | Single location, frequently updated |
| **Order** | `Orders` | Independent Entity | Transaction record, references 3 entities |
| **Order.DeliveryAddress** | (Embedded in `Orders`) | Owned Entity | Snapshot at order time, 1:1 |
| **Order.Items** | `OrderItem` table | Child Entity | Collection of line items (1-10 per order) |
| **OrderItem** | `OrderItem` | Child Entity | Separate table for flexibility |

### Complete Table List

```
Physical Database Tables (5 tables total):
═══════════════════════════════════════════

1. Users
   ├── Columns: Id, Name, Email, Phone
   ├── Embedded: Address_Street, Address_City, Address_ZipCode
   └── Embedded: Address_Location_Lat, Address_Location_Lon

2. Restaurants
   ├── Columns: Id, Name, Phone, IsOpen, DeliveryRadiusKm, 
   │            AvgPrepTimeMinutes, Rating
   ├── Embedded: Address_Street, Address_City, Address_ZipCode
   └── Embedded: Address_Location_Lat, Address_Location_Lon

3. Riders
   ├── Columns: Id, Name, Email, Phone, VehicleNumber, RiderStatus
   └── Embedded: CurrentLocation_Lat, CurrentLocation_Lon, LocationUpdatedAt

4. Orders
   ├── Columns: Id, UserId (FK), RestaurantId (FK), RiderId (FK),
   │            Total, Status, PaymentStatus, Notes
   ├── Embedded: DeliveryAddress_Lat, DeliveryAddress_Lon
   └── Timestamps: CreatedAt, UpdatedAt, CompletedAt

5. MenuItem
   ├── Columns: Id, RestaurantId (FK), Name, Price, Available,
   │            CuisineType, PreparationTimeMinutes
   └── Cascade Delete: Deleted when parent Restaurant deleted

6. OrderItem
   ├── Columns: Id, OrderId (FK), MenuItemId (reference), Name (snapshot),
   │            Price (snapshot), Quantity
   └── Cascade Delete: Deleted when parent Order deleted


NO Separate Tables For:
═══════════════════════
❌ Addresses (embedded in Users, Restaurants)
❌ Locations (embedded in Users, Restaurants, Riders, Orders)
❌ Menu (collection name, not a table - MenuItems is the table)
```

---

## Complete Table Structures

### 1. Users Table

**Purpose:** Store customer information and delivery addresses

```sql
CREATE TABLE [dbo].[Users] (
    [Id]              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]            NVARCHAR(100) NOT NULL,
    [Email]           NVARCHAR(256) NULL,
    [Phone]           NVARCHAR(20) NULL,
    
    -- Embedded Address (Owned Entity)
    [Address_Street]  NVARCHAR(200) NULL,
    [Address_City]    NVARCHAR(100) NULL,
    [Address_ZipCode] NVARCHAR(20) NULL,
    [Address_Location_Lat] FLOAT NOT NULL,  -- Latitude
    [Address_Location_Lon] FLOAT NOT NULL,  -- Longitude
    
    -- Metadata
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]       DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)

-- Indexes
CREATE UNIQUE INDEX [IX_Users_Name] ON [Users] ([Name])
CREATE INDEX [IX_Users_Email] ON [Users] ([Email])
CREATE INDEX [IX_Users_City] ON [Users] ([Address_City])  -- For regional queries
CREATE INDEX [IX_Users_Location] ON [Users] ([Address_Location_Lat], [Address_Location_Lon])  -- Spatial
```

**Storage Estimation (Per User):**
- Base data: ~200 bytes
- Indexes: ~100 bytes
- **Total per user: ~300 bytes**
- **10M users: ~3 GB**

**Key Design Decisions:**
- ✅ **Owned Entity Pattern**: Address embedded in Users table (no JOIN needed)
- ✅ **Unique Name**: Simplified for prototype (would use Email in production)
- ✅ **Denormalized Location**: Lat/Lon stored directly for fast geospatial queries
- ✅ **City Index**: Enables city-level filtering before complex geo calculations

---

### 2. Restaurants Table

**Purpose:** Store restaurant profiles, locations, and operational settings

```sql
CREATE TABLE [dbo].[Restaurants] (
    [Id]                          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]                        NVARCHAR(200) NOT NULL,
    [Phone]                       NVARCHAR(20) NULL,
    [IsOpen]                      BIT NOT NULL DEFAULT 1,
    [DeliveryRadiusKm]            FLOAT NOT NULL DEFAULT 5.0,
    [AveragePreparationTimeMinutes] INT NOT NULL DEFAULT 20,
    [Rating]                      DECIMAL(3,2) NOT NULL DEFAULT 4.0,
    
    -- Embedded Address (Owned Entity)
    [Address_Street]              NVARCHAR(200) NULL,
    [Address_City]                NVARCHAR(100) NULL,
    [Address_ZipCode]             NVARCHAR(20) NULL,
    [Address_Location_Lat]        FLOAT NOT NULL,
    [Address_Location_Lon]        FLOAT NOT NULL,
    
    -- Metadata
    [CreatedAt]                   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]                   DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)

-- Indexes
CREATE UNIQUE INDEX [IX_Restaurants_Name] ON [Restaurants] ([Name])
CREATE INDEX [IX_Restaurants_IsOpen] ON [Restaurants] ([IsOpen])  -- Filter closed restaurants
CREATE INDEX [IX_Restaurants_City] ON [Restaurants] ([Address_City])  -- City-based search
CREATE INDEX [IX_Restaurants_Location] ON [Restaurants] ([Address_Location_Lat], [Address_Location_Lon])  -- Geo
CREATE INDEX [IX_Restaurants_Rating] ON [Restaurants] ([Rating] DESC)  -- Popular restaurants
```

**Related Table: Menu Items (Owned Collection)**

```sql
CREATE TABLE [dbo].[MenuItem] (
    [Id]                      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [RestaurantId]            INT NOT NULL,  -- FK to Restaurants
    [Name]                    NVARCHAR(200) NOT NULL,
    [Price]                   DECIMAL(18,2) NOT NULL,
    [Available]               BIT NOT NULL DEFAULT 1,
    [CuisineType]             NVARCHAR(50) NOT NULL,  -- Enum: NorthIndian, Chinese, etc.
    [PreparationTimeMinutes]  INT NOT NULL DEFAULT 20,
    
    CONSTRAINT [FK_MenuItem_Restaurant] FOREIGN KEY ([RestaurantId]) 
        REFERENCES [Restaurants]([Id]) ON DELETE CASCADE
)

-- Indexes
CREATE INDEX [IX_MenuItem_RestaurantId] ON [MenuItem] ([RestaurantId])
CREATE INDEX [IX_MenuItem_CuisineType] ON [MenuItem] ([CuisineType])  -- Filter by cuisine
CREATE INDEX [IX_MenuItem_Available] ON [MenuItem] ([Available])  -- Only show available items
CREATE INDEX [IX_MenuItem_Restaurant_Cuisine] ON [MenuItem] ([RestaurantId], [CuisineType])  -- Composite
```

**Storage Estimation:**
- **Restaurant record**: ~350 bytes
- **Menu item**: ~150 bytes × 50 items/restaurant = ~7.5 KB per restaurant
- **100K restaurants**: ~750 MB (base) + ~750 MB (menu) = **~1.5 GB**

---

### 3. Riders Table

**Purpose:** Store delivery driver information and real-time location

```sql
CREATE TABLE [dbo].[Riders] (
    [Id]                   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name]                 NVARCHAR(100) NOT NULL,
    [Email]                NVARCHAR(256) NULL,
    [Phone]                NVARCHAR(20) NULL,
    [VehicleNumber]        NVARCHAR(50) NULL,
    [RiderStatus]          NVARCHAR(50) NOT NULL DEFAULT 'Available',  -- Available, Busy, Offline
    
    -- Current Location (Updated frequently)
    [CurrentLocation_Lat]  FLOAT NOT NULL,
    [CurrentLocation_Lon]  FLOAT NOT NULL,
    [LocationUpdatedAt]    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Metadata
    [CreatedAt]            DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]            DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)

-- Indexes
CREATE UNIQUE INDEX [IX_Riders_Name] ON [Riders] ([Name])
CREATE INDEX [IX_Riders_Email] ON [Riders] ([Email])
CREATE INDEX [IX_Riders_Status] ON [Riders] ([RiderStatus])  -- CRITICAL: Find available riders fast
CREATE INDEX [IX_Riders_Location] ON [Riders] ([CurrentLocation_Lat], [CurrentLocation_Lon])  -- Geo
CREATE INDEX [IX_Riders_Status_Location] ON [Riders] ([RiderStatus], [CurrentLocation_Lat], [CurrentLocation_Lon])  -- Composite
```

**Key Design Decisions:**
- ✅ **Frequent Updates**: Location updated every 10-30 seconds during active delivery
- ✅ **Status Index**: Critical for `WHERE RiderStatus = 'Available'` queries
- ✅ **Composite Index**: Optimizes the most common query: "Find available riders near X"
- ⚠️ **Hot Spot**: High write contention during peak hours

**Storage Estimation:**
- **Per rider**: ~250 bytes
- **100K riders**: ~25 MB (negligible)

---

### 4. Orders Table (Central Entity)

**Purpose:** Track all orders, their items, status, and lifecycle

```sql
CREATE TABLE [dbo].[Orders] (
    [Id]                       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId]                   INT NOT NULL,
    [RestaurantId]             INT NOT NULL,
    [RiderId]                  INT NULL,  -- Assigned later
    [Total]                    DECIMAL(18,2) NOT NULL,
    [Status]                   NVARCHAR(50) NOT NULL,  -- Placed, Accepted, Preparing, PickedUp, Delivered, Cancelled
    [PaymentStatus]            NVARCHAR(50) NOT NULL DEFAULT 'NotPaid',  -- NotPaid, Paid, Failed, Refunded
    [Notes]                    NVARCHAR(500) NULL,
    
    -- Delivery Address (Owned Entity)
    [DeliveryAddress_Lat]      FLOAT NOT NULL,
    [DeliveryAddress_Lon]      FLOAT NOT NULL,
    
    -- Timestamps (Lifecycle tracking)
    [CreatedAt]                DATETIME2 NOT NULL DEFAULT GETUTCDATE(),  -- Order placed
    [UpdatedAt]                DATETIME2 NOT NULL DEFAULT GETUTCDATE(),  -- Last status change
    [CompletedAt]              DATETIME2 NULL,  -- Delivered or cancelled
    
    -- Foreign Keys
    CONSTRAINT [FK_Orders_Users] FOREIGN KEY ([UserId]) 
        REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Orders_Restaurants] FOREIGN KEY ([RestaurantId]) 
        REFERENCES [Restaurants]([Id]),
    CONSTRAINT [FK_Orders_Riders] FOREIGN KEY ([RiderId]) 
        REFERENCES [Riders]([Id])
)

-- Indexes (Critical for Performance)
CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId])  -- User order history
CREATE INDEX [IX_Orders_RestaurantId] ON [Orders] ([RestaurantId])  -- Restaurant dashboard
CREATE INDEX [IX_Orders_RiderId] ON [Orders] ([RiderId])  -- Rider assignments
CREATE INDEX [IX_Orders_Status] ON [Orders] ([Status])  -- Active orders
CREATE INDEX [IX_Orders_CreatedAt] ON [Orders] ([CreatedAt] DESC)  -- Recent orders
CREATE INDEX [IX_Orders_CompletedAt] ON [Orders] ([CompletedAt] DESC)  -- Historical analysis
CREATE INDEX [IX_Orders_User_CreatedAt] ON [Orders] ([UserId], [CreatedAt] DESC)  -- User history optimization
```

**Related Table: Order Items (Owned Collection)**

```sql
CREATE TABLE [dbo].[OrderItem] (
    [Id]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrderId]      INT NOT NULL,  -- FK to Orders
    [MenuItemId]   INT NOT NULL,  -- Reference to original menu item
    [Name]         NVARCHAR(200) NOT NULL,  -- Snapshot at order time
    [Price]        DECIMAL(18,2) NOT NULL,  -- Snapshot at order time
    [Quantity]     INT NOT NULL,
    
    CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderId]) 
        REFERENCES [Orders]([Id]) ON DELETE CASCADE
)

-- Indexes
CREATE INDEX [IX_OrderItem_OrderId] ON [OrderItem] ([OrderId])
CREATE INDEX [IX_OrderItem_MenuItemId] ON [OrderItem] ([MenuItemId])  -- Analytics
```

**Storage Estimation:**
- **Order record**: ~250 bytes
- **Order items**: ~100 bytes × 3 items/order = ~300 bytes
- **Total per order**: ~550 bytes
- **100M orders/year**: ~55 GB/year
- **5 years retention**: ~275 GB

**Key Design Decisions:**
- ✅ **Snapshot Pattern**: Order items store menu item details (price, name) at order time
- ✅ **Denormalized Delivery Address**: No JOIN to Users table needed
- ✅ **Lifecycle Timestamps**: CreatedAt, UpdatedAt, CompletedAt enable time-series analysis
- ✅ **Composite Indexes**: Optimize common query patterns (user history, restaurant orders)

---

## Entity Relationships

### ER Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    Entity Relationships                         │
└─────────────────────────────────────────────────────────────────┘

┌─────────┐                                            ┌─────────┐
│  Users  │                                            │  Riders │
│  (10M)  │                                            │ (100K)  │
└────┬────┘                                            └────┬────┘
     │                                                      │
     │ 1:N                                             1:N  │
     │                                                      │
     │              ┌──────────────┐                        │
     └─────────────►│    Orders    │◄───────────────────────┘
                    │  (100M/year) │
     ┌─────────────►│              │
     │              └──────┬───────┘
     │ 1:N                 │
     │                     │ 1:N
┌────┴──────┐              │              ┌─────────────┐
│Restaurant │              │              │ OrderItems  │
│  (100K)   │              └─────────────►│  (300M/yr)  │
└────┬──────┘                             └─────────────┘
     │ 1:N
     │
     │              ┌─────────────┐
     └─────────────►│ MenuItems   │
                    │  (5M items) │
                    └─────────────┘

Cardinality:
  1:N = One-to-Many
  User → Orders: One user can place many orders
  Restaurant → Orders: One restaurant can receive many orders
  Rider → Orders: One rider can deliver many orders
  Restaurant → MenuItems: One restaurant has many menu items
  Order → OrderItems: One order contains many items
```

### Relationship Details

| Parent | Child | Type | Cascade Delete | Notes |
|--------|-------|------|----------------|-------|
| **Users** | Orders | 1:N | No | Keep order history even if user deleted |
| **Restaurants** | Orders | 1:N | No | Keep historical orders |
| **Restaurants** | MenuItems | 1:N | Yes | Delete menu items when restaurant deleted |
| **Riders** | Orders | 1:N | No | Keep delivery history |
| **Orders** | OrderItems | 1:N | Yes | Order items have no meaning without order |

---

## Indexes & Performance

### Index Strategy

#### 1. Primary Indexes (Clustered)
```sql
-- All tables use INT IDENTITY(1,1) as clustered PK
-- Benefits:
--   ✓ Sequential inserts (no page splits)
--   ✓ Efficient range scans
--   ✓ Smaller index size than GUID
```

#### 2. Foreign Key Indexes
```sql
-- Automatically created on FK columns:
CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId])
CREATE INDEX [IX_Orders_RestaurantId] ON [Orders] ([RestaurantId])
CREATE INDEX [IX_Orders_RiderId] ON [Orders] ([RiderId])
```

#### 3. Status/Filter Indexes
```sql
-- Enable fast filtering:
CREATE INDEX [IX_Restaurants_IsOpen] ON [Restaurants] ([IsOpen])
CREATE INDEX [IX_Riders_Status] ON [Riders] ([RiderStatus])
CREATE INDEX [IX_Orders_Status] ON [Orders] ([Status])
CREATE INDEX [IX_MenuItem_Available] ON [MenuItem] ([Available])
```

#### 4. Geospatial Indexes
```sql
-- Current approach: Composite indexes
CREATE INDEX [IX_Riders_Location] 
    ON [Riders] ([CurrentLocation_Lat], [CurrentLocation_Lon])

-- Future: SQL Server Spatial Types
CREATE SPATIAL INDEX [SIX_Riders_Location] 
    ON [Riders] ([Location])
```

#### 5. Composite Indexes (Query Optimization)
```sql
-- Rider assignment query: "Find available riders near location"
CREATE INDEX [IX_Riders_Status_Location] 
    ON [Riders] ([RiderStatus], [CurrentLocation_Lat], [CurrentLocation_Lon])
    INCLUDE ([Name], [Phone], [VehicleNumber])

-- User order history: "Show recent orders for user X"
CREATE INDEX [IX_Orders_User_CreatedAt] 
    ON [Orders] ([UserId], [CreatedAt] DESC)
    INCLUDE ([RestaurantId], [Status], [Total])

-- Restaurant menu filtering: "Get available NorthIndian items from restaurant X"
CREATE INDEX [IX_MenuItem_Restaurant_Cuisine] 
    ON [MenuItem] ([RestaurantId], [CuisineType], [Available])
    INCLUDE ([Name], [Price], [PreparationTimeMinutes])
```

### Index Maintenance

**Update Statistics:**
```sql
-- Run weekly during low-traffic periods
UPDATE STATISTICS [Users] WITH FULLSCAN;
UPDATE STATISTICS [Orders] WITH FULLSCAN;
UPDATE STATISTICS [Riders] WITH FULLSCAN;
```

**Rebuild Fragmented Indexes:**
```sql
-- Rebuild indexes with >30% fragmentation monthly
ALTER INDEX ALL ON [Orders] REBUILD;
```

---

## Data Types & Constraints

### Rationale for Data Type Choices

| Field | Type | Reasoning |
|-------|------|-----------|
| **ID columns** | `INT IDENTITY(1,1)` | • 2.1 billion max (sufficient for MVP)<br>• Sequential (no fragmentation)<br>• 4 bytes vs 16 bytes (GUID)<br>• Future: `BIGINT` if needed |
| **Prices** | `DECIMAL(18,2)` | • Exact precision (no floating point errors)<br>• Range: ±999,999,999,999,999.99<br>• Standard for financial data |
| **Ratings** | `DECIMAL(3,2)` | • Range: 0.00 - 9.99 (we use 0-5)<br>• Precision for half-stars (4.5★) |
| **GPS Coordinates** | `FLOAT` | • Latitude: -90 to 90<br>• Longitude: -180 to 180<br>• 6 decimal places = ~11 cm accuracy<br>• Double precision sufficient |
| **Status/Enums** | `NVARCHAR(50)` | • Human-readable in DB<br>• Easy debugging<br>• Future: Consider TINYINT for space savings |
| **Timestamps** | `DATETIME2` | • Range: 0001-01-01 to 9999-12-31<br>• 100ns precision<br>• UTC storage (no timezone issues) |
| **Text fields** | `NVARCHAR(N)` | • Unicode support (international names)<br>• Limits prevent abuse:<br>  - Name: 100 chars<br>  - Address: 200 chars<br>  - Email: 256 chars |

### Constraint Strategy

```sql
-- Check constraints for data integrity
ALTER TABLE [Orders] 
ADD CONSTRAINT [CK_Orders_Total_Positive] CHECK ([Total] > 0)

ALTER TABLE [OrderItem] 
ADD CONSTRAINT [CK_OrderItem_Quantity_Positive] CHECK ([Quantity] > 0)

ALTER TABLE [Restaurants] 
ADD CONSTRAINT [CK_Restaurants_Rating_Range] CHECK ([Rating] >= 0 AND [Rating] <= 5)

ALTER TABLE [Restaurants] 
ADD CONSTRAINT [CK_Restaurants_DeliveryRadius_Positive] CHECK ([DeliveryRadiusKm] > 0)

-- Default values for operational fields
ALTER TABLE [Restaurants] 
ADD CONSTRAINT [DF_Restaurants_IsOpen] DEFAULT 1 FOR [IsOpen]

ALTER TABLE [MenuItem] 
ADD CONSTRAINT [DF_MenuItem_Available] DEFAULT 1 FOR [Available]
```

---

## Scalability Strategy

### Phase 1: Current (Prototype)
**Target:** < 10,000 users, < 1,000 restaurants, < 50,000 orders

```
┌────────────────────────────────┐
│     Single SQL Server          │
│                                │
│  • Standard Edition            │
│  • 4 CPU cores                 │
│  • 16 GB RAM                   │
│  • 500 GB SSD                  │
│                                │
│  ✓ No caching                  │
│  ✓ No replication              │
│  ✓ All reads/writes to primary │
└────────────────────────────────┘
```

**Bottleneck:** None (over-provisioned for load)

---

### Phase 2: Growth (0-100K users)
**Target:** 100,000 users, 5,000 restaurants, 1M orders

```
┌─────────────────────────────────────────────┐
│           Read Replica Pattern               │
└─────────────────────────────────────────────┘

   ┌──────────────┐
   │  App Server  │
   └──┬────────┬──┘
      │        │
      │ Write  │ Read
      ▼        ▼
┌─────────┐  ┌─────────┐
│ Primary │──►│ Replica │
│  (RW)   │  │  (RO)   │
└─────────┘  └─────────┘
     │           │
     └───────────┘
    Synchronous
    Replication
```

**Implementation:**
- **Primary**: Handle all writes (INSERT, UPDATE, DELETE)
- **Replica**: Handle read queries (SELECT)
- **App Logic**: Route reads to replica, writes to primary
- **Benefit**: 2x read capacity, zero impact on writes

**Estimated Performance:**
- Writes: 1,000 orders/minute
- Reads: 10,000 queries/minute
- **Latency**: <50ms (95th percentile)

---

### Phase 3: Scale (100K-1M users)
**Target:** 1M users, 20K restaurants, 20M orders

```
┌──────────────────────────────────────────────────────┐
│         Multi-Replica + Caching Layer                │
└──────────────────────────────────────────────────────┘

          ┌──────────────┐
          │  App Server  │
          └──┬────────┬──┘
             │        │
        Write│        │Read (cache miss)
             │        │
             ▼        ▼
       ┌─────────┐  ┌───────────┐
       │ Primary │  │   Redis   │ ◄── Cache Layer
       │  (RW)   │  │  Cache    │     (Hot data)
       └────┬────┘  └───────────┘
            │
            │ Replicate
            ├──────────┬──────────┐
            ▼          ▼          ▼
       ┌─────────┐┌─────────┐┌─────────┐
       │Replica 1││Replica 2││Replica 3│
       │  (RO)   ││  (RO)   ││  (RO)   │
       └─────────┘└─────────┘└─────────┘
```

**Caching Strategy:**
- **Redis Cache**: Store hot data (restaurant menus, user profiles)
- **TTL**: 5-15 minutes
- **Invalidation**: On write, purge cache entry
- **Hit Rate Target**: 80%+

**Estimated Performance:**
- Writes: 5,000 orders/minute
- Reads: 50,000 queries/minute (80% cache hit)
- **Latency**: <20ms (95th percentile from cache)

---

### Phase 4: Massive Scale (1M-10M users)
**Target:** 10M users, 100K restaurants, 200M orders

```
┌─────────────────────────────────────────────────────────┐
│        Horizontal Sharding + Geographic Distribution     │
└─────────────────────────────────────────────────────────┘

            ┌──────────────┐
            │  API Gateway │
            │  (Routing)   │
            └──────┬───────┘
                   │
       ┌───────────┼───────────┐
       │           │           │
       ▼           ▼           ▼
┌─────────────┐┌─────────────┐┌─────────────┐
│  Shard 1    ││  Shard 2    ││  Shard 3    │
│ (Region: US)││(Region: EU) ││(Region: Asia)│
│             ││             ││             │
│ ┌─────────┐ ││ ┌─────────┐ ││ ┌─────────┐ │
│ │ Primary │ ││ │ Primary │ ││ │ Primary │ │
│ └────┬────┘ ││ └────┬────┘ ││ └────┬────┘ │
│      │      ││      │      ││      │      │
│ ┌────┴────┐ ││ ┌────┴────┐ ││ ┌────┴────┐ │
│ │Replicas │ ││ │Replicas │ ││ │Replicas │ │
│ └─────────┘ ││ └─────────┘ ││ └─────────┘ │
└─────────────┘└─────────────┘└─────────────┘
```

**Sharding Strategy Options:**

#### Option A: Geographic Sharding
```
Shard Key: City/Region

Shard 1: US East (New York, Boston, Philadelphia)
Shard 2: US West (SF, LA, Seattle)
Shard 3: Europe (London, Paris, Berlin)
Shard 4: Asia (Mumbai, Delhi, Bangalore)
```

**Pros:**
- ✅ Low latency (data close to users)
- ✅ Regulatory compliance (GDPR, data residency)
- ✅ Natural isolation (US orders don't query EU data)

**Cons:**
- ❌ Uneven load (some cities much larger)
- ❌ User travel (cross-region queries)

#### Option B: User ID Range Sharding
```
Shard Key: User ID % N

Shard 1: UserID 0-3,333,333
Shard 2: UserID 3,333,334-6,666,667
Shard 3: UserID 6,666,668-10,000,000
```

**Pros:**
- ✅ Even distribution
- ✅ Simple routing logic
- ✅ Deterministic (same user always goes to same shard)

**Cons:**
- ❌ No geographic locality
- ❌ Cross-shard queries for restaurants

#### Option C: Hybrid (Recommended)
```
Shard Key: City Hash + User ID

Logic:
  shard_id = HASH(city) % num_shards_per_region
  region = lookup_region(city)
  route_to(region, shard_id)
```

**Pros:**
- ✅ Geographic locality
- ✅ Even load within region
- ✅ Scalable per-region

**Implementation:**
```csharp
public int GetShardId(string city, int userId)
{
    var region = GetRegion(city);  // US, EU, Asia
    var shardsInRegion = GetShardsForRegion(region);  // 10 shards/region
    var shardIndex = Math.Abs(city.GetHashCode() % shardsInRegion.Count);
    return shardsInRegion[shardIndex];
}
```

---

## Sharding & Partitioning

### Table Partitioning (Within a Shard)

**Orders Table**: Partition by date (monthly)

```sql
-- Create partition function
CREATE PARTITION FUNCTION PF_OrdersByMonth (DATETIME2)
AS RANGE RIGHT FOR VALUES (
    '2025-01-01',  -- Jan 2025
    '2025-02-01',  -- Feb 2025
    '2025-03-01',  -- Mar 2025
    ...
    '2026-01-01'   -- Jan 2026
);

-- Create partition scheme
CREATE PARTITION SCHEME PS_OrdersByMonth
AS PARTITION PF_OrdersByMonth
TO (FG_Jan2025, FG_Feb2025, FG_Mar2025, ..., FG_Jan2026);

-- Create partitioned table
CREATE TABLE [Orders_Partitioned] (
    [Id] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    ...
) ON PS_OrdersByMonth([CreatedAt]);
```

**Benefits:**
- ✅ **Query Performance**: Filter by date hits only relevant partitions
- ✅ **Archive Strategy**: Drop old partitions (e.g., > 2 years)
- ✅ **Maintenance**: Rebuild indexes per partition
- ✅ **Parallel Processing**: Query multiple partitions in parallel

**Example Query:**
```sql
-- Only scans Dec 2025 partition
SELECT * FROM Orders_Partitioned
WHERE CreatedAt >= '2025-12-01' AND CreatedAt < '2026-01-01'
```

---

## Caching Strategy

### What to Cache

| Data Type | Cache? | TTL | Invalidation | Reasoning |
|-----------|--------|-----|--------------|-----------|
| **Restaurant Menus** | ✅ Yes | 15 min | On menu update | High read, low write |
| **User Profiles** | ✅ Yes | 10 min | On profile update | Frequent reads (every order) |
| **Rider Locations** | ❌ No | - | - | Changes every 10s (too volatile) |
| **Order Status** | ⚠️ Selective | 30 sec | On status change | Only active orders |
| **Restaurant List (by city)** | ✅ Yes | 30 min | On new restaurant | Popular pages |
| **Recommendation Results** | ✅ Yes | 5 min | On restaurant/menu change | Computationally expensive |

### Cache Keys

```
Format: {entity}:{id}:{context}

Examples:
  restaurant:123:menu          → Restaurant #123's full menu
  user:456:profile             → User #456's profile
  recommendations:789:Italian  → User #789's Italian restaurant recommendations
  restaurant_list:mumbai       → All restaurants in Mumbai
```

### Cache Invalidation Strategies

#### 1. Time-Based (TTL)
```csharp
// Set with expiration
await cache.SetAsync(
    key: $"restaurant:{id}:menu",
    value: menuData,
    expiration: TimeSpan.FromMinutes(15)
);
```

#### 2. Event-Based (Write-Through)
```csharp
// On restaurant menu update
public async Task UpdateMenuItemAsync(int restaurantId, MenuItem item)
{
    await _repository.UpdateAsync(item);  // Database write
    await _cache.RemoveAsync($"restaurant:{restaurantId}:menu");  // Invalidate cache
}
```

#### 3. Lazy Refresh (Cache-Aside)
```csharp
public async Task<Menu> GetRestaurantMenuAsync(int restaurantId)
{
    // Try cache first
    var cached = await _cache.GetAsync<Menu>($"restaurant:{restaurantId}:menu");
    if (cached != null) return cached;
    
    // Cache miss - fetch from DB
    var menu = await _repository.GetMenuAsync(restaurantId);
    
    // Store in cache for next time
    await _cache.SetAsync($"restaurant:{restaurantId}:menu", menu, TimeSpan.FromMinutes(15));
    
    return menu;
}
```

---

## Database Optimization Techniques

### 1. Query Optimization

**Bad Query (N+1 Problem):**
```csharp
// Fetches orders, then loops and queries for each user/restaurant
var orders = await _context.Orders.ToListAsync();
foreach (var order in orders)
{
    var user = await _context.Users.FindAsync(order.UserId);  // N queries!
    var restaurant = await _context.Restaurants.FindAsync(order.RestaurantId);
}
```

**Good Query (Eager Loading):**
```csharp
// Single query with JOINs
var orders = await _context.Orders
    .Include(o => o.User)
    .Include(o => o.Restaurant)
    .Include(o => o.Rider)
    .Include(o => o.Items)
    .ToListAsync();
```

### 2. Projection (Select Only Needed Columns)

**Bad:**
```csharp
// Fetches all columns
var users = await _context.Users.ToListAsync();
```

**Good:**
```csharp
// Fetches only needed columns
var users = await _context.Users
    .Select(u => new { u.Id, u.Name, u.Email })
    .ToListAsync();
```

**Savings:** 200 bytes → 50 bytes per row (75% reduction)

### 3. Batch Operations

**Bad (Individual Inserts):**
```csharp
foreach (var item in items)
{
    await _context.MenuItems.AddAsync(item);
    await _context.SaveChangesAsync();  // N round trips!
}
```

**Good (Batch Insert):**
```csharp
await _context.MenuItems.AddRangeAsync(items);  // Single round trip
await _context.SaveChangesAsync();
```

**Performance:** 1000 items: 30 seconds → 0.5 seconds (60x faster)

### 4. Asynchronous Operations

**Always use async/await for I/O:**
```csharp
// ✅ Good - Non-blocking
var orders = await _context.Orders.ToListAsync();

// ❌ Bad - Blocks thread
var orders = _context.Orders.ToList();
```

### 5. Connection Pooling

**Configuration (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Min Pool Size=10;Max Pool Size=100;..."
  }
}
```

**Rationale:**
- Reuses connections (avoids overhead of opening/closing)
- Min Pool: Always keep 10 connections warm
- Max Pool: Cap at 100 to avoid overwhelming database

---

## Monitoring & Maintenance

### Key Metrics to Track

| Metric | Tool | Alert Threshold | Action |
|--------|------|-----------------|--------|
| **Query Duration** | SQL Profiler | > 1 second | Optimize query/add index |
| **CPU Usage** | SQL Server DMVs | > 80% for 5 min | Scale up or optimize |
| **Disk I/O Wait** | Performance Monitor | > 15ms avg | Upgrade to faster storage |
| **Index Fragmentation** | `sys.dm_db_index_physical_stats` | > 30% | Rebuild indexes |
| **Blocking Queries** | `sys.dm_exec_requests` | > 10 seconds | Kill long-running queries |
| **Connection Pool** | App Insights | > 80% utilized | Increase max pool size |
| **Cache Hit Rate** | Redis metrics | < 70% | Review cache strategy |

### Monitoring Queries

**Find slow queries:**
```sql
SELECT TOP 10
    qs.execution_count,
    qs.total_elapsed_time / qs.execution_count AS avg_elapsed_time,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS statement_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY avg_elapsed_time DESC;
```

**Check index fragmentation:**
```sql
SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 30
ORDER BY ips.avg_fragmentation_in_percent DESC;
```

### Maintenance Windows

**Weekly (Low-traffic hours):**
- Update statistics
- Rebuild fragmented indexes (>30%)
- Clear old cache entries

**Monthly:**
- Full backup
- Audit slow queries
- Review and optimize top 10 slowest queries

**Quarterly:**
- Capacity planning review
- Archive old data (> 2 years)
- Update disaster recovery plan

---

## Migration Path: Current → Production Scale

```
┌──────────────────────────────────────────────────────────────┐
│             Scalability Roadmap Timeline                      │
└──────────────────────────────────────────────────────────────┘

Month 0-3: Single Server (Current)
  → Launch MVP
  → Gather metrics
  → Identify bottlenecks

Month 4-6: Add Read Replicas
  → Deploy 2 read replicas
  → Route 80% reads to replicas
  → 3x read capacity

Month 7-12: Implement Caching
  → Deploy Redis cluster
  → Cache hot data (menus, profiles)
  → 80%+ cache hit rate
  → 10x read performance improvement

Month 13-18: Database Optimization
  → Add missing indexes (based on slow query log)
  → Implement table partitioning (Orders by month)
  → Query optimization (eliminate N+1 queries)
  → Connection pool tuning

Month 19-24: Geographic Sharding
  → Shard by city/region
  → Deploy 3 regional clusters (US, EU, Asia)
  → Implement cross-shard query layer
  → 100x capacity vs. single server

Month 25+: Continuous Optimization
  → Monitor and tune
  → Add shards as needed
  → Implement machine learning for predictive scaling
```

---

## Summary: Capacity Planning

### Current Capacity (Phase 1)

| Resource | Limit | Utilization |
|----------|-------|-------------|
| **Users** | 10,000 | 0.1% |
| **Restaurants** | 1,000 | 1% |
| **Orders/day** | 5,000 | <1% CPU |
| **Storage** | 10 GB | 500 GB available |

### Target Capacity (Phase 4)

| Resource | Limit | Infrastructure |
|----------|-------|----------------|
| **Users** | 10,000,000 | 30 shards @ 333K users/shard |
| **Restaurants** | 100,000 | Distributed across shards |
| **Orders/day** | 1,000,000 | 12 orders/second average, 50 peak |
| **Storage** | 500 GB/shard | 15 TB total (30 shards × 500 GB) |
| **Read Throughput** | 100,000 QPS | Cache + 90 read replicas (30 shards × 3 replicas) |
| **Write Throughput** | 10,000 TPS | 30 primary databases (1 per shard) |

### Cost Estimation (Monthly)

| Phase | Users | Infrastructure | Cost/Month |
|-------|-------|----------------|------------|
| **Phase 1** | <10K | 1 SQL Server (Standard) | $500 |
| **Phase 2** | <100K | 1 Primary + 1 Replica | $1,200 |
| **Phase 3** | <1M | 1 Primary + 3 Replicas + Redis | $4,000 |
| **Phase 4** | <10M | 30 Shards (90 DB instances) + Redis Cluster | $35,000 |

---