using System.Text.RegularExpressions;

namespace FoodDeliveryPolaris.Validators;

/// <summary>
/// Common validation utilities shared across all validators.
/// </summary>
public static class ValidationHelpers
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^\+?[\d\s\-\(\)]+$",
        RegexOptions.Compiled);

    /// <summary>
    /// Validates a name field (used by User, Restaurant, Rider).
    /// </summary>
    public static void ValidateName(string? name, Dictionary<string, string[]> errors, string fieldName = "Name")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(fieldName, new[] { "Name is required and cannot be empty." });
        }
        else if (name.Length < 2)
        {
            errors.Add(fieldName, new[] { "Name must be at least 2 characters long." });
        }
        else if (name.Length > 100)
        {
            errors.Add(fieldName, new[] { "Name cannot exceed 100 characters." });
        }
    }

    /// <summary>
    /// Validates a restaurant name (longer max length).
    /// </summary>
    public static void ValidateRestaurantName(string? name, Dictionary<string, string[]> errors, string fieldName = "Name")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(fieldName, new[] { "Name is required and cannot be empty." });
        }
        else if (name.Length < 2)
        {
            errors.Add(fieldName, new[] { "Name must be at least 2 characters long." });
        }
        else if (name.Length > 200)
        {
            errors.Add(fieldName, new[] { "Name cannot exceed 200 characters." });
        }
    }

    /// <summary>
    /// Validates an email field (optional but must be valid if provided).
    /// </summary>
    public static void ValidateEmail(string? email, Dictionary<string, string[]> errors, string fieldName = "Email")
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return; // Email is optional
        }

        if (email.Length > 256)
        {
            errors.Add(fieldName, new[] { "Email cannot exceed 256 characters." });
        }
        else if (!EmailRegex.IsMatch(email))
        {
            errors.Add(fieldName, new[] { "Email format is invalid." });
        }
    }

    /// <summary>
    /// Validates a phone field (optional but must be valid if provided).
    /// </summary>
    public static void ValidatePhone(string? phone, Dictionary<string, string[]> errors, string fieldName = "Phone")
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return; // Phone is optional
        }

        if (phone.Length > 20)
        {
            errors.Add(fieldName, new[] { "Phone cannot exceed 20 characters." });
        }
        else if (!PhoneRegex.IsMatch(phone))
        {
            errors.Add(fieldName, new[] { "Phone format is invalid." });
        }
    }

    /// <summary>
    /// Validates a numeric range.
    /// </summary>
    public static void ValidateRange(double value, double min, double max, string fieldName, Dictionary<string, string[]> errors)
    {
        if (value < min || value > max)
        {
            errors.Add(fieldName, new[] { $"{fieldName} must be between {min} and {max}. Got: {value}" });
        }
    }

    /// <summary>
    /// Validates a string length.
    /// </summary>
    public static void ValidateStringLength(string? value, int maxLength, string fieldName, Dictionary<string, string[]> errors, bool required = false)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
            {
                errors.Add(fieldName, new[] { $"{fieldName} is required." });
            }
            return;
        }

        if (value.Length > maxLength)
        {
            errors.Add(fieldName, new[] { $"{fieldName} cannot exceed {maxLength} characters." });
        }
    }
}
