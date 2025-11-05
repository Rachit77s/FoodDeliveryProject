using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.DTOs;

/// <summary>
/// Data Transfer Object for creating a new user.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delivery address of the user.
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Converts this DTO to a User model.
    /// </summary>
    public User ToUser()
    {
        return new User
        {
            Name = Name,
            Email = Email,
            Phone = Phone,
            Address = Address
        };
    }
}

/// <summary>
/// Data Transfer Object for updating an existing user.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the delivery address of the user.
    /// </summary>
    public Address Address { get; set; }

    /// <summary>
    /// Converts this DTO to a User model with the specified ID.
    /// </summary>
    public User ToUser(int id)
    {
        return new User
        {
            Id = id,
            Name = Name,
            Email = Email,
            Phone = Phone,
            Address = Address
        };
    }
}
