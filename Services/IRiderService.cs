using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service interface for Rider registration and management.
/// Provides business logic for rider operations.
/// </summary>
public interface IRiderService
{
    /// <summary>
    /// Creates a new rider with validation and business rules.
    /// </summary>
    /// <param name="rider">The rider to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created rider.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when rider already exists.</exception>
    Task<Rider> CreateRiderAsync(Rider rider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a rider by their ID.
    /// </summary>
    /// <param name="id">The rider ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The rider if found, otherwise null.</returns>
    Task<Rider?> GetRiderByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registered riders.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all riders.</returns>
    Task<IEnumerable<Rider>> GetAllRidersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing rider with validation.
    /// </summary>
    /// <param name="id">The rider ID to update.</param>
    /// <param name="rider">The updated rider information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated rider.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when updated name conflicts with another rider.</exception>
    Task<Rider?> UpdateRiderAsync(int id, Rider rider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a rider's current location.
    /// Used when rider moves and needs to update GPS coordinates for accurate distance calculations.
    /// </summary>
    /// <param name="riderId">The rider ID whose location to update.</param>
    /// <param name="newLocation">The new GPS coordinates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated rider with new location.</returns>
    /// <exception cref="Exceptions.NotFoundException">Thrown when rider is not found.</exception>
    Task<Rider> UpdateRiderLocationAsync(
        int riderId, 
        Location newLocation, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a rider from the system.
    /// </summary>
    /// <param name="id">The rider ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteRiderAsync(int id, CancellationToken cancellationToken = default);
}
