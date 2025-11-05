using FoodDeliveryPolaris.Models;
using System.Text.Json.Serialization;

namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// Data Transfer Object for menu items.
/// Used when creating or updating restaurants with menu items.
/// </summary>
public class MenuItemRequest
{
    /// <summary>
    /// Gets or sets the name of the menu item.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the price of the menu item.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets whether the menu item is currently available for ordering.
    /// Defaults to true.
    /// </summary>
    public bool Available { get; set; } = true;

    /// <summary>
    /// Gets or sets the cuisine type of this menu item.
    /// Accepts string values like "Italian", "Indian", "Chinese", etc.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CuisineType CuisineType { get; set; }

    /// <summary>
    /// Gets or sets the preparation time in minutes for this menu item.
    /// Defaults to 20 minutes.
    /// </summary>
    public int PreparationTimeMinutes { get; set; } = 20;

    /// <summary>
    /// Converts this DTO to a MenuItem model.
    /// </summary>
    public MenuItem ToMenuItem()
    {
        return new MenuItem
        {
            Name = Name,
            Price = Price,
            Available = Available,
            CuisineType = CuisineType,
            PreparationTimeMinutes = PreparationTimeMinutes
        };
    }
}
