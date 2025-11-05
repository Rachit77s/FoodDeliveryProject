using FoodDeliveryPolaris.DTOs;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service interface for food recommendation operations.
/// Provides methods to search and recommend restaurants based on user preferences.
/// </summary>
public interface IFoodRecommendationService
{
    /// <summary>
    /// Gets food recommendations based on user preferences and constraints.
    /// Filters restaurants by cuisine type, time constraint, and location.
    /// Returns sorted list of recommendations with matching menu items.
    /// </summary>
    /// <param name="request">The search criteria including cuisine type, max time, and user location.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>List of recommended restaurants with matching menu items, sorted by best match.</returns>
    Task<List<FoodRecommendationResponse>> GetRecommendationsAsync(
        FoodRecommendationRequest request,
        CancellationToken cancellationToken = default);
}
