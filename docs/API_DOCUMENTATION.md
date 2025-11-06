# Food Delivery Polaris - API Documentation

**Version:** 1.0  
**Base URL:** `https://localhost:7233/api`  
**Content-Type:** `application/json`

---

## Table of Contents

1. [Authentication](#authentication)
2. [Users API](#users-api)
3. [Restaurants API](#restaurants-api)
4. [Riders API](#riders-api)
5. [Orders API](#orders-api)
6. [Recommendations API](#recommendations-api)
7. [Error Responses](#error-responses)
8. [Data Models](#data-models)

---

## Authentication

**Current Status:** No authentication required (prototype).  
**Future:** JWT Bearer token authentication will be implemented.

For now, pass `userId`, `restaurantId`, or `riderId` directly in request body or query parameters.

---

## Users API

### 1. Register New User

Creates a new user account.

**Endpoint:** `POST /api/users`

**Request Body:**
```json
{
  "name": "Rahul Sharma",
  "email": "rahul@example.com",
  "phone": "+91-9876543210",
  "address": {
    "street": "123 MG Road",
    "city": "Mumbai",
    "zipCode": "400001",
    "location": {
      "lat": 19.0760,
      "lon": 72.8777
    }
  }
}
```

**Success Response:** `201 Created`
```json
{
  "id": 1,
  "name": "Rahul Sharma",
  "email": "rahul@example.com",
  "phone": "+91-9876543210",
  "address": {
    "street": "123 MG Road",
    "city": "Mumbai",
    "zipCode": "400001",
    "location": {
      "lat": 19.0760,
      "lon": 72.8777
    }
  },
  "createdAt": "2025-11-06T10:30:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Validation failed or duplicate email
- `500 Internal Server Error` - Unexpected error

---

### 2. Get All Users

Retrieves a list of all registered users.

**Endpoint:** `GET /api/users`

**Success Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Rahul Sharma",
    "email": "rahul@example.com",
    "phone": "+91-9876543210",
    "address": { ... },
    "createdAt": "2025-11-06T10:30:00Z"
  }
]
```

---

### 3. Get User by ID

Retrieves a specific user's profile.

**Endpoint:** `GET /api/users/{id}`

**Parameters:**
- `id` (path, required) - User ID

**Success Response:** `200 OK`
```json
{
  "id": 1,
  "name": "Rahul Sharma",
  "email": "rahul@example.com",
  "phone": "+91-9876543210",
  "address": { ... },
  "createdAt": "2025-11-06T10:30:00Z"
}
```

**Error Responses:**
- `404 Not Found` - User not found

---

### 4. Update User

Updates an existing user's information.

**Endpoint:** `PUT /api/users/{id}`

**Parameters:**
- `id` (path, required) - User ID

**Request Body:**
```json
{
  "name": "Rahul Kumar Sharma",
  "email": "rahul.kumar@example.com",
  "phone": "+91-9876543210",
  "address": {
    "street": "456 New Address",
    "city": "Mumbai",
    "zipCode": "400002",
    "location": {
      "lat": 19.0800,
      "lon": 72.8800
    }
  }
}
```

**Success Response:** `200 OK`
```json
{
  "id": 1,
  "name": "Rahul Kumar Sharma",
  "email": "rahul.kumar@example.com",
  ...
}
```

**Error Responses:**
- `400 Bad Request` - Validation failed
- `404 Not Found` - User not found

---

### 5. Delete User

Deletes a user account.

**Endpoint:** `DELETE /api/users/{id}`

**Parameters:**
- `id` (path, required) - User ID

**Success Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - User not found

---

### 6. Get User Order History

Retrieves all orders placed by a specific user.

**Endpoint:** `GET /api/users/{id}/orders`

**Parameters:**
- `id` (path, required) - User ID

**Success Response:** `200 OK`
```json
[
  {
    "orderId": 101,
    "restaurantName": "Spice Garden",
    "status": "Delivered",
    "totalAmount": 450.00,
    "orderedAt": "2025-11-06T12:00:00Z",
    "deliveredAt": "2025-11-06T12:45:00Z"
  }
]
```

---

## Restaurants API

### 1. Register New Restaurant

Creates a new restaurant profile.

**Endpoint:** `POST /api/restaurants`

**Request Body:**
```json
{
  "name": "Spice Garden",
  "cuisineType": "NorthIndian",
  "address": {
    "street": "45 Food Street",
    "city": "Mumbai",
    "zipCode": "400015",
    "location": {
      "lat": 19.0850,
      "lon": 72.8800
    }
  },
  "isOpen": true,
  "averagePreparationTimeMinutes": 25,
  "menuItems": [
    {
      "name": "Butter Chicken",
      "description": "Creamy tomato-based curry",
      "price": 299.00,
      "category": "Main Course",
      "isVegetarian": false,
      "isAvailable": true
    }
  ]
}
```

**Success Response:** `201 Created`
```json
{
  "id": 1,
  "name": "Spice Garden",
  "cuisineType": "NorthIndian",
  "rating": 0.0,
  "isOpen": true,
  "averagePreparationTimeMinutes": 25,
  "address": { ... },
  "menuItems": [ ... ]
}
```

**Error Responses:**
- `400 Bad Request` - Validation failed or duplicate restaurant

---

### 2. Get All Restaurants

Retrieves a list of all restaurants.

**Endpoint:** `GET /api/restaurants`

**Success Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Spice Garden",
    "cuisineType": "NorthIndian",
    "rating": 4.5,
    "isOpen": true,
    "address": { ... },
    "menuItems": [ ... ]
  }
]
```

---

### 3. Get Restaurant by ID

Retrieves a specific restaurant's details including menu.

**Endpoint:** `GET /api/restaurants/{id}`

**Parameters:**
- `id` (path, required) - Restaurant ID

**Success Response:** `200 OK`
```json
{
  "id": 1,
  "name": "Spice Garden",
  "cuisineType": "NorthIndian",
  "rating": 4.5,
  "isOpen": true,
  "averagePreparationTimeMinutes": 25,
  "address": {
    "street": "45 Food Street",
    "city": "Mumbai",
    "zipCode": "400015",
    "location": {
      "lat": 19.0850,
      "lon": 72.8800
    }
  },
  "menuItems": [
    {
      "id": 1,
      "restaurantId": 1,
      "name": "Butter Chicken",
      "description": "Creamy tomato-based curry",
      "price": 299.00,
      "category": "Main Course",
      "isVegetarian": false,
      "isAvailable": true
    }
  ]
}
```

**Error Responses:**
- `404 Not Found` - Restaurant not found

---

### 4. Update Restaurant

Updates restaurant information.

**Endpoint:** `PUT /api/restaurants/{id}`

**Parameters:**
- `id` (path, required) - Restaurant ID

**Request Body:**
```json
{
  "name": "Spice Garden Deluxe",
  "cuisineType": "NorthIndian",
  "isOpen": true,
  "averagePreparationTimeMinutes": 30,
  "address": { ... },
  "menuItems": [ ... ]
}
```

**Success Response:** `200 OK`

**Error Responses:**
- `400 Bad Request` - Validation failed
- `404 Not Found` - Restaurant not found

---

### 5. Delete Restaurant

Deletes a restaurant.

**Endpoint:** `DELETE /api/restaurants/{id}`

**Parameters:**
- `id` (path, required) - Restaurant ID

**Success Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - Restaurant not found

---

### 6. Accept Order

Restaurant accepts a pending order, triggering rider assignment.

**Endpoint:** `POST /api/restaurants/{id}/orders/{orderId}/accept`

**Parameters:**
- `id` (path, required) - Restaurant ID
- `orderId` (path, required) - Order ID

**Success Response:** `200 OK`
```json
{
  "orderId": 101,
  "status": "Preparing",
  "assignedRider": {
    "id": 5,
    "name": "Vijay Kumar",
    "phone": "+91-9988776655",
    "vehicleNumber": "MH02AB1234"
  },
  "message": "Order accepted and rider assigned successfully"
}
```

**Error Responses:**
- `404 Not Found` - Order or restaurant not found
- `400 Bad Request` - Order already accepted or no riders available

---

## Riders API

### 1. Register New Rider

Creates a new rider account.

**Endpoint:** `POST /api/riders`

**Request Body:**
```json
{
  "name": "Vijay Kumar",
  "email": "vijay@example.com",
  "phone": "+91-9988776655",
  "vehicleNumber": "MH02AB1234",
  "currentLocation": {
    "lat": 19.0760,
    "lon": 72.8777
  },
  "riderStatus": "Available"
}
```

**Success Response:** `201 Created`
```json
{
  "id": 5,
  "name": "Vijay Kumar",
  "email": "vijay@example.com",
  "phone": "+91-9988776655",
  "vehicleNumber": "MH02AB1234",
  "currentLocation": {
    "lat": 19.0760,
    "lon": 72.8777
  },
  "riderStatus": "Available"
}
```

**Error Responses:**
- `400 Bad Request` - Validation failed or duplicate email

---

### 2. Get All Riders

Retrieves a list of all riders.

**Endpoint:** `GET /api/riders`

**Success Response:** `200 OK`
```json
[
  {
    "id": 5,
    "name": "Vijay Kumar",
    "email": "vijay@example.com",
    "phone": "+91-9988776655",
    "vehicleNumber": "MH02AB1234",
    "currentLocation": {
      "lat": 19.0760,
      "lon": 72.8777
    },
    "riderStatus": "Available"
  }
]
```

---

### 3. Get Rider by ID

Retrieves a specific rider's details.

**Endpoint:** `GET /api/riders/{id}`

**Parameters:**
- `id` (path, required) - Rider ID

**Success Response:** `200 OK`
```json
{
  "id": 5,
  "name": "Vijay Kumar",
  "email": "vijay@example.com",
  "phone": "+91-9988776655",
  "vehicleNumber": "MH02AB1234",
  "currentLocation": {
    "lat": 19.0760,
    "lon": 72.8777
  },
  "riderStatus": "Busy"
}
```

**Error Responses:**
- `404 Not Found` - Rider not found

---

### 4. Update Rider

Updates rider information.

**Endpoint:** `PUT /api/riders/{id}`

**Parameters:**
- `id` (path, required) - Rider ID

**Request Body:**
```json
{
  "name": "Vijay Kumar Singh",
  "email": "vijay.singh@example.com",
  "phone": "+91-9988776655",
  "vehicleNumber": "MH02AB1234",
  "currentLocation": {
    "lat": 19.0800,
    "lon": 72.8800
  },
  "riderStatus": "Available"
}
```

**Success Response:** `200 OK`

**Error Responses:**
- `400 Bad Request` - Validation failed
- `404 Not Found` - Rider not found

---

### 5. Delete Rider

Deletes a rider account.

**Endpoint:** `DELETE /api/riders/{id}`

**Parameters:**
- `id` (path, required) - Rider ID

**Success Response:** `204 No Content`

**Error Responses:**
- `404 Not Found` - Rider not found

---

### 6. Get Rider Orders

Retrieves all orders assigned to a specific rider.

**Endpoint:** `GET /api/riders/{id}/orders`

**Parameters:**
- `id` (path, required) - Rider ID

**Success Response:** `200 OK`
```json
[
  {
    "orderId": 101,
    "restaurantName": "Spice Garden",
    "customerName": "Rahul Sharma",
    "deliveryAddress": "123 MG Road, Mumbai",
    "status": "PickedUp",
    "totalAmount": 450.00
  }
]
```

---

### 7. Update Rider Location

Updates rider's current GPS location.

**Endpoint:** `PUT /api/riders/{id}/location`

**Parameters:**
- `id` (path, required) - Rider ID

**Request Body:**
```json
{
  "lat": 19.0850,
  "lon": 72.8850
}
```

**Success Response:** `200 OK`
```json
{
  "id": 5,
  "name": "Vijay Kumar",
  "currentLocation": {
    "lat": 19.0850,
    "lon": 72.8850
  },
  "message": "Location updated successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid coordinates
- `404 Not Found` - Rider not found

---

## Orders API

### 1. Calculate Cart (Preview Order)

Calculates order total and delivery estimate without placing the order.

**Endpoint:** `POST /api/orders/cart`

**Request Body:**
```json
{
  "userId": 1,
  "restaurantId": 1,
  "items": [
    {
      "menuItemId": 1,
      "quantity": 2,
      "specialInstructions": "Extra spicy"
    },
    {
      "menuItemId": 3,
      "quantity": 1
    }
  ]
}
```

**Success Response:** `200 OK`
```json
{
  "subtotal": 598.00,
  "deliveryFee": 40.00,
  "tax": 95.70,
  "total": 733.70,
  "estimatedDeliveryMinutes": 35,
  "items": [
    {
      "menuItemId": 1,
      "itemName": "Butter Chicken",
      "quantity": 2,
      "unitPrice": 299.00,
      "totalPrice": 598.00
    }
  ]
}
```

**Error Responses:**
- `400 Bad Request` - Invalid items or delivery out of range
- `404 Not Found` - Restaurant or user not found

---

### 2. Place Order

Places a new order with automatic calculation and validation.

**Endpoint:** `POST /api/orders`

**Request Body:**
```json
{
  "userId": 1,
  "restaurantId": 1,
  "deliveryAddress": {
    "street": "123 MG Road",
    "city": "Mumbai",
    "zipCode": "400001",
    "location": {
      "lat": 19.0760,
      "lon": 72.8777
    }
  },
  "items": [
    {
      "menuItemId": 1,
      "quantity": 2,
      "specialInstructions": "Extra spicy, less oil"
    }
  ],
  "specialInstructions": "Ring the bell twice"
}
```

**Success Response:** `201 Created`
```json
{
  "orderId": 101,
  "userId": 1,
  "restaurantId": 1,
  "restaurantName": "Spice Garden",
  "status": "Pending",
  "totalAmount": 733.70,
  "items": [
    {
      "orderItemId": 1,
      "menuItemId": 1,
      "itemName": "Butter Chicken",
      "quantity": 2,
      "unitPrice": 299.00,
      "totalPrice": 598.00,
      "specialInstructions": "Extra spicy, less oil"
    }
  ],
  "deliveryAddress": { ... },
  "orderedAt": "2025-11-06T12:00:00Z",
  "estimatedDeliveryTime": "2025-11-06T12:35:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Validation failed or out of delivery range
- `404 Not Found` - Restaurant or user not found

---

### 3. Get All Orders

Retrieves a list of all orders.

**Endpoint:** `GET /api/orders`

**Query Parameters:**
- `status` (optional) - Filter by order status (Pending, Confirmed, Preparing, PickedUp, Delivered, Cancelled)

**Success Response:** `200 OK`
```json
[
  {
    "orderId": 101,
    "userId": 1,
    "restaurantId": 1,
    "status": "Delivered",
    "totalAmount": 733.70,
    "orderedAt": "2025-11-06T12:00:00Z"
  }
]
```

---

### 4. Get Order by ID

Retrieves detailed information about a specific order.

**Endpoint:** `GET /api/orders/{id}`

**Parameters:**
- `id` (path, required) - Order ID

**Success Response:** `200 OK`
```json
{
  "orderId": 101,
  "userId": 1,
  "userName": "Rahul Sharma",
  "restaurantId": 1,
  "restaurantName": "Spice Garden",
  "riderId": 5,
  "riderName": "Vijay Kumar",
  "status": "PickedUp",
  "paymentStatus": "Pending",
  "totalAmount": 733.70,
  "items": [
    {
      "orderItemId": 1,
      "menuItemId": 1,
      "itemName": "Butter Chicken",
      "quantity": 2,
      "unitPrice": 299.00,
      "totalPrice": 598.00,
      "specialInstructions": "Extra spicy"
    }
  ],
  "deliveryAddress": {
    "street": "123 MG Road",
    "city": "Mumbai",
    "zipCode": "400001",
    "location": {
      "lat": 19.0760,
      "lon": 72.8777
    }
  },
  "specialInstructions": "Ring the bell twice",
  "orderedAt": "2025-11-06T12:00:00Z",
  "confirmedAt": "2025-11-06T12:02:00Z",
  "estimatedDeliveryTime": "2025-11-06T12:35:00Z"
}
```

**Error Responses:**
- `404 Not Found` - Order not found

---

### 5. Update Order

Updates order information (status, etc.).

**Endpoint:** `PUT /api/orders/{id}`

**Parameters:**
- `id` (path, required) - Order ID

**Request Body:**
```json
{
  "status": "Delivered",
  "paymentStatus": "Paid"
}
```

**Success Response:** `200 OK`

**Error Responses:**
- `400 Bad Request` - Invalid status transition
- `404 Not Found` - Order not found

---

### 6. Cancel Order

Cancels an order.

**Endpoint:** `DELETE /api/orders/{id}`

**Parameters:**
- `id` (path, required) - Order ID

**Success Response:** `204 No Content`

**Error Responses:**
- `400 Bad Request` - Order cannot be cancelled (already delivered)
- `404 Not Found` - Order not found

---

## Recommendations API

### 1. Get Restaurant Recommendations

Returns personalized restaurant recommendations based on user location, cuisine preference, and time constraints.

**Endpoint:** `GET /api/recommendations`

**Query Parameters:**
- `userId` (required) - User ID (system uses their registered address)
- `cuisineType` (required) - Desired cuisine type
- `maxTimeMinutes` (optional, default: 60) - Maximum acceptable delivery time

**Valid Cuisine Types:**
- `NorthIndian`
- `SouthIndian`
- `Mughlai`
- `Chinese`
- `Italian`
- `Biryani`
- `Pizza`
- `Burger`
- `FastFood`

**Example Request:**
```
GET /api/recommendations?userId=1&cuisineType=NorthIndian&maxTimeMinutes=45
```

**Success Response:** `200 OK`
```json
[
  {
    "restaurantId": 1,
    "name": "Spice Garden",
    "cuisineType": "NorthIndian",
    "rating": 4.5,
    "distanceKm": 2.3,
    "estimatedDeliveryMinutes": 30,
    "isOpen": true,
    "address": {
      "street": "45 Food Street",
      "city": "Mumbai",
      "zipCode": "400015"
    },
    "popularItems": [
      "Butter Chicken",
      "Paneer Tikka Masala",
      "Naan"
    ]
  },
  {
    "restaurantId": 3,
    "name": "Delhi Darbar",
    "cuisineType": "NorthIndian",
    "rating": 4.2,
    "distanceKm": 3.5,
    "estimatedDeliveryMinutes": 40,
    "isOpen": true,
    "address": { ... },
    "popularItems": [ ... ]
  }
]
```

**Sorting Logic:**
- Restaurants are ranked by a composite score:
  - Estimated delivery time (40% weight)
  - Restaurant rating (35% weight)
  - Distance from user (25% weight)

**Error Responses:**
- `400 Bad Request` - Invalid cuisine type or time parameter
- `404 Not Found` - User not found or user has no address configured

---

### 2. Get Available Cuisine Types

Returns a list of all available cuisine types in the system.

**Endpoint:** `GET /api/recommendations/cuisines`

**Success Response:** `200 OK`
```json
{
  "cuisineTypes": [
    "NorthIndian",
    "SouthIndian",
    "Mughlai",
    "Chinese",
    "Italian",
    "Biryani",
    "Pizza",
    "Burger",
    "FastFood"
  ]
}
```

---

## Error Responses

### Standard Error Format

All error responses follow a consistent ProblemDetails format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "Email": ["Email is required", "Email format is invalid"],
    "Phone": ["Phone number must be in format +XX-XXXXXXXXXX"]
  }
}
```

### Common HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Request succeeded |
| 201 | Created | Resource created successfully |
| 204 | No Content | Request succeeded, no content to return |
| 400 | Bad Request | Validation failed or invalid request |
| 404 | Not Found | Requested resource not found |
| 409 | Conflict | Duplicate entity or conflicting state |
| 500 | Internal Server Error | Unexpected server error |

---

## Data Models

### User

```json
{
  "id": 1,
  "name": "string (required, max 100 chars)",
  "email": "string (required, valid email format)",
  "phone": "string (required, format: +XX-XXXXXXXXXX)",
  "address": {
    "street": "string (required)",
    "city": "string (required)",
    "zipCode": "string (required)",
    "location": {
      "lat": "decimal (required, -90 to 90)",
      "lon": "decimal (required, -180 to 180)"
    }
  },
  "createdAt": "datetime (auto-generated)"
}
```

### Restaurant

```json
{
  "id": 1,
  "name": "string (required, max 100 chars)",
  "cuisineType": "enum (NorthIndian, SouthIndian, etc.)",
  "rating": "decimal (0.0 to 5.0)",
  "isOpen": "boolean",
  "averagePreparationTimeMinutes": "integer (required, > 0)",
  "address": { /* same as User address */ },
  "menuItems": [ /* array of MenuItem */ ]
}
```

### MenuItem

```json
{
  "id": 1,
  "restaurantId": 1,
  "name": "string (required)",
  "description": "string",
  "price": "decimal (required, > 0)",
  "category": "string (e.g., Appetizer, Main Course, Dessert)",
  "isVegetarian": "boolean",
  "isAvailable": "boolean"
}
```

### Rider

```json
{
  "id": 1,
  "name": "string (required)",
  "email": "string (required, unique)",
  "phone": "string (required)",
  "vehicleNumber": "string (required)",
  "currentLocation": {
    "lat": "decimal (required)",
    "lon": "decimal (required)"
  },
  "riderStatus": "enum (Available, Busy, Offline)"
}
```

### Order

```json
{
  "id": 1,
  "userId": 1,
  "restaurantId": 1,
  "riderId": 1,
  "status": "enum (Pending, Confirmed, Preparing, PickedUp, Delivered, Cancelled)",
  "paymentStatus": "enum (Pending, Paid, Failed, Refunded)",
  "totalAmount": "decimal",
  "items": [ /* array of OrderItem */ ],
  "deliveryAddress": { /* same as User address */ },
  "specialInstructions": "string",
  "orderedAt": "datetime",
  "confirmedAt": "datetime",
  "estimatedDeliveryTime": "datetime"
}
```

### OrderItem

```json
{
  "id": 1,
  "orderId": 1,
  "menuItemId": 1,
  "quantity": 2,
  "unitPrice": 299.00,
  "specialInstructions": "string"
}
```

### Order Status Flow

```
Pending → Confirmed → Preparing → PickedUp → Delivered
                ↓
            Cancelled
```

---

## Testing with Swagger UI

The API includes interactive Swagger documentation available at:

**URL:** `https://localhost:7233/swagger`

### Features:
- Try out all endpoints directly from the browser
- View request/response schemas
- Test with pre-populated sample data
- Export API definition (OpenAPI 3.0 JSON)

### Sample Test Flow:

1. **Register a User** - `POST /api/users`
2. **Get Recommendations** - `GET /api/recommendations?userId=1&cuisineType=NorthIndian`
3. **Place an Order** - `POST /api/orders`
4. **Restaurant Accepts Order** - `POST /api/restaurants/1/orders/101/accept`
5. **Update Rider Location** - `PUT /api/riders/5/location`
6. **Track Order** - `GET /api/orders/101`

---

## Code Examples

### cURL Examples

#### Place an Order
```bash
curl -X POST "https://localhost:7233/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "restaurantId": 1,
    "deliveryAddress": {
      "street": "123 Main St",
      "city": "Mumbai",
      "zipCode": "400001",
      "location": {
        "lat": 19.0760,
        "lon": 72.8777
      }
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

#### Get Recommendations
```bash
curl -X GET "https://localhost:7233/api/recommendations?userId=1&cuisineType=NorthIndian&maxTimeMinutes=45"
```
---

## Rate Limiting

**Current Status:** Not implemented (prototype)  
**Future:** Rate limiting will be added (e.g., 100 requests per minute per IP)