using FoodDeliveryPolaris.Data;
using FoodDeliveryPolaris.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryPolaris.Repositories;

/// <summary>
/// Entity Framework Core implementation of IRestaurantRepository.
/// Provides database-backed storage for Restaurant entities with async operations.
/// </summary>
public class RestaurantRepository : IRestaurantRepository
{
    private readonly FoodDeliveryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the RestaurantRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RestaurantRepository(FoodDeliveryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Restaurant> CreateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        await _context.Restaurants.AddAsync(restaurant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return restaurant;
    }

    /// <inheritdoc/>
    public async Task<Restaurant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Restaurants
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Restaurant> UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
    {
        _context.Restaurants.Update(restaurant);
        await _context.SaveChangesAsync(cancellationToken);
        return restaurant;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (restaurant == null)
        {
            return false;
        }

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Restaurants.AsNoTracking();
        
        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        return await query.AnyAsync(r => r.Name == name, cancellationToken);
    }
}
