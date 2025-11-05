using FoodDeliveryPolaris.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// Data Transfer Object for food recommendation search request.
/// </summary>
public class FoodRecommendationRequest
{
    /// <summary>
    /// Gets or sets the cuisine type the user wants.
    /// Examples: NorthIndian (0), Biryani (1), Mughlai (2), etc.
    /// </summary>
    [Required(ErrorMessage = "Cuisine type is required")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CuisineType CuisineType { get; set; }

    /// <summary>
    /// Gets or sets the user's location for distance calculation.
    /// This is populated from the user's registered address - not provided by API caller.
    /// </summary>
    [Required(ErrorMessage = "User location is required")]
    public Location UserLocation { get; set; }

    /// <summary>
    /// Gets or sets the maximum time user is willing to wait (in minutes).
    /// Includes preparation + delivery time. Default: 60 minutes.
    /// </summary>
    [Range(10, 180, ErrorMessage = "Maximum time must be between 10 and 180 minutes")]
    public int MaxTimeMinutes { get; set; } = 60;
}
