namespace FoodDeliveryPolaris.Models;

/// <summary>
/// Represents the current status of a rider.
/// </summary>
public enum RiderStatus
{
    /// <summary>
    /// Rider is available to accept new orders.
    /// </summary>
    Available,

    /// <summary>
    /// Rider is currently busy with an active delivery.
    /// </summary>
    Busy,

    /// <summary>
    /// Rider is offline and not accepting orders.
    /// </summary>
    Offline
}
