using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Validators;

/// <summary>
/// Validator for Restaurant entity registration and updates.
/// Ensures all required fields are present and valid.
/// </summary>
public static class RestaurantValidator
{
    /// <summary>
    /// Validates a restaurant for registration or update.
    /// </summary>
    /// <param name="restaurant">The restaurant to validate.</param>
    /// <returns>Dictionary of validation errors. Empty if validation passes.</returns>
    public static Dictionary<string, string[]> Validate(Restaurant restaurant)
    {
        var errors = new Dictionary<string, string[]>();

        if (restaurant == null)
        {
            errors.Add("Restaurant", new[] { "Restaurant cannot be null." });
            return errors;
        }

        ValidationHelpers.ValidateRestaurantName(restaurant.Name, errors, nameof(restaurant.Name));
        ValidationHelpers.ValidatePhone(restaurant.Phone, errors, nameof(restaurant.Phone));
        ValidateDeliveryRadius(restaurant, errors);
        ValidatePreparationTime(restaurant, errors);
        ValidateRating(restaurant, errors);
        AddressValidator.Validate(restaurant.Address, errors, nameof(restaurant.Address));
        ValidateMenu(restaurant, errors);

        return errors;
    }

    private static void ValidateDeliveryRadius(Restaurant restaurant, Dictionary<string, string[]> errors)
    {
        if (restaurant.DeliveryRadiusKm <= 0)
        {
            errors.Add(nameof(restaurant.DeliveryRadiusKm), new[] { "Delivery radius must be greater than 0." });
        }
        else if (restaurant.DeliveryRadiusKm > 50)
        {
            errors.Add(nameof(restaurant.DeliveryRadiusKm), new[] { "Delivery radius cannot exceed 50 km." });
        }
    }

    private static void ValidatePreparationTime(Restaurant restaurant, Dictionary<string, string[]> errors)
    {
        ValidationHelpers.ValidateRange(
            restaurant.AveragePreparationTimeMinutes, 5, 120,
            nameof(restaurant.AveragePreparationTimeMinutes),
            errors);
    }

    private static void ValidateRating(Restaurant restaurant, Dictionary<string, string[]> errors)
    {
        ValidationHelpers.ValidateRange(
            restaurant.Rating, 0, 5,
            nameof(restaurant.Rating),
            errors);
    }

    private static void ValidateMenu(Restaurant restaurant, Dictionary<string, string[]> errors)
    {
        if (restaurant.Menu == null || !restaurant.Menu.Any())
        {
            return; // Menu can be empty initially
        }

        for (int i = 0; i < restaurant.Menu.Count; i++)
        {
            ValidateMenuItem(restaurant.Menu[i], i, errors);
        }
    }

    private static void ValidateMenuItem(MenuItem menuItem, int index, Dictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(menuItem.Name))
        {
            errors.Add($"Menu[{index}].Name", new[] { "Menu item name is required." });
        }

        if (menuItem.Price < 0)
        {
            errors.Add($"Menu[{index}].Price", new[] { "Menu item price cannot be negative." });
        }

        ValidationHelpers.ValidateRange(
            menuItem.PreparationTimeMinutes, 5, 180,
            $"Menu[{index}].PreparationTimeMinutes",
            errors);
    }
}
