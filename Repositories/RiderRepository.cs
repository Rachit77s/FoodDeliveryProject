using FoodDeliveryPolaris.Data;
using FoodDeliveryPolaris.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryPolaris.Repositories;

/// <summary>
/// Entity Framework Core implementation of IRiderRepository.
/// Provides database-backed storage for Rider entities with async operations.
/// </summary>
public class RiderRepository : IRiderRepository
{
    private readonly FoodDeliveryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the RiderRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RiderRepository(FoodDeliveryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Rider> CreateAsync(Rider rider, CancellationToken cancellationToken = default)
    {
        await _context.Riders.AddAsync(rider, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return rider;
    }

    /// <inheritdoc/>
    public async Task<Rider?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Riders
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Rider>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Riders
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Rider> UpdateAsync(Rider rider, CancellationToken cancellationToken = default)
    {
        _context.Riders.Update(rider);
        await _context.SaveChangesAsync(cancellationToken);
        return rider;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var rider = await _context.Riders.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (rider == null)
        {
            return false;
        }

        _context.Riders.Remove(rider);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Riders.AsNoTracking();
        
        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        return await query.AnyAsync(r => r.Name == name, cancellationToken);
    }
}
