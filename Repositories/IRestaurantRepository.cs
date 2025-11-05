using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Repositories;

/// <summary>
/// Repository interface for Restaurant entity operations.
/// Defines contract for restaurant data access.
/// </summary>
public interface IRestaurantRepository
{
    /// <summary>
    /// Creates a new restaurant in the system.
    /// </summary>
    /// <param name="restaurant">The restaurant to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created restaurant with generated ID.</returns>
    Task<Restaurant> CreateAsync(Restaurant restaurant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a restaurant by its unique identifier.
    /// </summary>
    /// <param name="id">The restaurant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The restaurant if found, otherwise null.</returns>
    Task<Restaurant?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registered restaurants.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all restaurants.</returns>
    Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing restaurant.
    /// </summary>
    /// <param name="restaurant">The restaurant with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated restaurant.</returns>
    Task<Restaurant> UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a restaurant from the system.
    /// </summary>
    /// <param name="id">The restaurant ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a restaurant exists by name.
    /// </summary>
    /// <param name="name">The restaurant name.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if restaurant exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
