namespace FoodDeliveryPolaris.Models;

/// <summary>
/// Represents an individual item within an order.
/// Captures the menu item details, quantity, and price at the time of order.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the unique identifier for this order item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the menu item ID that was ordered.
    /// References the MenuItem entity.
    /// </summary>
    public int MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the menu item (snapshot at order time).
    /// Stored to preserve what was ordered even if menu item name changes later.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the price of the menu item (snapshot at order time).
    /// Stored to preserve the price paid even if menu item price changes later.
    /// Prevents price manipulation issues.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this item ordered.
    /// </summary>
    public int Quantity { get; set; }
}
