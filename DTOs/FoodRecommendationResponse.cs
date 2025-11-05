namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// Data Transfer Object for food recommendation response.
/// Contains restaurant information, time estimates, and matching menu items.
/// </summary>
public class FoodRecommendationResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the restaurant.
    /// </summary>
    public int RestaurantId { get; set; }

    /// <summary>
    /// Gets or sets the name of the restaurant.
    /// </summary>
    public string RestaurantName { get; set; }

    /// <summary>
    /// Gets or sets the restaurant's rating (0-5 stars).
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Gets or sets the distance from user to restaurant in kilometers.
    /// </summary>
    public double DistanceKm { get; set; }

    /// <summary>
    /// Gets or sets the total estimated time in minutes.
    /// Includes both preparation time and delivery time.
    /// This is the total time from ordering until food arrives.
    /// </summary>
    public int TotalEstimatedTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the restaurant's delivery address.
    /// Formatted as "Street, City".
    /// </summary>
    public string DeliveryAddress { get; set; }

    /// <summary>
    /// Gets or sets the list of recommended menu items matching the search criteria.
    /// </summary>
    public List<RecommendedMenuItem> RecommendedItems { get; set; } = new();
}

/// <summary>
/// Data Transfer Object for a recommended menu item.
/// Contains item details for display in search results.
/// </summary>
public class RecommendedMenuItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the menu item.
    /// </summary>
    public int MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the menu item.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the price of the menu item.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets the cuisine category as a string (e.g., "Italian", "Indian").
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the preparation time in minutes for this specific item.
    /// </summary>
    public int PreparationTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets whether the menu item is currently available for ordering.
    /// </summary>
    public bool Available { get; set; }
}
