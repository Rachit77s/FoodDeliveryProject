using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Validators;

/// <summary>
/// Shared validator for Address validation across all entities.
/// Ensures address fields are present and valid.
/// </summary>
public static class AddressValidator
{
    /// <summary>
    /// Validates an address with all its components.
    /// </summary>
    /// <param name="address">The address to validate.</param>
    /// <param name="errors">Dictionary to collect validation errors.</param>
    /// <param name="fieldPrefix">Prefix for error keys (e.g., "Address" or "User.Address").</param>
    public static void Validate(Address? address, Dictionary<string, string[]> errors, string fieldPrefix = "Address")
    {
        if (address == null)
        {
            errors.Add(fieldPrefix, new[] { "Address is required." });
            return;
        }

        ValidateStreet(address, errors, fieldPrefix);
        ValidateCity(address, errors, fieldPrefix);
        ValidateZipCode(address, errors, fieldPrefix);
        ValidateLocation(address, errors, fieldPrefix);
    }

    private static void ValidateStreet(Address address, Dictionary<string, string[]> errors, string fieldPrefix)
    {
        if (string.IsNullOrWhiteSpace(address.Street))
        {
            errors.Add($"{fieldPrefix}.Street", new[] { "Street address is required." });
        }
        else if (address.Street.Length > 200)
        {
            errors.Add($"{fieldPrefix}.Street", new[] { "Street address cannot exceed 200 characters." });
        }
    }

    private static void ValidateCity(Address address, Dictionary<string, string[]> errors, string fieldPrefix)
    {
        if (string.IsNullOrWhiteSpace(address.City))
        {
            errors.Add($"{fieldPrefix}.City", new[] { "City is required." });
        }
        else if (address.City.Length > 100)
        {
            errors.Add($"{fieldPrefix}.City", new[] { "City cannot exceed 100 characters." });
        }
    }

    private static void ValidateZipCode(Address address, Dictionary<string, string[]> errors, string fieldPrefix)
    {
        if (string.IsNullOrWhiteSpace(address.ZipCode))
        {
            errors.Add($"{fieldPrefix}.ZipCode", new[] { "ZIP code is required." });
        }
        else if (address.ZipCode.Length > 20)
        {
            errors.Add($"{fieldPrefix}.ZipCode", new[] { "ZIP code cannot exceed 20 characters." });
        }
    }

    private static void ValidateLocation(Address address, Dictionary<string, string[]> errors, string fieldPrefix)
    {
        if (address.Location == null)
        {
            errors.Add($"{fieldPrefix}.Location", new[] { "Location coordinates are required." });
            return;
        }

        if (address.Location.Lat < -90 || address.Location.Lat > 90)
        {
            errors.Add($"{fieldPrefix}.Location.Lat",
                new[] { $"Latitude must be between -90 and 90. Got: {address.Location.Lat}" });
        }

        if (address.Location.Lon < -180 || address.Location.Lon > 180)
        {
            errors.Add($"{fieldPrefix}.Location.Lon",
                new[] { $"Longitude must be between -180 and 180. Got: {address.Location.Lon}" });
        }
    }
}
