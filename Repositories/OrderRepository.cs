using FoodDeliveryPolaris.Data;
using FoodDeliveryPolaris.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryPolaris.Repositories;

/// <summary>
/// Entity Framework Core implementation of IOrderRepository.
/// Provides database-backed storage for Order entities with async operations.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly FoodDeliveryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the OrderRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public OrderRepository(FoodDeliveryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return order;
    }

    /// <inheritdoc/>
    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(
        int userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetOrdersByRiderIdAsync(
        int riderId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        bool includeInProgress = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.RiderId == riderId);

        // Filter by status if only completed orders are needed
        if (!includeInProgress)
        {
            query = query.Where(o => 
                o.Status == OrderStatus.Delivered || 
                o.Status == OrderStatus.Cancelled);
        }

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> GetUserOrderCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .CountAsync(o => o.UserId == userId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> GetRiderOrderCountAsync(
        int riderId, 
        bool includeInProgress = true, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.RiderId == riderId);

        if (!includeInProgress)
        {
            query = query.Where(o => 
                o.Status == OrderStatus.Delivered || 
                o.Status == OrderStatus.Cancelled);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.UpdatedAt = DateTime.UtcNow;

        // Set CompletedAt if status changed to Delivered or Cancelled
        if ((order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled) 
            && !order.CompletedAt.HasValue)
        {
            order.CompletedAt = DateTime.UtcNow;
        }

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
