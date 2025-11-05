using FoodDeliveryPolaris.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// System automatically fetches user's address and validates restaurant.
/// </summary>
public class PlaceOrderRequest
{
    /// <summary>
    /// Gets or sets the user ID placing the order (from login/session).
    /// </summary>
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the restaurant ID to order from.
    /// </summary>
    [Required(ErrorMessage = "Restaurant ID is required")]
    public int RestaurantId { get; set; }

    /// <summary>
    /// Gets or sets the list of items to order.
    /// </summary>
    [Required(ErrorMessage = "Order items are required")]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    public List<OrderItemRequest> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets optional special instructions (e.g., "Extra spicy", "No onions").
    /// </summary>
    [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// Data Transfer Object for an order item.
/// </summary>
public class OrderItemRequest
{
    /// <summary>
    /// Gets or sets the menu item ID to order.
    /// </summary>
    [Required(ErrorMessage = "Menu item ID is required")]
    public int MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the quantity to order.
    /// </summary>
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}

/// <summary>
/// Data Transfer Object for order response.
/// Contains complete order details with calculated totals and enriched data.
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// Gets or sets the unique order identifier.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the restaurant name.
    /// </summary>
    public string RestaurantName { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the rider name (null until assigned).
    /// </summary>
    public string? RiderName { get; set; }

    /// <summary>
    /// Gets or sets the list of order items with details.
    /// </summary>
    public List<OrderItemResponse> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the total order amount.
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// Gets or sets the order status as a string.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the payment status as a string.
    /// </summary>
    public string PaymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the order was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the estimated delivery time in minutes from now.
    /// Calculated based on prep time + delivery time.
    /// </summary>
    public int? EstimatedDeliveryMinutes { get; set; }

    /// <summary>
    /// Gets or sets the delivery address (user's registered address).
    /// </summary>
    public string DeliveryAddress { get; set; }

    /// <summary>
    /// Gets or sets the special instructions or notes.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data Transfer Object for order item response.
/// Contains item details with calculated item total.
/// </summary>
public class OrderItemResponse
{
    /// <summary>
    /// Gets or sets the menu item ID.
    /// </summary>
    public int MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the item price.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets the quantity ordered.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the item total (price × quantity).
    /// </summary>
    public double ItemTotal { get; set; }
}

/// <summary>
/// Data Transfer Object for rider assignment response.
/// Contains rider information and assignment details.
/// </summary>
public class RiderAssignmentResponse
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the order status after assignment.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the assigned rider ID.
    /// </summary>
    public int RiderId { get; set; }

    /// <summary>
    /// Gets or sets the rider's name.
    /// </summary>
    public string RiderName { get; set; }

    /// <summary>
    /// Gets or sets the rider's phone number.
    /// </summary>
    public string RiderPhone { get; set; }

    /// <summary>
    /// Gets or sets the rider's vehicle number.
    /// </summary>
    public string VehicleNumber { get; set; }

    /// <summary>
    /// Gets or sets the distance from rider to restaurant in kilometers.
    /// </summary>
    public double DistanceToRestaurantKm { get; set; }

    /// <summary>
    /// Gets or sets the estimated time for rider to reach restaurant in minutes.
    /// </summary>
    public int EstimatedPickupTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets a message describing the assignment.
    /// </summary>
    public string Message { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an order status.
/// Used for marking order as delivered, cancelled, etc.
/// </summary>
/// <example>
/// {
///   "status": "Delivered",
///   "paymentStatus": "Paid"
/// }
/// </example>
public class UpdateOrderRequest
{
    /// <summary>
    /// Gets or sets the order status as string.
    /// </summary>
    /// <example>Delivered</example>
    /// <remarks>
    /// Valid values:
    /// - "Placed" - Order has been placed but not yet accepted
    /// - "Accepted" - Restaurant has accepted the order
    /// - "Preparing" - Food is being prepared
    /// - "Ready" - Food is ready for pickup
    /// - "PickedUp" - Rider has picked up the food
    /// - "Delivered" - Order has been delivered
    /// - "Cancelled" - Order was cancelled
    /// </remarks>
    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the payment status as string.
    /// </summary>
    /// <example>Paid</example>
    /// <remarks>
    /// Valid values:
    /// - "NotPaid" - Payment not yet received
    /// - "Paid" - Payment completed successfully
    /// - "Failed" - Payment failed
    /// - "Refunded" - Payment was refunded
    /// </remarks>
    public string? PaymentStatus { get; set; }
}

/// <summary>
/// Data Transfer Object for order preview/cart calculation.
/// Shows price breakdown without creating an order.
/// </summary>
public class OrderPreviewResponse
{
    /// <summary>
    /// Gets or sets the restaurant name.
    /// </summary>
    public string RestaurantName { get; set; }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the list of items with prices.
    /// </summary>
    public List<OrderItemResponse> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the total order amount (sum of all items).
    /// </summary>
    public double Total { get; set; }

    /// <summary>
    /// Gets or sets the distance from user to restaurant in kilometers.
    /// </summary>
    public double DistanceKm { get; set; }

    /// <summary>
    /// Gets or sets the estimated delivery time in minutes.
    /// </summary>
    public int EstimatedDeliveryTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the delivery address (user's registered address).
    /// </summary>
    public string DeliveryAddress { get; set; }
}
