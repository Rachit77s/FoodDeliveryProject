using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service interface for Order management and history.
/// Provides business logic for order operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Places a new order with validation and total calculation.
    /// Validates restaurant, user, items, and delivery radius.
    /// Calculates subtotal, tax, delivery fee, and total.
    /// </summary>
    /// <param name="request">The order placement request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created order response with all details.</returns>
    Task<OrderResponse> PlaceOrderAsync(
        PlaceOrderRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restaurant accepts an order and triggers rider assignment.
    /// Updates order status from Placed to Accepted.
    /// Automatically attempts to assign nearest available rider.
    /// </summary>
    /// <param name="orderId">The order ID to accept.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated order response with acceptance confirmation.</returns>
    Task<OrderResponse> AcceptOrderAsync(
        int orderId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds and assigns the nearest available rider to an order.
    /// Calculates distances from all available riders to restaurant.
    /// Assigns closest rider and updates both order and rider status.
    /// </summary>
    /// <param name="orderId">The order ID to assign a rider to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Rider assignment details including distance and ETA.</returns>
    Task<RiderAssignmentResponse> AssignNearestRiderAsync(
        int orderId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found, otherwise null.</returns>
    Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order by its ID with enriched details (restaurant name, user name, rider name, ETA).
    /// Returns user-friendly OrderResponse DTO with string status values.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order response with enriched details if found, otherwise null.</returns>
    Task<OrderResponse?> GetOrderByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order history for a specific user.
    /// Returns paginated list of all orders placed by the user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of orders per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated order history with total count.</returns>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetUserOrderHistoryAsync(
        int userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order history for a specific rider.
    /// Returns paginated list of all orders assigned to the rider.
    /// </summary>
    /// <param name="riderId">The rider ID.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of orders per page.</param>
    /// <param name="includeInProgress">Include orders that are in progress.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated order history with total count.</returns>
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetRiderOrderHistoryAsync(
        int riderId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        bool includeInProgress = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all orders.</returns>
    Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="id">The order ID to update.</param>
    /// <param name="order">The updated order information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated order if found, otherwise null.</returns>
    Task<Order?> UpdateOrderAsync(int id, Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an order from the system.
    /// </summary>
    /// <param name="id">The order ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteOrderAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Preview order total without placing the order.
    /// Calculates subtotal, tax, delivery fee, and total for the given items.
    /// </summary>
    /// <param name="request">Order preview request with restaurant, user, and items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Price breakdown without creating an order</returns>
    Task<OrderPreviewResponse> PreviewOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default);
}
