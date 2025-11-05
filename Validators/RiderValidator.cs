using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Validators;

/// <summary>
/// Validator for Rider entity registration and updates.
/// Ensures all required fields are present and valid.
/// </summary>
public static class RiderValidator
{
    /// <summary>
    /// Validates a rider for registration or update.
    /// </summary>
    /// <param name="rider">The rider to validate.</param>
    /// <returns>Dictionary of validation errors. Empty if validation passes.</returns>
    public static Dictionary<string, string[]> Validate(Rider rider)
    {
        var errors = new Dictionary<string, string[]>();

        if (rider == null)
        {
            errors.Add("Rider", new[] { "Rider cannot be null." });
            return errors;
        }

        ValidationHelpers.ValidateName(rider.Name, errors, nameof(rider.Name));
        ValidationHelpers.ValidateEmail(rider.Email, errors, nameof(rider.Email));
        ValidationHelpers.ValidatePhone(rider.Phone, errors, nameof(rider.Phone));
        ValidationHelpers.ValidateStringLength(rider.VehicleNumber, 50, nameof(rider.VehicleNumber), errors);
        ValidateCurrentLocation(rider, errors);

        return errors;
    }

    private static void ValidateCurrentLocation(Rider rider, Dictionary<string, string[]> errors)
    {
        if (rider.CurrentLocation == null)
        {
            return; // Optional field
        }

        var location = rider.CurrentLocation;

        ValidationHelpers.ValidateRange(
            location.Lat, -90, 90,
            $"{nameof(rider.CurrentLocation)}.{nameof(location.Lat)}",
            errors);

        ValidationHelpers.ValidateRange(
            location.Lon, -180, 180,
            $"{nameof(rider.CurrentLocation)}.{nameof(location.Lon)}",
            errors);
    }
}
