# Documentation Index

This file provides a quick reference to all documentation available for the Food Delivery platform.

## 📚 Complete Documentation Structure

**Recommended Reading Order:**

```
1. README.md                    → Start here: Setup & overview
2. DATABASE_SCHEMA.md           → Foundation: Data model & relationships
3. TECHNICAL_DESIGN.md          → Blueprint: Business logic & algorithms
4. API_DOCUMENTATION.md         → Interface: API endpoints & usage
5. TESTING_GUIDE.md             → Practice: Hands-on testing tutorial
6. DESIGN_PRESENTATION.md       → Complete: System architecture overview
```

**All Documentation Files:**

```
FoodDeliveryPolaris/
├── README.md                          # Main project overview & getting started
├── docs/
│   ├── DATABASE_SCHEMA.md            # Database architecture & scalability
│   ├── TECHNICAL_DESIGN.md           # Algorithms, assumptions & logic
│   ├── API_DOCUMENTATION.md          # Complete API reference
│   ├── TESTING_GUIDE.md              # Step-by-step testing tutorial
│   ├── DESIGN_PRESENTATION.md        # System design presentation
│   └── DOCUMENTATION_INDEX.md        # This file
```

---

## 1️⃣ README.md - Project Overview

**Purpose:** First stop for anyone new to the project

**What's Inside:**
- ✅ What the application does
- ✅ Technology stack
- ✅ How to run the application
- ✅ Project structure
- ✅ Quick examples
- ✅ Configuration guide
- ✅ Troubleshooting

**Best For:**
- New developers joining the project
- Setting up local development environment
- Quick reference for running the app
- Understanding project structure

**Start Here If:** You're new to the project or need to set it up

---

## 2️⃣ docs/DATABASE_SCHEMA.md - Database Architecture (READ THIS FIRST)

**Purpose:** Complete database design documentation with scaling strategies for production

**Why Read This First?**
- 🏗️ **Foundation Layer** - Understand the data model before learning business logic
- 🎯 **Domain-Driven** - Starts with real-world concepts (User, Restaurant, Order)
- 🔗 **Relationships Explained** - Visual diagrams showing how entities connect
- 📊 **Complete Picture** - All 6 tables and their relationships documented

**What's Inside:**

### 📊 Domain Model & Concepts
- Core actors: User, Restaurant, Rider, Order
- Supporting concepts: Address, Location, MenuItem, OrderItem
- Real-world scenarios and use cases
- Visual relationship diagrams

### 🔑 Entity Relationships
- Complete ER diagram with all 6 entities
- 9 relationship types documented:
  - User → Order (1:N)
  - Restaurant → MenuItem (1:N)
  - Restaurant → Order (1:N)
  - Rider → Order (1:N)
  - Order → OrderItem (1:N)
  - User → Address → Location (owned entities)
  - Restaurant → Address → Location (owned entities)
  - Rider → CurrentLocation (owned entity)
  - Order → DeliveryAddress (owned entity)

### 🗂️ Design Patterns
- **Owned Entity Pattern** - Why Address/Location are NOT separate tables
- **Aggregates** - Parent-child relationships (Restaurant → MenuItem)
- **References** - Foreign key associations (Order → User)
- **Snapshot Pattern** - Historical data preservation (OrderItem prices)

### 📈 All 6 Physical Tables
1. **Users** (with embedded Address → Location)
2. **Restaurants** (with embedded Address → Location)
3. **Riders** (with embedded CurrentLocation)
4. **Orders** (with embedded DeliveryAddress)
5. **MenuItem** (child of Restaurant, CASCADE DELETE)
6. **OrderItem** (child of Order, CASCADE DELETE)

### 🚀 Scalability Strategy
- **Phase 1:** Single server (<10K users)
- **Phase 2:** Read replicas (0-100K users)
- **Phase 3:** Multi-replica + Redis caching (100K-1M users)
- **Phase 4:** Geographic sharding (1M-10M users, 1M orders/day)

### 💾 Storage Estimates
- 10M users = 3 GB
- 100K restaurants (with menus) = 1.5 GB
- 100M orders/year = 55 GB/year

### 🔧 Optimization Techniques
- Composite indexes for common queries
- Table partitioning by month
- Connection pooling
- Query optimization (eliminate N+1)
- Caching strategy with Redis

