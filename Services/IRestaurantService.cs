using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service interface for Restaurant registration and management.
/// Provides business logic for restaurant operations.
/// </summary>
public interface IRestaurantService
{
    /// <summary>
    /// Creates a new restaurant with validation and business rules.
    /// </summary>
    /// <param name="restaurant">The restaurant to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created restaurant.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when restaurant already exists.</exception>
    Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a restaurant by its ID.
    /// </summary>
    /// <param name="id">The restaurant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The restaurant if found, otherwise null.</returns>
    Task<Restaurant?> GetRestaurantByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registered restaurants.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all restaurants.</returns>
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing restaurant with validation.
    /// </summary>
    /// <param name="id">The restaurant ID to update.</param>
    /// <param name="restaurant">The updated restaurant information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated restaurant.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when updated name conflicts with another restaurant.</exception>
    Task<Restaurant?> UpdateRestaurantAsync(int id, Restaurant restaurant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a restaurant from the system.
    /// </summary>
    /// <param name="id">The restaurant ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteRestaurantAsync(int id, CancellationToken cancellationToken = default);
}
