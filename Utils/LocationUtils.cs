using FoodDeliveryPolaris.Models;

namespace FoodDeliveryPolaris.Utils;

/// <summary>
/// Utility class for geographical location calculations.
/// Provides distance calculation using Haversine formula and related geo operations.
/// </summary>
public static class LocationUtils
{
    /// <summary>
    /// Calculates the distance between two geographical locations using the Haversine formula.
    /// This formula accounts for the Earth's curvature and provides accurate distance calculation.
    /// </summary>
    /// <param name="from">Starting location with latitude and longitude</param>
    /// <param name="to">Destination location with latitude and longitude</param>
    /// <returns>Distance in kilometers</returns>
    /// <remarks>
    /// The Haversine formula calculates the great-circle distance between two points 
    /// on a sphere given their longitudes and latitudes.
    /// Accuracy: Less than 0.5% error for typical distances.
    /// 
    /// Examples:
    /// - Delhi to Mumbai: ~1140 km
    /// - 2 km local distance: ~2.01 km (very accurate)
    /// - 10 km local distance: ~10.05 km (very accurate)
    /// </remarks>
    public static double CalculateDistance(Location from, Location to)
    {
        const double EarthRadiusKm = 6371;

        var dLat = ToRadians(to.Lat - from.Lat);
        var dLon = ToRadians(to.Lon - from.Lon);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(from.Lat)) * Math.Cos(ToRadians(to.Lat)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Converts degrees to radians for trigonometric calculations.
    /// </summary>
    /// <param name="degrees">Angle in degrees</param>
    /// <returns>Angle in radians</returns>
    public static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    /// <param name="radians">Angle in radians</param>
    /// <returns>Angle in degrees</returns>
    public static double ToDegrees(double radians)
    {
        return radians * 180 / Math.PI;
    }

    /// <summary>
    /// Checks if a location is within a specified radius of another location.
    /// Useful for delivery zone validation and proximity checks.
    /// </summary>
    /// <param name="center">Center location (e.g., restaurant location)</param>
    /// <param name="target">Target location to check (e.g., user location)</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    /// <returns>True if target is within radius, false otherwise</returns>
    /// <example>
    /// var restaurant = new Location { Lat = 28.6139, Lon = 77.2090 };
    /// var user = new Location { Lat = 28.6200, Lon = 77.2150 };
    /// bool canDeliver = LocationUtils.IsWithinRadius(restaurant, user, 5.0);
    /// </example>
    public static bool IsWithinRadius(Location center, Location target, double radiusKm)
    {
        var distance = CalculateDistance(center, target);
        return distance <= radiusKm;
    }

    /// <summary>
    /// Gets the approximate bounding box (min/max lat/lon) for a given center and radius.
    /// Useful for database queries to pre-filter locations before precise distance calculation.
    /// </summary>
    /// <param name="center">Center location</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    /// <returns>Tuple with (minLat, maxLat, minLon, maxLon)</returns>
    /// <remarks>
    /// This provides a rough bounding box. Items within this box should be further 
    /// validated with CalculateDistance for accurate filtering.
    /// </remarks>
    public static (double MinLat, double MaxLat, double MinLon, double MaxLon) GetBoundingBox(
        Location center, 
        double radiusKm)
    {
        const double EarthRadiusKm = 6371;
        
        // Approximate degrees per km at this latitude
        var latDegreePerKm = 1.0 / 110.574;
        var lonDegreePerKm = 1.0 / (111.320 * Math.Cos(ToRadians(center.Lat)));

        var latOffset = radiusKm * latDegreePerKm;
        var lonOffset = radiusKm * lonDegreePerKm;

        return (
            MinLat: center.Lat - latOffset,
            MaxLat: center.Lat + latOffset,
            MinLon: center.Lon - lonOffset,
            MaxLon: center.Lon + lonOffset
        );
    }
}
