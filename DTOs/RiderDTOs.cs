using FoodDeliveryPolaris.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// Data Transfer Object for creating a new rider.
/// </summary>
public class CreateRiderRequest
{
    /// <summary>
    /// Gets or sets the rider's name.
    /// </summary>
    [Required(ErrorMessage = "Rider name is required")]
    [MinLength(2, ErrorMessage = "Rider name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Rider name cannot exceed 100 characters")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the rider's email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rider's phone number.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle number.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Vehicle number cannot exceed 50 characters")]
    public string VehicleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rider's current location (optional at registration).
    /// </summary>
    public Location CurrentLocation { get; set; } = new Location();

    /// <summary>
    /// Converts this DTO to a Rider model.
    /// </summary>
    public Rider ToRider()
    {
        return new Rider
        {
            Name = Name,
            Email = Email,
            Phone = Phone,
            VehicleNumber = VehicleNumber,
            CurrentLocation = CurrentLocation,
            RiderStatus = RiderStatus.Available
        };
    }
}

/// <summary>
/// Data Transfer Object for updating a rider's location.
/// Used when rider moves and needs to update their GPS coordinates.
/// </summary>
public class UpdateRiderLocationRequest
{
    /// <summary>
    /// Gets or sets the new location coordinates.
    /// </summary>
    [Required(ErrorMessage = "Location is required")]
    public Location NewLocation { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when location was captured (optional).
    /// Defaults to current UTC time.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Data Transfer Object for updating a rider.
/// </summary>
public class UpdateRiderRequest
{
    /// <summary>
    /// Gets or sets the rider's name.
    /// </summary>
    [MinLength(2, ErrorMessage = "Rider name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Rider name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rider's email address.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rider's phone number.
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle number.
    /// </summary>
    [MaxLength(50, ErrorMessage = "Vehicle number cannot exceed 50 characters")]
    public string VehicleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rider status as string.
    /// Valid values: "Available", "Busy", "Offline"
    /// </summary>
    public string? RiderStatus { get; set; }

    /// <summary>
    /// Converts this DTO to a Rider model with the specified ID.
    /// </summary>
    public Rider ToRider(int id, Rider existingRider)
    {
        if (!string.IsNullOrEmpty(Name)) existingRider.Name = Name;
        if (!string.IsNullOrEmpty(Email)) existingRider.Email = Email;
        if (!string.IsNullOrEmpty(Phone)) existingRider.Phone = Phone;
        if (!string.IsNullOrEmpty(VehicleNumber)) existingRider.VehicleNumber = VehicleNumber;
        
        // Parse string to enum
        if (!string.IsNullOrEmpty(RiderStatus))
        {
            if (Enum.TryParse<Models.RiderStatus>(RiderStatus, true, out var status))
            {
                existingRider.RiderStatus = status;
            }
        }

        return existingRider;
    }
}

/// <summary>
/// Data Transfer Object for rider response with user-friendly values.
/// Returns rider details with string status instead of enum integers.
/// </summary>
public class RiderResponse
{
    /// <summary>
    /// Gets or sets the unique rider identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the rider's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the rider's current GPS location.
    /// </summary>
    public Location CurrentLocation { get; set; }

    /// <summary>
    /// Gets or sets the rider's email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the rider's phone number.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the vehicle number.
    /// </summary>
    public string VehicleNumber { get; set; }

    /// <summary>
    /// Gets or sets the rider status as string.
    /// Values: "Available", "Busy", "Offline"
    /// </summary>
    public string RiderStatus { get; set; }
}
