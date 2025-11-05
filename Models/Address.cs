namespace FoodDeliveryPolaris.Models;

/// <summary>
/// Represents a physical address with street details and geographical location.
/// Used for delivery addresses and restaurant locations.
/// </summary>
public class Address
{
    /// <summary>
    /// Gets or sets the street address (e.g., "123 Main Street, Apt 4B").
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ZIP/postal code.
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the geographical location (latitude and longitude coordinates).
    /// Used for distance calculations and mapping.
    /// </summary>
    public Location Location { get; set; } = new Location();
}
