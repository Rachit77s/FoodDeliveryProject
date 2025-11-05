using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Repositories;

/// <summary>
/// Repository interface for Rider entity operations.
/// Defines contract for rider data access.
/// </summary>
public interface IRiderRepository
{
    /// <summary>
    /// Creates a new rider in the system.
    /// </summary>
    /// <param name="rider">The rider to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created rider with generated ID.</returns>
    Task<Rider> CreateAsync(Rider rider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a rider by their unique identifier.
    /// </summary>
    /// <param name="id">The rider ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The rider if found, otherwise null.</returns>
    Task<Rider?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all riders in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all riders.</returns>
    Task<IEnumerable<Rider>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing rider.
    /// </summary>
    /// <param name="rider">The rider with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated rider.</returns>
    Task<Rider> UpdateAsync(Rider rider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a rider from the system.
    /// </summary>
    /// <param name="id">The rider ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a rider exists by name.
    /// </summary>
    /// <param name="name">The rider name.</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if rider exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
