using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service interface for User registration and management.
/// Provides business logic for user operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Creates a new user with validation and business rules.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when user already exists.</exception>
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registered users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all users.</returns>
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user with validation.
    /// </summary>
    /// <param name="id">The user ID to update.</param>
    /// <param name="user">The updated user information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="Exceptions.DuplicateEntityException">Thrown when updated name conflicts with another user.</exception>
    Task<User?> UpdateUserAsync(int id, User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="id">The user ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}
