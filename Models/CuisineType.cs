namespace FoodDeliveryPolaris.Models;

/// <summary>
/// Represents different types of cuisines available in the food delivery system.
/// Used for categorizing menu items and filtering recommendations.
/// Ordered by popularity in India.
/// </summary>
public enum CuisineType
{
    /// <summary>
    /// North Indian cuisine - Butter Chicken, Dal Makhani, Naan, Paneer dishes, etc.
    /// </summary>
    NorthIndian = 0,

    /// <summary>
    /// Biryani - Hyderabadi Biryani, Lucknowi Biryani, Chicken/Mutton Biryani, etc.
    /// </summary>
    Biryani = 1,

    /// <summary>
    /// Mughlai cuisine - Kebabs, Korma, Nihari, Haleem, etc.
    /// </summary>
    Mughlai = 2,

    /// <summary>
    /// South Indian cuisine - Dosa, Idli, Vada, Sambhar, Uttapam, etc.
    /// </summary>
    SouthIndian = 3,

    /// <summary>
    /// Chinese cuisine - Noodles, Fried Rice, Manchurian, Momos, etc.
    /// </summary>
    Chinese = 4,

    /// <summary>
    /// Pizza - Margherita, Pepperoni, Veggie Pizza, etc.
    /// </summary>
    Pizza = 5,

    /// <summary>
    /// Burgers - Veg Burger, Chicken Burger, Cheese Burger, etc.
    /// </summary>
    Burger = 6,

    /// <summary>
    /// Rolls - Kathi Rolls, Shawarma Rolls, Paneer Rolls, etc.
    /// </summary>
    Rolls = 7
}
