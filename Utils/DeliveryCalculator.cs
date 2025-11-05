namespace FoodDeliveryPolaris.Utils;

/// <summary>
/// Utility class for delivery-related calculations.
/// Provides methods for estimating delivery times based on distance.
/// </summary>
public static class DeliveryCalculator
{
    private const double AverageDeliverySpeedKmPerHour = 20.0; // Bike speed in city traffic
    private const int DeliveryBufferMinutes = 5; // Buffer for traffic, parking, pickup time

    /// <summary>
    /// Estimates delivery time based on distance.
    /// Includes base travel time plus buffer for real-world conditions.
    /// </summary>
    /// <param name="distanceKm">Distance in kilometers</param>
    /// <returns>Estimated delivery time in minutes</returns>
    /// <remarks>
    /// Formula: (distance / 20 km/h) * 60 + 5 min buffer
    /// 
    /// Examples:
    /// - 1 km ? 3 min travel + 5 min buffer = 8 min
    /// - 2 km ? 6 min travel + 5 min buffer = 11 min
    /// - 5 km ? 15 min travel + 5 min buffer = 20 min
    /// - 10 km ? 30 min travel + 5 min buffer = 35 min
    /// 
    /// The buffer accounts for:
    /// - Traffic delays
    /// - Finding parking
    /// - Pickup time at restaurant
    /// - Delivery to door
    /// </remarks>
    public static int EstimateDeliveryTimeMinutes(double distanceKm)
    {
        var hours = distanceKm / AverageDeliverySpeedKmPerHour;
        var baseMinutes = hours * 60;
        var roundedMinutes = (int)Math.Ceiling(baseMinutes);

        return roundedMinutes + DeliveryBufferMinutes;
    }

    /// <summary>
    /// Estimates delivery time with custom speed and buffer.
    /// Useful for different delivery scenarios (e.g., car vs bike, rush hour).
    /// </summary>
    /// <param name="distanceKm">Distance in kilometers</param>
    /// <param name="speedKmPerHour">Average delivery speed</param>
    /// <param name="bufferMinutes">Additional buffer time</param>
    /// <returns>Estimated delivery time in minutes</returns>
    /// <remarks>
    /// This overload is provided for future extensibility when different vehicle types
    /// or traffic conditions need to be considered.
    /// 
    /// Example use cases:
    /// - Car delivery: EstimateDeliveryTimeMinutes(5.0, speedKmPerHour: 40.0, bufferMinutes: 10)
    /// - Rush hour: EstimateDeliveryTimeMinutes(5.0, speedKmPerHour: 15.0, bufferMinutes: 10)
    /// </remarks>
    public static int EstimateDeliveryTimeMinutes(
        double distanceKm, 
        double speedKmPerHour, 
        int bufferMinutes)
    {
        var hours = distanceKm / speedKmPerHour;
        var baseMinutes = hours * 60;
        var roundedMinutes = (int)Math.Ceiling(baseMinutes);

        return roundedMinutes + bufferMinutes;
    }
}
