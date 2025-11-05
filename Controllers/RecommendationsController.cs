using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Services;
using FoodDeliveryPolaris.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryPolaris.Controllers;

/// <summary>
/// API controller for food recommendation operations.
/// Provides endpoint to get personalized restaurant recommendations based on user preferences.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IFoodRecommendationService _recommendationService;
    private readonly IUserService _userService;
    private readonly ILogger<RecommendationsController> _logger;

    public RecommendationsController(
        IFoodRecommendationService recommendationService,
        IUserService userService,
        ILogger<RecommendationsController> logger)
    {
        _recommendationService = recommendationService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get restaurant recommendations for a user based on cuisine type.
    /// System automatically uses the user's registered address for location.
    /// </summary>
    /// <param name="userId">Your user ID (from login/registration)</param>
    /// <param name="cuisineType">Type of cuisine (e.g., "NorthIndian", "Biryani", "Mughlai")</param>
    /// <param name="maxTimeMinutes">Maximum delivery time in minutes (optional, default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of recommended restaurants sorted by relevance</returns>
    /// <response code="200">Returns list of restaurant recommendations</response>
    /// <response code="400">If parameters are invalid</response>
    /// <response code="404">If user not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FoodRecommendationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FoodRecommendationResponse>>> GetRecommendations(
        [FromQuery] int userId,
        [FromQuery] string cuisineType,
        [FromQuery] int? maxTimeMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate time
            if (maxTimeMinutes.HasValue && maxTimeMinutes.Value <= 0)
            {
                return BadRequest(new { error = "Max time must be greater than 0 minutes" });
            }

            // Parse cuisine type string to enum
            if (!Enum.TryParse<Models.CuisineType>(cuisineType, true, out var parsedCuisineType))
            {
                return BadRequest(new
                {
                    error = "Invalid Cuisine Type",
                    message = $"Invalid cuisine type: '{cuisineType}'. Valid values are: NorthIndian, SouthIndian, Mughlai, Chinese, Italian, Biryani, Pizza, Burger, FastFood"
                });
            }

            _logger.LogInformation(
                "Getting recommendations for User={UserId}, Cuisine={Cuisine}, MaxTime={MaxTime}min",
                userId, parsedCuisineType, maxTimeMinutes ?? 60);

            // Get user to retrieve their address/location
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return NotFound(new 
                { 
                    error = "User not found",
                    message = $"User with ID {userId} does not exist. Please check your user ID or register first."
                });
            }

            // Validate user has an address
            if (user.Address == null || user.Address.Location == null)
            {
                _logger.LogWarning("User {UserId} has no address/location configured", userId);
                return BadRequest(new 
                { 
                    error = "User location not configured",
                    message = "Please update your profile with a delivery address before searching for restaurants."
                });
            }

            // Create request object using user's location
            var request = new FoodRecommendationRequest
            {
                CuisineType = parsedCuisineType,
                UserLocation = user.Address.Location,
                MaxTimeMinutes = maxTimeMinutes ?? 60
            };

            var recommendations = await _recommendationService
                .GetRecommendationsAsync(request, cancellationToken);

            if (!recommendations.Any())
            {
                _logger.LogInformation(
                    "No recommendations found for User={UserId}, Cuisine={Cuisine} within {MaxTime} minutes",
                    userId, parsedCuisineType, maxTimeMinutes ?? 60);

                return Ok(new
                {
                    message = $"No restaurants found serving {parsedCuisineType} within {maxTimeMinutes ?? 60} minutes of your location",
                    userLocation = $"{user.Address.City}, {user.Address.ZipCode}",
                    suggestions = new[]
                    {
                        "Try increasing the delivery time (e.g., 90 or 120 minutes)",
                        "Try a different cuisine type",
                        "Check if there are restaurants in your area",
                        "Make sure your delivery address is correct"
                    },
                    recommendations = new List<FoodRecommendationResponse>()
                });
            }

            _logger.LogInformation(
                "Found {Count} recommendations for User={UserId}, Cuisine={Cuisine}",
                recommendations.Count, userId, parsedCuisineType);

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for User={UserId}", userId);
            return StatusCode(500, new
            {
                error = "An error occurred while getting recommendations",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get list of available cuisine types.
    /// Useful for showing options in UI.
    /// </summary>
    /// <returns>List of cuisine types with their values</returns>
    [HttpGet("cuisines")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    public ActionResult<Dictionary<string, int>> GetAvailableCuisines()
    {
        var cuisines = Enum.GetValues<Models.CuisineType>()
            .ToDictionary(
                c => c.ToString(),
                c => (int)c
            );

        return Ok(cuisines);
    }
}
