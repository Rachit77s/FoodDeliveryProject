using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Repositories;
using FoodDeliveryPolaris.Utils;
using FoodDeliveryPolaris.Exceptions;
using Microsoft.Extensions.Logging;

namespace FoodDeliveryPolaris.Services;

/// <summary>
/// Service implementation for Order management and history.
/// Implements validation, business logic, order placement, and rider assignment.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IRestaurantRepository restaurantRepository,
        IRiderRepository riderRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _restaurantRepository = restaurantRepository ?? throw new ArgumentNullException(nameof(restaurantRepository));
        _riderRepository = riderRepository ?? throw new ArgumentNullException(nameof(riderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates user exists and has a registered address.
    /// Returns user and delivery address location.
    /// </summary>
    private async Task<(User user, Location deliveryAddress)> ValidateUserAndGetAddressAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        if (user.Address == null || user.Address.Location == null)
        {
            throw new InvalidOperationException(
                "User does not have a registered delivery address. Please update your profile with a delivery address first.");
        }

        return (user, user.Address.Location);
    }

    /// <summary>
    /// Validates restaurant exists, is open, and within delivery radius.
    /// Returns restaurant and calculated distance.
    /// </summary>
    private async Task<(Restaurant restaurant, double distance)> ValidateRestaurantAndDistanceAsync(
        int restaurantId,
        Location deliveryAddress,
        CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant == null)
        {
            throw new NotFoundException($"Restaurant with ID {restaurantId} not found");
        }

        if (!restaurant.IsOpen)
        {
            throw new InvalidOperationException($"Restaurant '{restaurant.Name}' is currently closed");
        }

        var distance = LocationUtils.CalculateDistance(
            restaurant.Address.Location,
            deliveryAddress);

        if (distance > restaurant.DeliveryRadiusKm)
        {
            throw new InvalidOperationException(
                $"Your delivery address is outside restaurant's delivery radius. " +
                $"Distance: {distance:F2} km, Max radius: {restaurant.DeliveryRadiusKm:F2} km");
        }

        return (restaurant, distance);
    }

    /// <summary>
    /// Validates menu items and calculates order items with subtotal.
    /// Returns list of order item responses and subtotal.
    /// </summary>
    private (List<OrderItemResponse> orderItems, double subtotal) ValidateAndCalculateOrderItems(
        Restaurant restaurant,
        List<OrderItemRequest> itemRequests)
    {
        var orderItems = new List<OrderItemResponse>();
        double subtotal = 0;

        foreach (var itemRequest in itemRequests)
        {
            var menuItem = restaurant.Menu.FirstOrDefault(m => m.Id == itemRequest.MenuItemId);

            if (menuItem == null)
            {
                throw new NotFoundException(
                    $"Menu item with ID {itemRequest.MenuItemId} not found in restaurant menu");
            }

            if (!menuItem.Available)
            {
                throw new InvalidOperationException($"Menu item '{menuItem.Name}' is not available");
            }

            var itemTotal = menuItem.Price * itemRequest.Quantity;
            subtotal += itemTotal;

            orderItems.Add(new OrderItemResponse
            {
                MenuItemId = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                Quantity = itemRequest.Quantity,
                ItemTotal = itemTotal
            });
        }

        return (orderItems, subtotal);
    }

    /// <inheritdoc/>
    public async Task<OrderPreviewResponse> PreviewOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Previewing order for user {UserId} at restaurant {RestaurantId}",
            request.UserId, request.RestaurantId);

        // Step 1: Validate user and get address (reusable method)
        var (user, deliveryAddress) = await ValidateUserAndGetAddressAsync(
            request.UserId, cancellationToken);

        // Step 2: Validate restaurant and distance (reusable method)
        var (restaurant, distance) = await ValidateRestaurantAndDistanceAsync(
            request.RestaurantId, deliveryAddress, cancellationToken);

        // Step 3: Validate menu items and calculate subtotal (reusable method)
        var (orderItems, subtotal) = ValidateAndCalculateOrderItems(
            restaurant, request.Items);

        // Step 4: Calculate totals (simplified)
        var total = subtotal;

        // Step 5: Calculate estimated delivery time
        var estimatedDeliveryTimeMinutes = DeliveryCalculator.EstimateDeliveryTimeMinutes(distance) +
                                           restaurant.AveragePreparationTimeMinutes;

        // Step 6: Format delivery address
        string deliveryAddressText = $"{user.Address.Street}, {user.Address.City}, {user.Address.ZipCode}";

        _logger.LogInformation(
            "Order preview calculated. Total: {Total:C}, Distance: {Distance:F2} km",
            total, distance);

        return new OrderPreviewResponse
        {
            RestaurantName = restaurant.Name,
            UserName = user.Name,
            Items = orderItems,
            Total = Math.Round(total, 2),
            DistanceKm = Math.Round(distance, 2),
            EstimatedDeliveryTimeMinutes = estimatedDeliveryTimeMinutes,
            DeliveryAddress = deliveryAddressText
        };
    }

    /// <inheritdoc/>
    public async Task<OrderResponse> PlaceOrderAsync(
        PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Placing order for user {UserId} at restaurant {RestaurantId}",
            request.UserId, request.RestaurantId);

        // Step 1: Validate user and get address (reusable method)
        var (user, deliveryAddress) = await ValidateUserAndGetAddressAsync(
            request.UserId, cancellationToken);

        // Step 2: Validate restaurant and distance (reusable method)
        var (restaurant, distance) = await ValidateRestaurantAndDistanceAsync(
            request.RestaurantId, deliveryAddress, cancellationToken);

        // Step 3: Validate menu items and calculate subtotal (reusable method)
        var (orderItemResponses, subtotal) = ValidateAndCalculateOrderItems(
            restaurant, request.Items);

        // Step 4: Convert to OrderItem entities for database
        var orderItems = orderItemResponses.Select(item => new OrderItem
        {
            MenuItemId = item.MenuItemId,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity
        }).ToList();

        // Step 5: Calculate totals (simplified)
        var total = subtotal;

        // Step 6: Create order
        var order = new Order
        {
            UserId = request.UserId,
            RestaurantId = request.RestaurantId,
            Items = orderItems,
            Total = total,
            Status = OrderStatus.Placed,
            PaymentStatus = PaymentStatus.NotPaid,
            DeliveryAddress = deliveryAddress,
            Notes = request.Notes ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdOrder = await _orderRepository.CreateAsync(order, cancellationToken);

        _logger.LogInformation(
            "Order {OrderId} created successfully. Total: {Total:C}, Distance: {Distance:F2} km, Delivery to: {City}",
            createdOrder.Id, total, distance, user.Address.City);

        // Step 7: Build response
        return await BuildOrderResponse(createdOrder, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OrderResponse> AcceptOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Restaurant accepting order {OrderId}", orderId);

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"Order with ID {orderId} not found");
        }

        if (order.Status != OrderStatus.Placed)
        {
            throw new InvalidOperationException(
                $"Cannot accept order with status '{order.Status}'. Only 'Placed' orders can be accepted.");
        }

        // Update order status
        order.Status = OrderStatus.Accepted;
        order.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _orderRepository.UpdateAsync(order, cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("shadow key property"))
        {
            _logger.LogError(ex, "Failed to update order {OrderId} due to EF Core tracking issue", orderId);
            throw new InvalidOperationException(
                "Failed to accept order due to a database tracking issue. Please try again.");
        }

        _logger.LogInformation("Order {OrderId} accepted. Attempting to assign nearest rider...", orderId);

        // Try to auto-assign nearest rider
        try
        {
            await AssignNearestRiderAsync(orderId, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Failed to auto-assign rider to order {OrderId}: {Message}",
                orderId, ex.Message);
            // Order is still accepted even if rider assignment fails
            // Can be assigned manually later
        }

        return await BuildOrderResponse(order, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<RiderAssignmentResponse> AssignNearestRiderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning nearest rider to order {OrderId}", orderId);

        // Get order
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
        {
            throw new NotFoundException($"Order with ID {orderId} not found");
        }

        if (order.Status != OrderStatus.Accepted && order.Status != OrderStatus.Preparing)
        {
            throw new InvalidOperationException(
                $"Cannot assign rider to order with status '{order.Status}'. " +
                $"Order must be 'Accepted' or 'Preparing'.");
        }

        // Get restaurant
        var restaurant = await _restaurantRepository.GetByIdAsync(order.RestaurantId, cancellationToken);
        if (restaurant == null)
        {
            throw new NotFoundException($"Restaurant with ID {order.RestaurantId} not found");
        }

        // Get all available riders
        var allRiders = await _riderRepository.GetAllAsync(cancellationToken);
        var availableRiders = allRiders
            .Where(r => r.RiderStatus == RiderStatus.Available)
            .ToList();

        if (!availableRiders.Any())
        {
            throw new InvalidOperationException("No riders available at the moment. Please try again later.");
        }

        // Calculate distances and find nearest
        var riderDistances = availableRiders
            .Select(rider => new
            {
                Rider = rider,
                Distance = LocationUtils.CalculateDistance(
                    rider.CurrentLocation,
                    restaurant.Address.Location)
            })
            .OrderBy(rd => rd.Distance)
            .ToList();

        var selectedRider = riderDistances.First();

        _logger.LogInformation(
            "Selected rider {RiderId} ({RiderName}) for order {OrderId}. Distance: {Distance:F2} km",
            selectedRider.Rider.Id, selectedRider.Rider.Name, orderId, selectedRider.Distance);

        // Assign rider to order
        order.RiderId = selectedRider.Rider.Id;
        order.Status = OrderStatus.Preparing;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        // Update rider status
        selectedRider.Rider.RiderStatus = RiderStatus.Busy;
        await _riderRepository.UpdateAsync(selectedRider.Rider, cancellationToken);

        // Calculate ETA
        var estimatedPickupTimeMinutes = DeliveryCalculator.EstimateDeliveryTimeMinutes(selectedRider.Distance);

        return new RiderAssignmentResponse
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            RiderId = selectedRider.Rider.Id,
            RiderName = selectedRider.Rider.Name,
            RiderPhone = selectedRider.Rider.Phone,
            VehicleNumber = selectedRider.Rider.VehicleNumber,
            DistanceToRestaurantKm = Math.Round(selectedRider.Distance, 2),
            EstimatedPickupTimeMinutes = estimatedPickupTimeMinutes,
            Message = $"Rider {selectedRider.Rider.Name} assigned successfully. ETA to restaurant: {estimatedPickupTimeMinutes} minutes"
        };
    }

    /// <inheritdoc/>
    public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving order with ID: {OrderId}", id);
        return await _orderRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<OrderResponse?> GetOrderByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving order details with ID: {OrderId}", id);
        
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
        {
            return null;
        }

        return await BuildOrderResponse(order, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetUserOrderHistoryAsync(
        int userId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving order history for user {UserId}, page {Page}, size {Size}",
            userId, pageNumber, pageSize);

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        var allOrders = await _orderRepository.GetAllAsync(cancellationToken);
        var userOrders = allOrders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = userOrders.Count();
        var orders = userOrders
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        _logger.LogInformation(
            "Found {Count} total orders for user {UserId}, returning page {Page}",
            totalCount, userId, pageNumber);

        return (orders, totalCount);
    }

    /// <inheritdoc/>
    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetRiderOrderHistoryAsync(
        int riderId,
        int pageNumber = 1,
        int pageSize = 20,
        bool includeInProgress = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving order history for rider {RiderId}, page {Page}, size {Size}, includeInProgress: {IncludeInProgress}",
            riderId, pageNumber, pageSize, includeInProgress);

        // Validate rider exists
        var rider = await _riderRepository.GetByIdAsync(riderId, cancellationToken);
        if (rider == null)
        {
            throw new NotFoundException($"Rider with ID {riderId} not found");
        }

        var allOrders = await _orderRepository.GetAllAsync(cancellationToken);
        var riderOrders = allOrders.Where(o => o.RiderId == riderId);

        if (!includeInProgress)
        {
            riderOrders = riderOrders.Where(o =>
                o.Status == OrderStatus.Delivered ||
                o.Status == OrderStatus.Cancelled);
        }

        var orderedOrders = riderOrders.OrderByDescending(o => o.CreatedAt);
        var totalCount = orderedOrders.Count();
        var orders = orderedOrders
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        _logger.LogInformation(
            "Found {Count} total orders for rider {RiderId}, returning page {Page}",
            totalCount, riderId, pageNumber);

        return (orders, totalCount);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all orders");
        return await _orderRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Order?> UpdateOrderAsync(int id, Order order, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating order {OrderId}", id);

        var existingOrder = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (existingOrder == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found for update", id);
            return null;
        }

        order.Id = id;
        order.UpdatedAt = DateTime.UtcNow;

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
        {
            order.CompletedAt = DateTime.UtcNow;

            // If order is delivered or cancelled, set rider back to available
            if (order.RiderId.HasValue)
            {
                var rider = await _riderRepository.GetByIdAsync(order.RiderId.Value, cancellationToken);
                if (rider != null && rider.RiderStatus == RiderStatus.Busy)
                {
                    rider.RiderStatus = RiderStatus.Available;
                    await _riderRepository.UpdateAsync(rider, cancellationToken);
                }
            }
        }

        var updatedOrder = await _orderRepository.UpdateAsync(order, cancellationToken);
        _logger.LogInformation("Order {OrderId} updated successfully", id);

        return updatedOrder;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting order {OrderId}", id);

        var deleted = await _orderRepository.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            _logger.LogInformation("Order {OrderId} deleted successfully", id);
        }
        else
        {
            _logger.LogWarning("Order with ID {OrderId} not found for deletion", id);
        }

        return deleted;
    }

    /// <summary>
    /// Builds a complete order response with enriched data (restaurant name, user name, rider name).
    /// </summary>
    private async Task<OrderResponse> BuildOrderResponse(Order order, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(order.RestaurantId, cancellationToken);
        var user = await _userRepository.GetByIdAsync(order.UserId, cancellationToken);
        Rider? rider = order.RiderId.HasValue
            ? await _riderRepository.GetByIdAsync(order.RiderId.Value, cancellationToken)
            : null;

        // Format delivery address
        string deliveryAddressText = "Unknown";
        if (user?.Address != null)
        {
            deliveryAddressText = $"{user.Address.Street}, {user.Address.City}, {user.Address.ZipCode}";
        }

        var response = new OrderResponse
        {
            OrderId = order.Id,
            RestaurantName = restaurant?.Name ?? "Unknown",
            UserName = user?.Name ?? "Unknown",
            RiderName = rider?.Name,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                MenuItemId = i.MenuItemId,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity,
                ItemTotal = i.Price * i.Quantity
            }).ToList(),
            Total = order.Total,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            CreatedAt = order.CreatedAt,
            DeliveryAddress = deliveryAddressText,
            Notes = order.Notes
        };

        // Calculate estimated delivery time
        if (restaurant != null && order.Status != OrderStatus.Delivered && order.Status != OrderStatus.Cancelled)
        {
            var distance = LocationUtils.CalculateDistance(restaurant.Address.Location, order.DeliveryAddress);
            var deliveryTimeMinutes = DeliveryCalculator.EstimateDeliveryTimeMinutes(distance);
            var totalMinutes = restaurant.AveragePreparationTimeMinutes + deliveryTimeMinutes;
            
            var estimatedDeliveryTime = order.CreatedAt.AddMinutes(totalMinutes);
            response.EstimatedDeliveryMinutes = (int)Math.Ceiling((estimatedDeliveryTime - DateTime.UtcNow).TotalMinutes);
            
            // Ensure minutes is not negative
            if (response.EstimatedDeliveryMinutes < 0)
            {
                response.EstimatedDeliveryMinutes = 0;
            }
        }

        return response;
    }
}


