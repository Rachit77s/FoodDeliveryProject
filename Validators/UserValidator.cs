using FoodDeliveryPolaris.Models;
using System.Text.RegularExpressions;

namespace FoodDeliveryPolaris.Validators;

/// <summary>
/// Validator for User entity registration and updates.
/// Ensures all required fields are present and valid.
/// </summary>
public static class UserValidator
{
    /// <summary>
    /// Validates a user for registration or update.
    /// </summary>
    /// <param name="user">The user to validate.</param>
    /// <returns>Dictionary of validation errors. Empty if validation passes.</returns>
    public static Dictionary<string, string[]> Validate(User user)
    {
        var errors = new Dictionary<string, string[]>();

        if (user == null)
        {
            errors.Add("User", new[] { "User cannot be null." });
            return errors;
        }

        ValidationHelpers.ValidateName(user.Name, errors, nameof(user.Name));
        ValidationHelpers.ValidateEmail(user.Email, errors, nameof(user.Email));
        ValidationHelpers.ValidatePhone(user.Phone, errors, nameof(user.Phone));
        AddressValidator.Validate(user.Address, errors, nameof(user.Address));

        return errors;
    }
}
