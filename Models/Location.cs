namespace FoodDeliveryPolaris.Models;

/// <summary>
/// Represents a geographical location with latitude and longitude coordinates.
/// Used to track positions of users, restaurants, and riders.
/// </summary>
public class Location
{
    /// <summary>
    /// Gets or sets the latitude coordinate of the location.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the location.
    /// </summary>
    public double Lon { get; set; }
}
