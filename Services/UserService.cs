using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Repositories;
using FoodDeliveryPolaris.Validators;
using Microsoft.Extensions.Logging;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service implementation for User registration and management.
/// Implements validation, business logic, and error handling.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create user: {UserName}", user?.Name);

        // Validate the user
        var validationErrors = UserValidator.Validate(user);
        if (validationErrors.Any())
        {
            _logger.LogWarning("User validation failed: {Errors}", 
                string.Join(", ", validationErrors.SelectMany(e => e.Value)));
            throw new ValidationException(validationErrors);
        }

        // Check for duplicate user by name
        var exists = await _userRepository.ExistsByNameAsync(user.Name, cancellationToken: cancellationToken);
        if (exists)
        {
            _logger.LogWarning("User with name {UserName} already exists", user.Name);
            throw new DuplicateEntityException($"A user with the name '{user.Name}' already exists.");
        }

        // Create the user
        try
        {
            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);
            _logger.LogInformation("Successfully created user: {UserId} - {UserName}", 
                createdUser.Id, createdUser.Name);
            return createdUser;
        }
        catch (Exception ex) when (ex is not DuplicateEntityException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error occurred while creating user: {UserName}", user.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving user with ID: {UserId}", id);
        return await _userRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all users");
        return await _userRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<User?> UpdateUserAsync(int id, User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update user: {UserId}", id);

        // Check if user exists
        var existingUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (existingUser == null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update", id);
            return null;
        }

        // Ensure the ID matches
        user.Id = id;

        // Validate the user
        var validationErrors = UserValidator.Validate(user);
        if (validationErrors.Any())
        {
            _logger.LogWarning("User update validation failed: {Errors}", 
                string.Join(", ", validationErrors.SelectMany(e => e.Value)));
            throw new ValidationException(validationErrors);
        }

        // Check for duplicate name (excluding current user)
        var nameExists = await _userRepository.ExistsByNameAsync(user.Name, excludeId: id, cancellationToken: cancellationToken);
        if (nameExists)
        {
            _logger.LogWarning("User name {UserName} already exists for another user", user.Name);
            throw new DuplicateEntityException($"A user with the name '{user.Name}' already exists.");
        }

        // Update the user
        try
        {
            var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("Successfully updated user: {UserId} - {UserName}", 
                updatedUser.Id, updatedUser.Name);
            return updatedUser;
        }
        catch (Exception ex) when (ex is not DuplicateEntityException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error occurred while updating user: {UserId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete user: {UserId}", id);

        try
        {
            var deleted = await _userRepository.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted user: {UserId}", id);
            }
            else
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user: {UserId}", id);
            throw;
        }
    }
}