**Best For:**
- Database administrators
- Understanding the data model BEFORE diving into code
- Architects designing for scale
- DevOps engineers planning infrastructure
- Anyone who wants to understand "what data exists and how it relates"

**Use This When:**
- Starting to work on the project (read this first!)
- Planning database migrations
- Optimizing queries
- Designing new features that need new tables
- Scaling from prototype to production

---

## 5️⃣ docs/API_DOCUMENTATION.md - API Reference

**Purpose:** Complete REST API reference for all endpoints

**Why Read This Third?**
- 🚪 **Interface Layer** - Now that you know the data and logic, learn HOW to interact with the system
- 🔌 **Practical Usage** - See how to actually call endpoints and pass data
- 📡 **Integration Ready** - Complete examples for building clients
- ✨ **Full Context** - API endpoints make sense because you understand the underlying data and algorithms

**What's Inside:**
- ✅ All 27+ API endpoints documented
- ✅ Request/response examples (JSON)
- ✅ HTTP methods, parameters, headers
- ✅ Success and error responses
- ✅ Data models and schemas
- ✅ Code examples (cURL, C#, JavaScript)
- ✅ Authentication information
- ✅ Testing guide with Swagger

**Sections:**
1. **Users API** - Registration, profile, order history
2. **Restaurants API** - Management, menus, order acceptance
3. **Riders API** - Registration, location updates, orders
4. **Orders API** - Cart calculation, placement, tracking
5. **Recommendations API** - Restaurant suggestions
6. **Data Models** - Entity schemas
7. **Error Responses** - Standard error format

**Best For:**
- Frontend developers integrating with the API
- QA engineers writing test cases
- API consumers building clients
- Understanding request/response contracts

**Use This When:**
- Calling any API endpoint
- Understanding what parameters to pass
- Debugging API issues
- Writing API integration code

---

## 6️⃣ docs/TESTING_GUIDE.md - Hands-On Testing Tutorial

**Purpose:** Step-by-step guide for testing the complete order flow using Swagger UI

**Why Read This After API Documentation?**
- 🧪 **Practice What You Learned** - Now that you know the endpoints, test them!
- 🎯 **Real Scenarios** - See how APIs work together in actual business flows
- 📝 **Copy-Paste Examples** - Ready-to-use JSON for every request
- ✅ **Verify Setup** - Confirm your local environment works correctly

**What's Inside:**
- ✅ **Complete Order Flow** - From browsing restaurants to delivery (9 phases)
- ✅ **Step-by-Step Swagger Instructions** - Click-by-click guidance
- ✅ **Real JSON Examples** - Copy-paste ready request/response samples
- ✅ **Behind-the-Scenes Explanations** - What happens at each step
- ✅ **9 Testing Phases:**
  1. Setup test data (users, restaurants, riders)
  2. Browse restaurants and get recommendations
  3. Preview order (cart simulation)
  4. Place order (automatic rider assignment)
  5. Restaurant accepts order
  6. Rider picks up order
  7. Delivery in progress (location tracking)
  8. Order delivered
  9. Verify complete flow
- ✅ **Common Scenarios** - Test edge cases (closed restaurant, no riders, etc.)
- ✅ **Troubleshooting** - 7 common problems with solutions
- ✅ **Quick Reference Table** - All endpoints at a glance
- ✅ **Tips for Beginners** - Best practices for testing

**Best For:**
- **New developers** wanting hands-on experience
- **QA testers** learning the testing workflow
- **Anyone** who prefers learning by doing
- **Demonstrations** of platform capabilities

**Covers:**
- Creating test entities (users, restaurants, riders, menu items)
- Getting restaurant recommendations based on location/cuisine
- Placing orders with multiple menu items
- Complete order lifecycle (Pending → Confirmed → Preparing → PickedUp → Delivered)
- Rider assignment algorithm in action (nearest available rider)
- Status updates from all perspectives (customer, restaurant, rider)
- Payment processing and order history

**Time to Complete:** 30-45 minutes for full walkthrough

**Prerequisites:**
- Application running (`dotnet run`)
- Browser with Swagger UI open (`https://localhost:5001/swagger`)
- Notepad to save entity IDs

**Start Here If:** 
- You want to test the APIs hands-on
- You need to verify the order flow works
- You're preparing test cases
- You're demonstrating the platform to others

---

## 7️⃣ docs/DESIGN_PRESENTATION.md - System Design Document

**Purpose:** Comprehensive system design document for technical presentations and reviews

**What's Inside:**
- ✅ **Your Approach** - How you thought through the problem
  - Domain-Driven Design methodology
  - Clean Architecture principles (3-layer)
  - 4-week development phases
  - Key design decisions with rationale
- ✅ **Core Assumptions** - What you assumed and why
  - Geographic (Haversine formula, GPS precision)
  - Time-related (delivery speed, prep times)
  - Business logic (order workflow, rider availability)
  - Data (user addresses, restaurant hours, menu availability)
- ✅ **Low-Level Design** - Detailed architecture
  - Complete architecture diagrams (all 4 layers)
  - Data flow examples (step-by-step order placement)
  - Algorithm deep dives with complexity analysis
  - Full database schema with SQL DDL
- ✅ **How You Handle Scale** - Production-ready scaling
  - 4-phase scalability roadmap (10K → 10M users)
  - Sharding strategy (geographic)
  - Caching strategy (Redis, 80% hit rate)
  - Query optimization techniques
  - Connection pooling

**Best For:**
- **Technical presentations** to stakeholders or interviewers
- **Post-submission reviews** and discussions
- **Architecture reviews** with senior engineers
- **Portfolio presentations** for job applications
- **Team onboarding** - complete system overview
- **Design document templates** for similar projects

**Covers:**
- Executive summary with key achievements
- Complete technology stack
- Domain-Driven Design approach
- All trade-offs and decisions (SQL vs NoSQL, INT vs GUID, etc.)
- Performance analysis with concrete numbers
- Cost estimates per phase
- Future roadmap (12-month plan)

**Format:** Presentation-style document with diagrams, code examples, and metrics

**Time to Read:** 60-90 minutes

**Start Here If:**
- Preparing for a technical presentation
- Need to explain your design decisions
- Want to see the complete system at a glance
- Reviewing architecture before implementation

---

## 4️⃣ docs/TECHNICAL_DESIGN.md - Algorithms & Logic

**Purpose:** Deep dive into system design, assumptions, and algorithms

**Why Read This Second?**
- 📐 **Blueprint Layer** - Now that you know the data model, learn HOW we process that data
- 🧠 **Business Logic** - Understand the algorithms that power the platform
- ⚙️ **Implementation Details** - See how entities interact in real scenarios
- 🎯 **Makes Sense Now** - References Users, Restaurants, Orders from the database schema

**What's Inside:**

### 🎯 Core Assumptions
- **Geographic Assumptions**
  - Earth radius: 6,371 km
  - Delivery speed: 20 km/h
  - Distance calculation: Haversine formula
  - Coordinate precision: 6 decimal places

- **Time Assumptions**
  - Delivery buffer: 5 minutes
  - Restaurant prep time: 15-45 minutes
  - Max delivery radius: 10 km (configurable)

- **Business Logic**
  - Order lifecycle states
  - Rider status model
  - Concurrency handling

### 📐 Algorithms Explained

#### Rider Assignment Algorithm
```
Step-by-step explanation of:
1. How we select the nearest available rider
2. Distance calculation (Haversine)
3. Sorting and selection logic
4. State updates (rider → Busy, order → Preparing)
5. ETA calculation for pickup
```

**Time Complexity:** O(n log n)  
**Accuracy:** Within 2-3 km of actual road distance

#### Restaurant Recommendation Engine
```
Multi-stage filtering pipeline:
Stage 1: Filter open restaurants
Stage 2: Distance radius check (eliminates 60-80%)
Stage 3: Cuisine type matching
Stage 4: Time constraint filter
Stage 5: Ranking & sorting (time 40%, rating 35%, distance 25%)
```

**Performance:** 88% operation reduction through early filtering  
**Result:** Top N restaurants sorted by relevance

#### Distance Calculation
```
Haversine formula implementation:
- Input: Two GPS coordinates
- Output: Straight-line distance in kilometers
- Accuracy: < 0.5% error for typical distances
- Speed: Microseconds per calculation
```

**When to Use:** Delivery radius checks, rider selection, filtering  
**Limitations:** Doesn't account for actual road routes or traffic

#### Delivery Time Estimation
```
Formula: Time = (Distance / 20 km/h) × 60 + 5 min buffer

Examples:
- 1 km → 8 minutes
- 5 km → 20 minutes
- 10 km → 35 minutes
```

**Buffer Includes:** Parking, pickup, finding address, handoff

### ⚙️ Technical Details
- Complete order placement flow (8 stages)
- Validation rules for all entities
- Performance optimizations
- Edge cases & error handling
- Race condition scenarios
- Spatial indexing strategies

### 🚀 Future Enhancements
- Predictive routing
- Dynamic pricing
- Machine learning recommendations
- Real-time tracking
- Batch delivery
- Zone-based assignment

**Best For:**
- Understanding WHY things work the way they do
- Architects designing similar systems
- Developers modifying core algorithms
- Code reviewers evaluating logic
- Technical interviews or presentations

**Use This When:**
- Modifying rider assignment logic
- Changing distance calculations
- Optimizing performance
- Understanding trade-offs
- Planning future features

---

## 4️⃣ docs/DATABASE_SCHEMA.md - Database Architecture & Scalability

**Purpose:** Complete database design documentation with scaling strategies for production

**What's Inside:**

### 📊 Schema Overview
- **All Tables Documented:**
  - Users (customer information)
  - Restaurants (with embedded menu items)
  - Riders (delivery drivers with real-time location)
  - Orders (central transaction table)
  - OrderItems (order line items)
  - MenuItem (restaurant menu catalog)

- **Entity-Relationship Diagrams:**
  - Visual representation of table relationships
  - Cardinality (1:N relationships)
  - Foreign key constraints
  - Cascade delete rules

### 🔑 Indexes & Performance
- Primary key strategy (INT IDENTITY vs GUID)
- Foreign key indexes
- Composite indexes for common queries
- Geospatial indexes for location-based queries
- Index maintenance schedule
- Fragmentation monitoring

### 📈 Scalability Roadmap

**Phase 1: Prototype (Current)**
- Single SQL Server
- <10,000 users, <1,000 restaurants
- 5,000 orders/day

**Phase 2: Growth (0-100K users)**
- Read replica pattern
- 1 Primary + 1 Replica
- 2x read capacity

**Phase 3: Scale (100K-1M users)**
- Multi-replica + Redis caching
- 1 Primary + 3 Replicas + Cache
- 80%+ cache hit rate
- 10x performance improvement

**Phase 4: Massive Scale (1M-10M users)**
- Geographic sharding (30 shards)
- 10M users, 100K restaurants
- 1M orders/day (12 orders/sec avg, 50 peak)
- 100,000 queries/second

### 🌍 Sharding Strategies
1. **Geographic Sharding** - By city/region (recommended)
2. **User ID Range Sharding** - Even distribution
3. **Hybrid Approach** - City hash + User ID

### 💾 Storage Estimates
- **User**: ~300 bytes → 10M users = 3 GB
- **Restaurant**: ~8 KB (with menu) → 100K = 800 MB
- **Order**: ~550 bytes → 100M/year = 55 GB/year
- **Total (5 years)**: ~300 GB

### 🔧 Optimization Techniques
- Query optimization (eliminate N+1)
- Projection (select only needed columns)
- Batch operations
- Connection pooling
- Asynchronous I/O
- Table partitioning by month

### 📦 Caching Strategy
- **What to cache:** Restaurant menus, user profiles, recommendations
- **TTL policies:** 5-30 minutes
- **Invalidation:** Event-based + time-based
- **Cache keys:** `{entity}:{id}:{context}`
- **Target hit rate:** 80%+

### 📊 Monitoring & Maintenance
- Key metrics (query duration, CPU, I/O wait)
- Slow query detection
- Index fragmentation checks
- Weekly maintenance windows
- Capacity planning reviews

**Best For:**
- Database administrators
- DevOps engineers planning infrastructure
- Architects designing for scale
- Performance tuning and optimization
- Cost estimation for cloud deployment

**Use This When:**
- Planning database migrations
- Scaling from prototype to production
- Optimizing slow queries
- Implementing caching
- Designing sharding strategy
- Estimating infrastructure costs

---

## Quick Navigation Guide

### 🎯 Learning Path (Recommended Order)

**For New Developers:**
1. Read `README.md` → Get the project running
2. Read `docs/DATABASE_SCHEMA.md` → Understand the data model
3. Read `docs/TECHNICAL_DESIGN.md` → Learn the business logic
4. Read `docs/API_DOCUMENTATION.md` → Study all endpoints
5. Read `docs/TESTING_GUIDE.md` → Test the complete order flow hands-on
6. Read `docs/DESIGN_PRESENTATION.md` → See the big picture

**Why This Order?**
- **Foundation → Blueprint → Interface**
- Understand WHAT data exists before learning HOW it's processed
- Understand HOW it's processed before learning HOW to access it via API

---

### I want to...

**...set up the project locally**
→ Start with `README.md` → Getting Started section

**...understand what entities exist (User, Restaurant, Order)**
→ Go to `docs/DATABASE_SCHEMA.md` → Domain Model & Concepts

**...see how entities relate to each other**
→ Go to `docs/DATABASE_SCHEMA.md` → Entity Relationships

**...test the complete order flow step-by-step**
→ Go to `docs/TESTING_GUIDE.md` → Complete Order Flow Testing

**...learn how to use Swagger to test APIs**
→ Go to `docs/TESTING_GUIDE.md` → Prerequisites & Getting Started

**...understand why Address is not a separate table**
→ Go to `docs/DATABASE_SCHEMA.md` → Owned Entity Pattern

**...see all database tables and their structure**
→ Go to `docs/DATABASE_SCHEMA.md` → Complete Table Structures

**...understand how rider assignment works**
→ Go to `docs/TECHNICAL_DESIGN.md` → Rider Assignment Algorithm

**...know what the recommendation algorithm does**
→ Go to `docs/TECHNICAL_DESIGN.md` → Restaurant Recommendation Engine

**...call an API endpoint**
→ Go to `docs/API_DOCUMENTATION.md` → Find your endpoint

**...test edge cases (closed restaurant, no riders)**
→ Go to `docs/TESTING_GUIDE.md` → Common Scenarios

**...plan for scaling to millions of users**
→ Go to `docs/DATABASE_SCHEMA.md` → Scalability Strategy section

**...prepare a technical presentation**
→ Go to `docs/DESIGN_PRESENTATION.md` → System Design Document

**...optimize database performance**
→ Go to `docs/DATABASE_SCHEMA.md` → Indexes & Optimization section

**...implement caching**
→ Go to `docs/DATABASE_SCHEMA.md` → Caching Strategy section

**...see example API requests**
→ Go to `docs/API_DOCUMENTATION.md` → Examples section

**...understand distance calculations**
→ Go to `docs/TECHNICAL_DESIGN.md` → Distance Calculation

**...know what assumptions we're making**
→ Go to `docs/TECHNICAL_DESIGN.md` → Core Assumptions

**...test the API interactively**
→ Go to `README.md` → Find Swagger UI link → https://localhost:7233/swagger

**...understand order lifecycle**
→ Go to `docs/TECHNICAL_DESIGN.md` → Order Placement Flow

**...see all data models**
→ Go to `docs/API_DOCUMENTATION.md` → Data Models section

---

## Documentation Maintenance

### When to Update

| Document | Update When... |
|----------|---------------|
| **README.md** | • Adding new features<br>• Changing setup process<br>• Updating dependencies<br>• Modifying project structure |
| **API_DOCUMENTATION.md** | • Adding/modifying endpoints<br>• Changing request/response formats<br>• Adding new data models<br>• Updating error codes |
| **TECHNICAL_DESIGN.md** | • Modifying algorithms<br>• Changing assumptions<br>• Performance optimizations<br>• New business logic |

### Documentation Standards

✅ **Keep Examples Current** - Test code examples before committing  
✅ **Update Diagrams** - Visual aids should match implementation  
✅ **Version Numbers** - Update version in headers when making changes  
✅ **Date Stamps** - Include "Last Updated" dates  
✅ **Cross-References** - Link between related sections  

---

## Additional Resources

### In the Codebase

- **XML Comments**: All controllers and services have inline documentation
- **Unit Tests**: Serve as executable documentation (future)
- **Swagger/OpenAPI**: Auto-generated from code annotations

### External Links

- **GitHub Repository**: https://github.com/Rachit77s/FoodDeliveryProject
- **Issues & Questions**: https://github.com/Rachit77s/FoodDeliveryProject/issues
- **Discussions**: https://github.com/Rachit77s/FoodDeliveryProject/discussions

---

**Happy Reading! 📖**
