using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Repositories;
using FoodDeliveryPolaris.Validators;
using Microsoft.Extensions.Logging;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service implementation for Restaurant registration and management.
/// Implements validation, business logic, and error handling.
/// </summary>
public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly ILogger<RestaurantService> _logger;

    public RestaurantService(
        IRestaurantRepository restaurantRepository, 
        ILogger<RestaurantService> logger)
    {
        _restaurantRepository = restaurantRepository ?? throw new ArgumentNullException(nameof(restaurantRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<Restaurant> CreateRestaurantAsync(
        Restaurant restaurant, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create restaurant: {RestaurantName}", restaurant?.Name);

        // Validate the restaurant
        var validationErrors = RestaurantValidator.Validate(restaurant);
        if (validationErrors.Any())
        {
            _logger.LogWarning("Restaurant validation failed: {Errors}", 
                string.Join(", ", validationErrors.SelectMany(e => e.Value)));
            throw new ValidationException(validationErrors);
        }

        // Check for duplicate restaurant by name
        var exists = await _restaurantRepository.ExistsByNameAsync(restaurant.Name, cancellationToken: cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Restaurant with name {RestaurantName} already exists", restaurant.Name);
            throw new DuplicateEntityException($"A restaurant with the name '{restaurant.Name}' already exists.");
        }

        // Create the restaurant
        try
        {
            var createdRestaurant = await _restaurantRepository.CreateAsync(restaurant, cancellationToken);
            _logger.LogInformation("Successfully created restaurant: {RestaurantId} - {RestaurantName}", 
                createdRestaurant.Id, createdRestaurant.Name);
            return createdRestaurant;
        }
        catch (Exception ex) when (ex is not DuplicateEntityException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error occurred while creating restaurant: {RestaurantName}", restaurant.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Restaurant?> GetRestaurantByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        

        _logger.LogDebug("Retrieving restaurant with ID: {RestaurantId}", id);
        return await _restaurantRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all restaurants");
        return await _restaurantRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Restaurant?> UpdateRestaurantAsync(int id, Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update restaurant: {RestaurantId}", id);

        // Check if restaurant exists
        var existingRestaurant = await _restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (existingRestaurant == null)
        {
            _logger.LogWarning("Restaurant with ID {RestaurantId} not found for update", id);
            return null;
        }

        // Ensure the ID matches
        restaurant.Id = id;

        // Validate the restaurant
        var validationErrors = RestaurantValidator.Validate(restaurant);
        if (validationErrors.Any())
        {
            _logger.LogWarning("Restaurant update validation failed: {Errors}", 
                string.Join(", ", validationErrors.SelectMany(e => e.Value)));
            throw new ValidationException(validationErrors);
        }

        // Check for duplicate name (excluding current restaurant)
        var nameExists = await _restaurantRepository.ExistsByNameAsync(restaurant.Name, excludeId: id, cancellationToken: cancellationToken);
        if (nameExists)
        {
            _logger.LogWarning("Restaurant name {RestaurantName} already exists for another restaurant", restaurant.Name);
            throw new DuplicateEntityException($"A restaurant with the name '{restaurant.Name}' already exists.");
        }

        // Update the restaurant
        try
        {
            var updatedRestaurant = await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);
            _logger.LogInformation("Successfully updated restaurant: {RestaurantId} - {RestaurantName}", 
                updatedRestaurant.Id, updatedRestaurant.Name);
            return updatedRestaurant;
        }
        catch (Exception ex) when (ex is not DuplicateEntityException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error occurred while updating restaurant: {RestaurantId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteRestaurantAsync(int id, CancellationToken cancellationToken = default)
    {
        

        _logger.LogInformation("Attempting to delete restaurant: {RestaurantId}", id);

        try
        {
            var deleted = await _restaurantRepository.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted restaurant: {RestaurantId}", id);
            }
            else
            {
                _logger.LogWarning("Restaurant with ID {RestaurantId} not found for deletion", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting restaurant: {RestaurantId}", id);
            throw;
        }
    }
}


