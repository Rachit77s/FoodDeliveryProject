using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Repositories;
using FoodDeliveryPolaris.Validators;
using Microsoft.Extensions.Logging;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service implementation for Rider registration and management.
/// Implements validation, business logic, and error handling.
/// </summary>
public class RiderService : IRiderService
{
    private readonly IRiderRepository _riderRepository;
    private readonly ILogger<RiderService> _logger;

    public RiderService(
        IRiderRepository riderRepository, 
        ILogger<RiderService> logger)
    {
        _riderRepository = riderRepository ?? throw new ArgumentNullException(nameof(riderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<Rider> CreateRiderAsync(Rider rider, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create rider: {RiderName}", rider?.Name);

        // Validate the rider
        var validationErrors = RiderValidator.Validate(rider);
        if (validationErrors.Any())
        {
            _logger.LogWarning("Rider validation failed: {Errors}", 
                string.Join(", ", validationErrors.SelectMany(e => e.Value)));
            throw new ValidationException(validationErrors);
        }

        // Check for duplicate rider by name
        var exists = await _riderRepository.ExistsByNameAsync(rider.Name, cancellationToken: cancellationToken);
        if (exists)
        {
            _logger.LogWarning("Rider with name {RiderName} already exists", rider.Name);
            throw new DuplicateEntityException($"A rider with the name '{rider.Name}' already exists.");
        }

        // Create the rider
        try
        {
            var createdRider = await _riderRepository.CreateAsync(rider, cancellationToken);
            _logger.LogInformation("Successfully created rider: {RiderId} - {RiderName}", 
                createdRider.Id, createdRider.Name);
            return createdRider;
        }
        catch (Exception ex) when (ex is not DuplicateEntityException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error occurred while creating rider: {RiderName}", rider.Name);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Rider?> GetRiderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        

        _logger.LogDebug("Retrieving rider with ID: {RiderId}", id);
        return await _riderRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Rider>> GetAllRidersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all riders");
        return await _riderRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Rider?> UpdateRiderAsync(int id, Rider rider, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update rider: {RiderId}", id);

        // Check if rider exists
        var existingRider = await _riderRepository.GetByIdAsync(id, cancellationToken);
        if (existingRider == null)
        {
            _logger.LogWarning("Rider with ID {RiderId} not found for update", id);
            return null;
        }

        // Ensure the ID matches
        rider.Id = id;

        // Validate the rider
        var validationErrors = RiderValidator.Validate(rider);
        if (validationErrors.Any())
        {
            _logger.LogWarning("Rider update validation failed: {Errors}", 
                string.Join(", ", validationErrors.SelectMany(e => e.Value)));
            throw new ValidationException(validationErrors);
        }

        // Check for duplicate name (excluding current rider)
        var nameExists = await _riderRepository.ExistsByNameAsync(rider.Name, excludeId: id, cancellationToken: cancellationToken);
        if (nameExists)
        {
            _logger.LogWarning("Rider name {RiderName} already exists for another rider", rider.Name);
            throw new DuplicateEntityException($"A rider with the name '{rider.Name}' already exists.");
        }

        // Update the rider
        try
        {
            var updatedRider = await _riderRepository.UpdateAsync(rider, cancellationToken);
            _logger.LogInformation("Successfully updated rider: {RiderId} - {RiderName}", 
                updatedRider.Id, updatedRider.Name);
            return updatedRider;
        }
        catch (Exception ex) when (ex is not DuplicateEntityException && ex is not ValidationException)
        {
            _logger.LogError(ex, "Error occurred while updating rider: {RiderId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Rider> UpdateRiderLocationAsync(
        int riderId, 
        Location newLocation, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating location for rider {RiderId}", riderId);

        // Validate new location
        if (newLocation == null)
        {
            _logger.LogWarning("UpdateRiderLocationAsync called with null location");
            throw new ArgumentNullException(nameof(newLocation), "Location cannot be null");
        }

        // Validate latitude and longitude ranges
        if (newLocation.Lat < -90 || newLocation.Lat > 90)
        {
            throw new ArgumentException("Latitude must be between -90 and 90 degrees", nameof(newLocation));
        }

        if (newLocation.Lon < -180 || newLocation.Lon > 180)
        {
            throw new ArgumentException("Longitude must be between -180 and 180 degrees", nameof(newLocation));
        }

        // Get existing rider
        var rider = await _riderRepository.GetByIdAsync(riderId, cancellationToken);
        if (rider == null)
        {
            _logger.LogWarning("Rider with ID {RiderId} not found for location update", riderId);
            throw new NotFoundException($"Rider with ID {riderId} not found");
        }

        // Calculate distance moved (for logging/analytics)
        var oldLocation = rider.CurrentLocation;
        double distanceMoved = 0;
        
        if (oldLocation != null && oldLocation.Lat != 0 && oldLocation.Lon != 0)
        {
            distanceMoved = Utils.LocationUtils.CalculateDistance(oldLocation, newLocation);
        }

        // Update location
        rider.CurrentLocation = newLocation;

        // Update in database
        var updatedRider = await _riderRepository.UpdateAsync(rider, cancellationToken);

        _logger.LogInformation(
            "Successfully updated location for rider {RiderId} ({RiderName}). " +
            "New location: ({Lat}, {Lon}), Distance moved: {Distance:F2} km",
            riderId, rider.Name, newLocation.Lat, newLocation.Lon, distanceMoved);

        return updatedRider;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteRiderAsync(int id, CancellationToken cancellationToken = default)
    {
        

        _logger.LogInformation("Attempting to delete rider: {RiderId}", id);

        try
        {
            var deleted = await _riderRepository.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                _logger.LogInformation("Successfully deleted rider: {RiderId}", id);
            }
            else
            {
                _logger.LogWarning("Rider with ID {RiderId} not found for deletion", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting rider: {RiderId}", id);
            throw;
        }
    }
}







