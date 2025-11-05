using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// Data Transfer Object for creating a new restaurant.
/// </summary>
public class CreateRestaurantRequest
{
    /// <summary>
    /// Gets or sets the name of the restaurant.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the restaurant.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical address of the restaurant.
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Gets or sets whether the restaurant is currently open.
    /// </summary>
    public bool IsOpen { get; set; } = true;

    /// <summary>
    /// Gets or sets the delivery radius in kilometers.
    /// Defaults to 5 km.
    /// </summary>
    public double DeliveryRadiusKm { get; set; } = 5.0;

    /// <summary>
    /// Gets or sets the average preparation time in minutes.
    /// Defaults to 20 minutes.
    /// </summary>
    public int AveragePreparationTimeMinutes { get; set; } = 20;

    /// <summary>
    /// Gets or sets the restaurant's rating (0-5 stars).
    /// Defaults to 4.0.
    /// </summary>
    public double Rating { get; set; } = 4.0;

    /// <summary>
    /// Gets or sets the list of menu items (without IDs).
    /// </summary>
    public List<MenuItemRequest> Menu { get; set; } = new();

    /// <summary>
    /// Converts this DTO to a Restaurant model.
    /// </summary>
    public Restaurant ToRestaurant()
    {
        return new Restaurant
        {
            Name = Name,
            Phone = Phone,
            Address = Address,
            IsOpen = IsOpen,
            DeliveryRadiusKm = DeliveryRadiusKm,
            AveragePreparationTimeMinutes = AveragePreparationTimeMinutes,
            Rating = Rating,
            Menu = Menu.Select(m => m.ToMenuItem()).ToList()  // Convert MenuItemRequest to MenuItem
        };
    }
}

/// <summary>
/// Data Transfer Object for updating an existing restaurant.
/// </summary>
public class UpdateRestaurantRequest
{
    /// <summary>
    /// Gets or sets the name of the restaurant.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the restaurant.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical address of the restaurant.
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Gets or sets whether the restaurant is currently open.
    /// </summary>
    public bool IsOpen { get; set; } = true;

    /// <summary>
    /// Gets or sets the delivery radius in kilometers.
    /// Defaults to 5 km.
    /// </summary>
    public double DeliveryRadiusKm { get; set; } = 5.0;

    /// <summary>
    /// Gets or sets the average preparation time in minutes.
    /// Defaults to 20 minutes.
    /// </summary>
    public int AveragePreparationTimeMinutes { get; set; } = 20;

    /// <summary>
    /// Gets or sets the restaurant's rating (0-5 stars).
    /// Defaults to 4.0.
    /// </summary>
    public double Rating { get; set; } = 4.0;

    /// <summary>
    /// Gets or sets the list of menu items (without IDs).
    /// </summary>
    public List<MenuItemRequest> Menu { get; set; } = new();

    /// <summary>
    /// Converts this DTO to a Restaurant model with the specified ID.
    /// </summary>
    public Restaurant ToRestaurant(int id)
    {
        return new Restaurant
        {
            Id = id,
            Name = Name,
            Phone = Phone,
            Address = Address,
            IsOpen = IsOpen,
            DeliveryRadiusKm = DeliveryRadiusKm,
            AveragePreparationTimeMinutes = AveragePreparationTimeMinutes,
            Rating = Rating,
            Menu = Menu.Select(m => m.ToMenuItem()).ToList()
        };
    }
}
