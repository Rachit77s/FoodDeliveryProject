# Testing Guide - Food Delivery Platform

**Version:** 1.0  
**Purpose:** Step-by-step guide for testing the complete order flow

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Prerequisites](#prerequisites)
3. [Complete Order Flow Testing](#complete-order-flow-testing)
4. [API Testing Reference](#api-testing-reference)
5. [Common Scenarios](#common-scenarios)
6. [Troubleshooting](#troubleshooting)

---

## Getting Started

### What You'll Learn

This guide will walk you through testing the **complete order flow** from start to finish:

```
1. Setup → Create test users, restaurants, riders
2. Browse → Search restaurants and view menus
3. Cart → Preview order before placing
4. Place Order → Submit order and get assigned rider
5. Track → Follow order through its lifecycle
6. Complete → Deliver order and verify status
```

### Tools Needed

- **Swagger UI** (built-in): `https://localhost:5001/swagger`
- **Postman** (optional): For saving test collections
- **Browser**: To view Swagger documentation
- **Notepad**: To save IDs from responses

---

## Prerequisites

### Step 1: Start the Application

```powershell
# Navigate to project directory
cd C:\Users\rachit\source\repos\FoodDeliveryPolaris

# Run the application
dotnet run

# Expected output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:5001
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5000
```

### Step 2: Open Swagger UI

1. Open browser
2. Navigate to: `https://localhost:5001/swagger`
3. You should see the Swagger UI with all API endpoints

### Step 3: Check Database Seeding

The application automatically seeds test data on startup. Verify by checking:

**GET /api/users**
- Click on the endpoint
- Click "Try it out"
- Click "Execute"
- You should see pre-seeded users

---

## Complete Order Flow Testing

### Phase 1: Setup Test Data

Before testing the order flow, let's ensure we have the necessary test data or create new entities.

#### 1.1 Create a Test User

**Endpoint:** `POST /api/users`

**Swagger Steps:**
1. Expand `POST /api/users`
2. Click "Try it out"
3. Replace the request body with:

```json
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
```

4. Click "Execute"
5. **Expected Response:** `201 Created`

```json
{
  "id": 1,
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
```

6. **⚠️ IMPORTANT:** Save the `id` value (e.g., `1`) - you'll need it for ordering!

---

#### 1.2 Create a Test Restaurant

**Endpoint:** `POST /api/restaurants`

**Swagger Steps:**
1. Expand `POST /api/restaurants`
2. Click "Try it out"
3. Use this request body:

```json
{
  "name": "Pizza Paradise",
  "phone": "+912212345678",
  "address": {
    "street": "456 Food Street",
    "city": "Mumbai",
    "zipCode": "400002",
    "location": {
      "latitude": 19.0896,
      "longitude": 72.8656
    }
  },
  "isOpen": true,
  "deliveryRadiusKm": 10.0,
  "averagePreparationTimeMinutes": 20,
  "rating": 4.5
}
```

4. Click "Execute"
5. **Expected Response:** `201 Created`

```json
{
  "id": 5,
  "name": "Pizza Paradise",
  "phone": "+912212345678",
  "address": {
    "street": "456 Food Street",
    "city": "Mumbai",
    "zipCode": "400002",
    "location": {
      "latitude": 19.0896,
      "longitude": 72.8656
    }
  },
  "isOpen": true,
  "deliveryRadiusKm": 10.0,
  "averagePreparationTimeMinutes": 20,
  "rating": 4.5,
  "menu": []
}
```

6. **⚠️ IMPORTANT:** Save the restaurant `id` (e.g., `5`)

---

#### 1.3 Add Menu Items

**Endpoint:** `POST /api/restaurants/{restaurantId}/menu`

**Swagger Steps:**
1. Expand `POST /api/restaurants/{restaurantId}/menu`
2. Click "Try it out"
3. Enter the `restaurantId` from step 1.2 (e.g., `5`)
4. Add a Margherita Pizza:

```json
{
  "name": "Margherita Pizza",
  "price": 299.00,
  "available": true,
  "cuisineType": "Italian",
  "preparationTimeMinutes": 20
}
```

5. Click "Execute"
6. **Expected Response:** `201 Created`

```json
{
  "id": 101,
  "restaurantId": 5,
  "name": "Margherita Pizza",
  "price": 299.00,
  "available": true,
  "cuisineType": "Italian",
  "preparationTimeMinutes": 20
}
```

7. **⚠️ IMPORTANT:** Save the menu item `id` (e.g., `101`)

8. **Repeat for more items:**

**Pepperoni Pizza:**
```json
{
  "name": "Pepperoni Pizza",
  "price": 349.00,
  "available": true,
  "cuisineType": "Italian",
  "preparationTimeMinutes": 25
}
```

**Garlic Bread:**
```json
{
  "name": "Garlic Bread",
  "price": 99.00,
  "available": true,
  "cuisineType": "Italian",
  "preparationTimeMinutes": 10
}
```

9. **Save all menu item IDs** (e.g., `101`, `102`, `103`)

---

#### 1.4 Create a Test Rider

**Endpoint:** `POST /api/riders`

**Swagger Steps:**
1. Expand `POST /api/riders`
2. Click "Try it out"
3. Use this request body:

```json
{
  "name": "Fast Delivery Rider",
  "email": "rider@test.com",
  "phone": "+919123456789",
  "vehicleNumber": "MH-01-AB-1234",
  "currentLocation": {
    "latitude": 19.0850,
    "longitude": 72.8700
  },
  "riderStatus": "Available"
}
```

4. Click "Execute"
5. **Expected Response:** `201 Created`

```json
{
  "id": 10,
  "name": "Fast Delivery Rider",
  "email": "rider@test.com",
  "phone": "+919123456789",
  "vehicleNumber": "MH-01-AB-1234",
  "currentLocation": {
    "latitude": 19.0850,
    "longitude": 72.8700
  },
  "riderStatus": "Available"
}
```

6. **⚠️ IMPORTANT:** Save the rider `id` (e.g., `10`)

---

### Phase 2: Browse Restaurants

Now that we have test data, let's browse restaurants as a customer would.

#### 2.1 Get Food Recommendations

**Endpoint:** `POST /api/recommendations`

**Swagger Steps:**
1. Expand `POST /api/recommendations`
2. Click "Try it out"
3. Use this request body (replace `userId` with your user ID):

```json
{
  "userId": 1,
  "cuisineType": "Italian",
  "maxDeliveryTimeMinutes": 45
}
```

4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
{
  "userId": 1,
  "recommendations": [
    {
      "restaurantId": 5,
      "name": "Pizza Paradise",
      "rating": 4.5,
      "distanceKm": 1.7,
      "estimatedDeliveryMinutes": 35,
      "cuisineType": "Italian",
      "isOpen": true
    }
  ]
}
```

**What's Happening Behind the Scenes:**
- ✅ System calculates distance from user to all restaurants (Haversine formula)
- ✅ Filters restaurants within delivery radius
- ✅ Filters by cuisine type (Italian)
- ✅ Filters by delivery time (< 45 minutes)
- ✅ Ranks by score (time + rating + distance)

---

#### 2.2 View Restaurant Details

**Endpoint:** `GET /api/restaurants/{id}`

**Swagger Steps:**
1. Expand `GET /api/restaurants/{id}`
2. Click "Try it out"
3. Enter the restaurant ID (e.g., `5`)
4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
{
  "id": 5,
  "name": "Pizza Paradise",
  "phone": "+912212345678",
  "address": {
    "street": "456 Food Street",
    "city": "Mumbai",
    "zipCode": "400002",
    "location": {
      "latitude": 19.0896,
      "longitude": 72.8656
    }
  },
  "isOpen": true,
  "deliveryRadiusKm": 10.0,
  "averagePreparationTimeMinutes": 20,
  "rating": 4.5,
  "menu": [
    {
      "id": 101,
      "name": "Margherita Pizza",
      "price": 299.00,
      "available": true,
      "cuisineType": "Italian",
      "preparationTimeMinutes": 20
    },
    {
      "id": 102,
      "name": "Pepperoni Pizza",
      "price": 349.00,
      "available": true,
      "cuisineType": "Italian",
      "preparationTimeMinutes": 25
    },
    {
      "id": 103,
      "name": "Garlic Bread",
      "price": 99.00,
      "available": true,
      "cuisineType": "Italian",
      "preparationTimeMinutes": 10
    }
  ]
}
```

**Customer Journey:** User sees the full menu with prices and can build their cart.

---

### Phase 3: Preview Order (Cart)

Before placing the order, the customer wants to preview the total cost and delivery details.

#### 3.1 Calculate Order Preview

**Note:** The current API doesn't have a dedicated "preview" endpoint, but you can calculate manually:

**Manual Calculation:**
```
Cart:
  1x Margherita Pizza (₹299)
  1x Pepperoni Pizza (₹349)
  2x Garlic Bread (₹99 × 2 = ₹198)

Subtotal:     ₹299 + ₹349 + ₹198 = ₹846
Tax (0%):     ₹0
Delivery Fee: ₹0 (included)
─────────────────────────────────────
TOTAL:        ₹846
```

**🔮 Future Enhancement:** Add `POST /api/orders/preview` endpoint that:
- Validates all items are available
- Calculates total
- Estimates delivery time
- Shows delivery address
- Returns preview without creating order

---

### Phase 4: Place Order

Now the customer is ready to place the order!

#### 4.1 Create Order

**Endpoint:** `POST /api/orders`

**Swagger Steps:**
1. Expand `POST /api/orders`
2. Click "Try it out"
3. Use this request body (replace IDs with your saved values):

```json
{
  "userId": 1,
  "restaurantId": 5,
  "items": [
    {
      "menuItemId": 101,
      "quantity": 1
    },
    {
      "menuItemId": 102,
      "quantity": 1
    },
    {
      "menuItemId": 103,
      "quantity": 2
    }
  ],
  "deliveryAddress": {
    "latitude": 19.0760,
    "longitude": 72.8777
  },
  "notes": "Please ring the doorbell twice"
}
```

4. Click "Execute"
5. **Expected Response:** `201 Created`

```json
{
  "id": 42,
  "userId": 1,
  "userName": "Test Customer",
  "restaurantId": 5,
  "restaurantName": "Pizza Paradise",
  "riderId": 10,
  "riderName": "Fast Delivery Rider",
  "items": [
    {
      "id": 201,
      "menuItemId": 101,
      "name": "Margherita Pizza",
      "price": 299.00,
      "quantity": 1
    },
    {
      "id": 202,
      "menuItemId": 102,
      "name": "Pepperoni Pizza",
      "price": 349.00,
      "quantity": 1
    },
    {
      "id": 203,
      "menuItemId": 103,
      "name": "Garlic Bread",
      "price": 99.00,
      "quantity": 2
    }
  ],
  "total": 846.00,
  "status": "Preparing",
  "paymentStatus": "NotPaid",
  "deliveryAddress": {
    "latitude": 19.0760,
    "longitude": 72.8777
  },
  "notes": "Please ring the doorbell twice",
  "createdAt": "2025-11-06T10:30:00Z",
  "updatedAt": "2025-11-06T10:30:00Z"
}
```

6. **⚠️ IMPORTANT:** Save the order `id` (e.g., `42`)

---

#### 4.2 What Just Happened? (Behind the Scenes)

When you clicked "Execute", the system performed these steps:

```
Step 1: Validate User
  → Check if userId=1 exists ✓
  
Step 2: Validate Restaurant
  → Check if restaurantId=5 exists ✓
  → Check if restaurant is open ✓
  
Step 3: Validate Menu Items
  → Check if menuItemId=101 exists ✓
  → Check if menuItemId=101 is available ✓
  → Check if menuItemId=102 exists ✓
  → Check if menuItemId=102 is available ✓
  → Check if menuItemId=103 exists ✓
  → Check if menuItemId=103 is available ✓
  
Step 4: Calculate Total
  → Item 1: ₹299 × 1 = ₹299
  → Item 2: ₹349 × 1 = ₹349
  → Item 3: ₹99 × 2 = ₹198
  → Total = ₹846 ✓
  
Step 5: Create Order Entity
  → Status = "Pending"
  → PaymentStatus = "NotPaid"
  → Save to database ✓
  
Step 6: Find Nearest Available Rider
  → Query all riders with Status="Available"
  → Found: Rider 10 at (19.0850, 72.8700)
  → Calculate distance to restaurant (19.0896, 72.8656)
  → Distance = 0.6 km (nearest) ✓
  
Step 7: Assign Rider
  → Order.RiderId = 10
  → Order.Status = "Preparing"
  → Rider.Status = "Busy"
  → Update database ✓
  
Step 8: Calculate ETA
  → Prep time: 20 minutes (restaurant avg)
  → Distance: 1.7 km (restaurant to customer)
  → Travel time: (1.7 / 20) × 60 = 5 minutes
  → Buffer: 5 minutes
  → Total ETA: 20 + 5 + 5 = 30 minutes ✓
```

**Result:** Order created, rider assigned, status set to "Preparing"

---

### Phase 5: Restaurant Accepts Order

The restaurant receives the order notification and starts preparing.

#### 5.1 View Order Details (Restaurant Perspective)

**Endpoint:** `GET /api/orders/{id}`

**Swagger Steps:**
1. Expand `GET /api/orders/{id}`
2. Click "Try it out"
3. Enter the order ID (e.g., `42`)
4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
{
  "id": 42,
  "userId": 1,
  "userName": "Test Customer",
  "restaurantId": 5,
  "restaurantName": "Pizza Paradise",
  "riderId": 10,
  "riderName": "Fast Delivery Rider",
  "items": [
    {
      "id": 201,
      "menuItemId": 101,
      "name": "Margherita Pizza",
      "price": 299.00,
      "quantity": 1
    },
    {
      "id": 202,
      "menuItemId": 102,
      "name": "Pepperoni Pizza",
      "price": 349.00,
      "quantity": 1
    },
    {
      "id": 203,
      "menuItemId": 103,
      "name": "Garlic Bread",
      "price": 99.00,
      "quantity": 2
    }
  ],
  "total": 846.00,
  "status": "Preparing",
  "paymentStatus": "NotPaid",
  "deliveryAddress": {
    "latitude": 19.0760,
    "longitude": 72.8777
  },
  "notes": "Please ring the doorbell twice",
  "createdAt": "2025-11-06T10:30:00Z",
  "updatedAt": "2025-11-06T10:30:00Z"
}
```

**Restaurant Actions:**
- ✅ Review order items
- ✅ Check special notes
- ✅ Start preparing food
- ⏳ Update status when ready for pickup

---

#### 5.2 Update Order Status to Confirmed

**Endpoint:** `PUT /api/orders/{id}/status`

**Swagger Steps:**
1. Expand `PUT /api/orders/{id}/status`
2. Click "Try it out"
3. Enter the order ID (e.g., `42`)
4. Enter the new status:

```json
{
  "status": "Confirmed"
}
```

5. Click "Execute"
6. **Expected Response:** `200 OK`

```json
{
  "id": 42,
  "status": "Confirmed",
  "updatedAt": "2025-11-06T10:32:00Z"
}
```

**Status Flow:**
```
Pending → Confirmed → Preparing → PickedUp → Delivered
  (initial)  (restaurant accepts)  (cooking)  (rider has it)  (complete)
```

---

### Phase 6: Rider Picks Up Order

The rider receives notification and heads to the restaurant.

#### 6.1 Update Rider Location (Heading to Restaurant)

**Endpoint:** `PUT /api/riders/{id}/location`

**Swagger Steps:**
1. Expand `PUT /api/riders/{id}/location`
2. Click "Try it out"
3. Enter the rider ID (e.g., `10`)
4. Update location (closer to restaurant):

```json
{
  "latitude": 19.0890,
  "longitude": 72.8670
}
```

5. Click "Execute"
6. **Expected Response:** `200 OK`

```json
{
  "id": 10,
  "name": "Fast Delivery Rider",
  "currentLocation": {
    "latitude": 19.0890,
    "longitude": 72.8670
  },
  "locationUpdatedAt": "2025-11-06T10:45:00Z"
}
```

---

#### 6.2 Update Order Status to PickedUp

**Endpoint:** `PUT /api/orders/{id}/status`

**Swagger Steps:**
1. Expand `PUT /api/orders/{id}/status`
2. Click "Try it out"
3. Enter the order ID (e.g., `42`)
4. Update status:

```json
{
  "status": "PickedUp"
}
```

5. Click "Execute"
6. **Expected Response:** `200 OK`

```json
{
  "id": 42,
  "status": "PickedUp",
  "updatedAt": "2025-11-06T10:50:00Z"
}
```

**What This Means:**
- ✅ Rider has collected the food from restaurant
- ✅ Rider is now en route to customer
- ✅ Customer can track rider location in real-time (future feature)

---

### Phase 7: Delivery in Progress

The rider is traveling to the customer's location.

#### 7.1 Update Rider Location (Multiple Times)

Simulate rider movement by updating location multiple times:

**Location Update 1:** (Halfway to customer)
```json
{
  "latitude": 19.0820,
  "longitude": 72.8720
}
```

**Location Update 2:** (Almost at customer)
```json
{
  "latitude": 19.0770,
  "longitude": 72.8765
}
```

**Location Update 3:** (At customer location)
```json
{
  "latitude": 19.0760,
  "longitude": 72.8777
}
```

Use `PUT /api/riders/{id}/location` for each update.

---

#### 7.2 Check Order Status (Customer Tracking)

**Endpoint:** `GET /api/orders/{id}`

Customers can check order status anytime:

```json
{
  "id": 42,
  "status": "PickedUp",
  "riderId": 10,
  "riderName": "Fast Delivery Rider",
  "currentRiderLocation": {
    "latitude": 19.0770,
    "longitude": 72.8765
  }
}
```

**Customer sees:**
- ✅ Rider name and ID
- ✅ Current status (PickedUp)
- ✅ Estimated arrival time
- 🔮 Future: Live map with rider location

---

### Phase 8: Order Delivered

The rider arrives and completes the delivery.

#### 8.1 Mark Order as Delivered

**Endpoint:** `PUT /api/orders/{id}/status`

**Swagger Steps:**
1. Expand `PUT /api/orders/{id}/status`
2. Click "Try it out"
3. Enter the order ID (e.g., `42`)
4. Update status:

```json
{
  "status": "Delivered"
}
```

5. Click "Execute"
6. **Expected Response:** `200 OK`

```json
{
  "id": 42,
  "status": "Delivered",
  "completedAt": "2025-11-06T11:15:00Z",
  "updatedAt": "2025-11-06T11:15:00Z"
}
```

---

#### 8.2 Update Rider Status to Available

**Endpoint:** `PUT /api/riders/{id}/status`

**Swagger Steps:**
1. Expand `PUT /api/riders/{id}/status`
2. Click "Try it out"
3. Enter the rider ID (e.g., `10`)
4. Update status:

```json
{
  "status": "Available"
}
```

5. Click "Execute"
6. **Expected Response:** `200 OK`

```json
{
  "id": 10,
  "name": "Fast Delivery Rider",
  "riderStatus": "Available"
}
```

**What This Means:**
- ✅ Rider is now free for new orders
- ✅ Rider appears in available riders pool
- ✅ Rider can be assigned to next order

---

#### 8.3 Update Payment Status

**Endpoint:** `PUT /api/orders/{id}/payment-status`

**Swagger Steps:**
1. Expand `PUT /api/orders/{id}/payment-status`
2. Click "Try it out"
3. Enter the order ID (e.g., `42`)
4. Update payment status:

```json
{
  "paymentStatus": "Paid"
}
```

5. Click "Execute"
6. **Expected Response:** `200 OK`

```json
{
  "id": 42,
  "paymentStatus": "Paid",
  "updatedAt": "2025-11-06T11:16:00Z"
}
```

---

### Phase 9: Verify Complete Order

Let's verify the entire order lifecycle was successful.

#### 9.1 Get Final Order Details

**Endpoint:** `GET /api/orders/{id}`

**Swagger Steps:**
1. Expand `GET /api/orders/{id}`
2. Click "Try it out"
3. Enter the order ID (e.g., `42`)
4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
{
  "id": 42,
  "userId": 1,
  "userName": "Test Customer",
  "restaurantId": 5,
  "restaurantName": "Pizza Paradise",
  "riderId": 10,
  "riderName": "Fast Delivery Rider",
  "items": [
    {
      "id": 201,
      "menuItemId": 101,
      "name": "Margherita Pizza",
      "price": 299.00,
      "quantity": 1
    },
    {
      "id": 202,
      "menuItemId": 102,
      "name": "Pepperoni Pizza",
      "price": 349.00,
      "quantity": 1
    },
    {
      "id": 203,
      "menuItemId": 103,
      "name": "Garlic Bread",
      "price": 99.00,
      "quantity": 2
    }
  ],
  "total": 846.00,
  "status": "Delivered",
  "paymentStatus": "Paid",
  "deliveryAddress": {
    "latitude": 19.0760,
    "longitude": 72.8777
  },
  "notes": "Please ring the doorbell twice",
  "createdAt": "2025-11-06T10:30:00Z",
  "updatedAt": "2025-11-06T11:16:00Z",
  "completedAt": "2025-11-06T11:15:00Z"
}
```

**Verification Checklist:**
- ✅ Status = "Delivered"
- ✅ PaymentStatus = "Paid"
- ✅ CompletedAt timestamp set
- ✅ All items present
- ✅ Total calculated correctly
- ✅ Delivery address recorded

---

#### 9.2 View User's Order History

**Endpoint:** `GET /api/users/{id}/orders`

**Swagger Steps:**
1. Expand `GET /api/users/{id}/orders`
2. Click "Try it out"
3. Enter the user ID (e.g., `1`)
4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
[
  {
    "id": 42,
    "restaurantName": "Pizza Paradise",
    "total": 846.00,
    "status": "Delivered",
    "paymentStatus": "Paid",
    "createdAt": "2025-11-06T10:30:00Z",
    "completedAt": "2025-11-06T11:15:00Z"
  }
]
```

**Customer sees:** Complete order history with all past orders

---

#### 9.3 View Restaurant's Order History

**Endpoint:** `GET /api/restaurants/{id}/orders`

**Swagger Steps:**
1. Expand `GET /api/restaurants/{id}/orders`
2. Click "Try it out"
3. Enter the restaurant ID (e.g., `5`)
4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
[
  {
    "id": 42,
    "userName": "Test Customer",
    "total": 846.00,
    "status": "Delivered",
    "createdAt": "2025-11-06T10:30:00Z",
    "completedAt": "2025-11-06T11:15:00Z"
  }
]
```

**Restaurant sees:** All orders received from customers

---

#### 9.4 View Rider's Delivery History

**Endpoint:** `GET /api/riders/{id}/orders`

**Swagger Steps:**
1. Expand `GET /api/riders/{id}/orders`
2. Click "Try it out"
3. Enter the rider ID (e.g., `10`)
4. Click "Execute"
5. **Expected Response:** `200 OK`

```json
[
  {
    "id": 42,
    "restaurantName": "Pizza Paradise",
    "userName": "Test Customer",
    "total": 846.00,
    "status": "Delivered",
    "createdAt": "2025-11-06T10:30:00Z",
    "completedAt": "2025-11-06T11:15:00Z"
  }
]
```

**Rider sees:** All deliveries completed

---

## API Testing Reference

### Quick Reference Table

| Action | Method | Endpoint | Key Parameters |
|--------|--------|----------|----------------|
| Create User | POST | `/api/users` | name, email, phone, address |
| Create Restaurant | POST | `/api/restaurants` | name, location, isOpen |
| Add Menu Item | POST | `/api/restaurants/{id}/menu` | name, price, cuisineType |
| Create Rider | POST | `/api/riders` | name, vehicleNumber, location |
| Get Recommendations | POST | `/api/recommendations` | userId, cuisineType, maxTime |
| Place Order | POST | `/api/orders` | userId, restaurantId, items |
| Update Order Status | PUT | `/api/orders/{id}/status` | status |
| Update Payment | PUT | `/api/orders/{id}/payment-status` | paymentStatus |
| Update Rider Location | PUT | `/api/riders/{id}/location` | latitude, longitude |
| Get Order Details | GET | `/api/orders/{id}` | - |
| View Order History | GET | `/api/users/{id}/orders` | - |

---

## Common Scenarios

### Scenario 1: Order Cancelled by Customer

**Before PickedUp:**

```json
PUT /api/orders/42/status
{
  "status": "Cancelled"
}
```

**What happens:**
- Order status → "Cancelled"
- Rider status → "Available" (if assigned)
- Refund initiated (if paid)

---

### Scenario 2: No Riders Available

**Test Setup:**
1. Set all riders to "Busy":

```json
PUT /api/riders/10/status
{
  "status": "Busy"
}
```

2. Try to place order:

```json
POST /api/orders
{
  "userId": 1,
  "restaurantId": 5,
  "items": [...]
}
```

**Expected Response:** `400 Bad Request`

```json
{
  "error": "ValidationException",
  "message": "No riders available for delivery at this time",
  "statusCode": 400
}
```

**Solution:**
- Wait for rider to become available
- Or set rider status back to "Available"

---

### Scenario 3: Restaurant Closed

**Test Setup:**
1. Close the restaurant:

```json
PUT /api/restaurants/5
{
  "isOpen": false
}
```

2. Try to place order:

```json
POST /api/orders
{
  "userId": 1,
  "restaurantId": 5,
  "items": [...]
}
```

**Expected Response:** `400 Bad Request`

```json
{
  "error": "ValidationException",
  "message": "Restaurant is currently closed",
  "statusCode": 400
}
```

---

### Scenario 4: Menu Item Not Available

**Test Setup:**
1. Mark menu item as unavailable:

```json
PUT /api/restaurants/5/menu/101
{
  "available": false
}
```

2. Try to order that item:

```json
POST /api/orders
{
  "userId": 1,
  "restaurantId": 5,
  "items": [
    {
      "menuItemId": 101,
      "quantity": 1
    }
  ]
}
```

**Expected Response:** `400 Bad Request`

```json
{
  "error": "ValidationException",
  "message": "Menu item 'Margherita Pizza' is not available",
  "statusCode": 400
}
```

---

### Scenario 5: User Outside Delivery Range

**Test Setup:**
1. Create user far from restaurant (> 10 km):

```json
POST /api/users
{
  "name": "Far Customer",
  "address": {
    "location": {
      "latitude": 18.5000,
      "longitude": 73.8000
    }
  }
}
```

2. Get recommendations:

```json
POST /api/recommendations
{
  "userId": 100,
  "cuisineType": "Italian",
  "maxDeliveryTimeMinutes": 45
}
```

**Expected Response:** `200 OK` with empty recommendations

```json
{
  "userId": 100,
  "recommendations": []
}
```

**Explanation:** User is outside all restaurant delivery radii

---

## Troubleshooting

### Problem 1: "User not found"

**Error:**
```json
{
  "error": "NotFoundException",
  "message": "User with ID 999 not found"
}
```

**Solution:**
- Check the user ID exists: `GET /api/users/999`
- Create user if needed: `POST /api/users`
- Use correct ID from creation response

---

### Problem 2: "Restaurant not found"

**Error:**
```json
{
  "error": "NotFoundException",
  "message": "Restaurant with ID 888 not found"
}
```

**Solution:**
- List all restaurants: `GET /api/restaurants`
- Verify restaurant ID
- Create restaurant if needed: `POST /api/restaurants`

---

### Problem 3: "Invalid menu item"

**Error:**
```json
{
  "error": "ValidationException",
  "message": "Menu item 555 does not belong to restaurant 5"
}
```

**Solution:**
- Get restaurant menu: `GET /api/restaurants/5`
- Verify menu item IDs
- Ensure menu item belongs to correct restaurant

---

### Problem 4: Order total mismatch

**Symptom:** Order total doesn't match expected value

**Debug Steps:**
1. Get menu item prices: `GET /api/restaurants/5`
2. Calculate manually:
   ```
   Item 1: ₹299 × 2 = ₹598
   Item 2: ₹349 × 1 = ₹349
   Total = ₹947
   ```
3. Check if prices changed
4. Verify quantities in request

---

### Problem 5: Application not starting

**Error:** Port already in use

**Solution:**
```powershell
# Find process using port 5001
netstat -ano | findstr :5001

# Kill the process (replace PID with actual process ID)
taskkill /PID 12345 /F

# Restart application
dotnet run
```

---

### Problem 6: Database not seeded

**Symptom:** `GET /api/users` returns empty array

**Solution:**
```powershell
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update

# Restart application (seeding runs on startup)
dotnet run
```

---

### Problem 7: Swagger not loading

**Solution:**
1. Check if app is running: Look for "Now listening on: https://localhost:5001"
2. Verify URL: `https://localhost:5001/swagger` (not http)
3. Accept SSL certificate warning in browser
4. Clear browser cache and reload

---

## Complete Order Flow Summary

### Status Progression

```
┌─────────────────────────────────────────────────────────────┐
│                    ORDER LIFECYCLE                          │
└─────────────────────────────────────────────────────────────┘

1. Pending
   Customer places order
   System validates data
   ↓

2. Confirmed
   Restaurant accepts order
   Starts preparing food
   ↓

3. Preparing
   Restaurant cooking
   Rider assigned
   ↓

4. PickedUp
   Rider collected food
   Heading to customer
   ↓

5. Delivered
   Order completed
   Payment processed
   ✓ Done!

Alternative Path: Cancelled
   (from Pending, Confirmed, or Preparing)
```

---

### Time Estimates

| Phase | Duration | Who |
|-------|----------|-----|
| Order Placement | 2-3 minutes | Customer |
| Order Confirmation | 1 minute | Restaurant |
| Food Preparation | 15-30 minutes | Restaurant |
| Pickup | 2-5 minutes | Rider |
| Delivery | 5-20 minutes | Rider |
| **Total** | **25-60 minutes** | - |

---

## Tips for New Developers

### 1. Save IDs as You Go

Keep a notepad with all created entity IDs:

```
User ID: 1
Restaurant ID: 5
Menu Items: 101, 102, 103
Rider ID: 10
Order ID: 42
```

### 2. Use Swagger's "Try it out" Feature

- Click "Try it out" button
- Edit the JSON directly in the browser
- Click "Execute"
- See immediate results

### 3. Check Response Status Codes

- `200 OK` - Success (GET)
- `201 Created` - Success (POST)
- `204 No Content` - Success (DELETE)
- `400 Bad Request` - Validation error
- `404 Not Found` - Entity doesn't exist
- `500 Internal Server Error` - Bug in code

### 4. Read Error Messages

Error responses tell you exactly what's wrong:

```json
{
  "error": "ValidationException",
  "message": "Restaurant is currently closed",
  "statusCode": 400
}
```

### 5. Test Edge Cases

- Order with 0 quantity
- Order from closed restaurant
- Order unavailable items
- Place order with no riders
- Invalid GPS coordinates

### 6. Use Browser Dev Tools

Press F12 → Network tab → See actual HTTP requests/responses

---

## Next Steps

### 1. Test More Scenarios

- Multiple orders from same user
- Multiple orders to same restaurant
- Rider handling multiple deliveries
- Peak hours with many orders

### 2. Explore All Endpoints

Check the API documentation for more endpoints:
- User management
- Restaurant management
- Rider management
- Order queries

### 3. Build a Frontend

Use these APIs to build:
- Customer mobile app
- Restaurant dashboard
- Rider app
- Admin panel

### 4. Extend Functionality

Add features:
- Order preview endpoint
- Real-time tracking (SignalR)
- Payment gateway integration
- Rating and review system

---

## Conclusion

You now know how to:
- ✅ Set up test data (users, restaurants, riders)
- ✅ Browse restaurants and menus
- ✅ Place orders with multiple items
- ✅ Track order status through complete lifecycle
- ✅ Handle edge cases and errors
- ✅ Verify order completion

**Happy Testing!** 🚀

For more information, see:
- [API Documentation](./API_DOCUMENTATION.md) - Complete API reference
- [Database Schema](./DATABASE_SCHEMA.md) - Data model details
- [Technical Design](./TECHNICAL_DESIGN.md) - Algorithm explanations
- [Design Presentation](./DESIGN_PRESENTATION.md) - System architecture

---

**Questions or Issues?**  
Check the [Troubleshooting](#troubleshooting) section or review the error message carefully.
