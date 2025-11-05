using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Repositories;
using FoodDeliveryPolaris.Utils;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service implementation for food recommendation operations.
/// Handles restaurant filtering, distance calculation, time estimation, and ranking.
/// </summary>
public class FoodRecommendationService : IFoodRecommendationService
{
    private readonly IRestaurantRepository _restaurantRepository;

    public FoodRecommendationService(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    /// <summary>
    /// Gets food recommendations based on user preferences and constraints.
    /// Implements multi-stage filtering for performance optimization.
    /// </summary>
    public async Task<List<FoodRecommendationResponse>> GetRecommendationsAsync(
        FoodRecommendationRequest request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get all open restaurants
        var allRestaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        var openRestaurants = allRestaurants.Where(r => r.IsOpen).ToList();

        var recommendations = new List<FoodRecommendationResponse>();

        foreach (var restaurant in openRestaurants)
        {
            // Step 2: Quick radius check (FAST pre-filter)
            // This eliminates 80-90% of restaurants before expensive operations
            var distanceKm = LocationUtils.CalculateDistance(request.UserLocation, restaurant.Address.Location);
            
            if (distanceKm > restaurant.DeliveryRadiusKm)
            {
                continue; // Skip - outside delivery range
            }

            // Step 3: Filter menu items by cuisine type
            var matchingItems = restaurant.Menu
                .Where(m => m.Available && m.CuisineType == request.CuisineType)
                .ToList();

            if (!matchingItems.Any())
            {
                continue; // Skip - no matching cuisine items
            }

            // Step 4: Calculate total time (prep + delivery)
            var deliveryTimeMinutes = DeliveryCalculator.EstimateDeliveryTimeMinutes(distanceKm);
            var fastestPrepTime = matchingItems.Min(m => m.PreparationTimeMinutes);
            var totalTime = fastestPrepTime + deliveryTimeMinutes;

            // Step 5: Time constraint filter
            if (totalTime > request.MaxTimeMinutes)
            {
                continue; // Skip - cannot deliver within time constraint
            }

            // Step 6: Build recommendation response
            recommendations.Add(new FoodRecommendationResponse
            {
                RestaurantId = restaurant.Id,
                RestaurantName = restaurant.Name,
                Rating = restaurant.Rating,
                DistanceKm = Math.Round(distanceKm, 2),
                TotalEstimatedTimeMinutes = totalTime,
                DeliveryAddress = $"{restaurant.Address.Street}, {restaurant.Address.City}",
                RecommendedItems = matchingItems.Select(m => new RecommendedMenuItem
                {
                    MenuItemId = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    Category = m.CuisineType.ToString(),
                    PreparationTimeMinutes = m.PreparationTimeMinutes,
                    Available = m.Available
                }).ToList()
            });
        }

        // Step 7: Sort recommendations by best match
        return recommendations
            .OrderBy(r => r.TotalEstimatedTimeMinutes)  // Fastest first (primary concern)
            .ThenByDescending(r => r.Rating)            // Then by highest rating
            .ThenBy(r => r.DistanceKm)                  // Then by nearest (tie-breaker)
            .ToList();
    }
}
