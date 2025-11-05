using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Repositories;

/// <summary>
/// Repository interface for Order entity operations.
/// Defines contract for order data access and history retrieval.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Creates a new order in the system.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created order with generated ID.</returns>
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an order by its unique identifier.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order if found, otherwise null.</returns>
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all orders in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all orders.</returns>
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order history for a specific user.
    /// Returns all orders placed by the user, ordered by creation date (newest first).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="pageNumber">Page number for pagination (1-based).</param>
    /// <param name="pageSize">Number of orders per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of orders placed by the user.</returns>
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(
        int userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order history for a specific rider.
    /// Returns all orders assigned to the rider, ordered by creation date (newest first).
    /// </summary>
    /// <param name="riderId">The rider/delivery partner ID.</param>
    /// <param name="pageNumber">Page number for pagination (1-based).</param>
    /// <param name="pageSize">Number of orders per page.</param>
    /// <param name="includeInProgress">Include orders that are in progress (not completed).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of orders assigned to the rider.</returns>
    Task<IEnumerable<Order>> GetOrdersByRiderIdAsync(
        int riderId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        bool includeInProgress = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of orders for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total number of orders.</returns>
    Task<int> GetUserOrderCountAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of orders for a rider.
    /// </summary>
    /// <param name="riderId">The rider ID.</param>
    /// <param name="includeInProgress">Include orders that are in progress.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total number of orders.</returns>
    Task<int> GetRiderOrderCountAsync(
        int riderId, 
        bool includeInProgress = true, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated order.</returns>
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an order from the system.
    /// </summary>
    /// <param name="id">The order ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
