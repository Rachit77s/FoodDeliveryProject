using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryPolaris.Controllers;

/// <summary>
/// API controller for order management operations.
/// Provides endpoints for placing orders, viewing order history, and order CRUD operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Calculate shopping cart total and fees without placing the order.
    /// </summary>
    /// <param name="request">Cart calculation request with items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Price breakdown and delivery estimate</returns>
    /// <response code="200">Cart calculated successfully</response>
    /// <response code="400">Invalid request or business rule violation</response>
    /// <response code="404">Restaurant or user not found</response>
    [HttpPost("cart")]
    [ProducesResponseType(typeof(OrderPreviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderPreviewResponse>> CalculateCart(
        [FromBody] PlaceOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var preview = await _orderService.PreviewOrderAsync(request, cancellationToken);

            _logger.LogInformation(
                "Cart calculated for user {UserId} at restaurant {RestaurantId}. Total: {Total:C}",
                request.UserId, request.RestaurantId, preview.Total);

            return Ok(preview);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found while calculating cart");
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while calculating cart");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while calculating cart");
            return StatusCode(500, new { error = "An unexpected error occurred while calculating cart" });
        }
    }

    /// <summary>
    /// Place a new order with validation and automatic total calculation.
    /// Validates restaurant, user, menu items, and delivery radius.
    /// </summary>
    /// <param name="request">Order placement request with items and delivery info</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order with all details and pricing breakdown</returns>
    /// <response code="201">Order placed successfully</response>
    /// <response code="400">Invalid request or business rule violation</response>
    /// <response code="404">Restaurant or user not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> PlaceOrder(
        [FromBody] PlaceOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var orderResponse = await _orderService.PlaceOrderAsync(request, cancellationToken);

            _logger.LogInformation(
                "Order {OrderId} placed successfully by user {UserId}",
                orderResponse.OrderId, request.UserId);

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = orderResponse.OrderId },
                orderResponse);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found while placing order");
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while placing order");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while placing order");
            return StatusCode(500, new { error = "An unexpected error occurred while placing the order" });
        }
    }

    /// <summary>
    /// Gets all orders in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all order responses with enriched details.</returns>
    /// <response code="200">Orders retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrders(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
        
        var orderResponses = new List<OrderResponse>();
        foreach (var order in orders)
        {
            var orderResponse = await _orderService.GetOrderByIdWithDetailsAsync(order.Id, cancellationToken);
            if (orderResponse != null)
            {
                orderResponses.Add(orderResponse);
            }
        }
        
        return Ok(orderResponses);
    }

    /// <summary>
    /// Gets a single order by ID & returns user-friendly order response with ETA.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order response with enriched details.</returns>
    /// <response code="200">Order found and returned.</response>
    /// <response code="404">Order not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrderById(
        int id, 
        CancellationToken cancellationToken)
    {
        try
        {
            var orderResponse = await _orderService.GetOrderByIdWithDetailsAsync(id, cancellationToken);
            
            if (orderResponse == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Order Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Order with ID {id} was not found."
                });
            }

            return Ok(orderResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId}", id);
            return StatusCode(500, new { error = "An unexpected error occurred while retrieving the order" });
        }
    }

    /// <summary>
    /// Updates an existing order status.
    /// </summary>
    /// <param name="id">The order ID to update.</param>
    /// <param name="request">The updated order status information (as strings).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated order response with enriched details.</returns>
    /// <response code="200">Order successfully updated.</response>
    /// <response code="400">Invalid status value.</response>
    /// <response code="404">Order not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> UpdateOrder(int id,
        [FromBody] UpdateOrderRequest request, 
        CancellationToken cancellationToken)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (existingOrder == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Order Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Order with ID '{id}' was not found."
            });
        }

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var orderStatus))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Status",
                Status = StatusCodes.Status400BadRequest,
                Detail = $"Invalid status value: '{request.Status}'. Valid values are: Placed, Accepted, Preparing, Ready, PickedUp, Delivered, Cancelled"
            });
        }

        PaymentStatus? paymentStatus = null;
        if (!string.IsNullOrEmpty(request.PaymentStatus))
        {
            if (!Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var parsedPaymentStatus))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Payment Status",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = $"Invalid payment status value: '{request.PaymentStatus}'. Valid values are: NotPaid, Paid, Failed, Refunded"
                });
            }
            paymentStatus = parsedPaymentStatus;
        }

        existingOrder.Status = orderStatus;
        if (paymentStatus.HasValue)
        {
            existingOrder.PaymentStatus = paymentStatus.Value;
        }

        await _orderService.UpdateOrderAsync(id, existingOrder, cancellationToken);

        var orderResponse = await _orderService.GetOrderByIdWithDetailsAsync(id, cancellationToken);
        return Ok(orderResponse);
    }

    /// <summary>
    /// Deletes an order.
    /// </summary>
    /// <param name="id">The order ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Order successfully deleted.</response>
    /// <response code="404">Order not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(
        int id, 
        CancellationToken cancellationToken)
    {
        var deleted = await _orderService.DeleteOrderAsync(id, cancellationToken);
        
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Order Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Order with ID {id} was not found."
            });
        }

        return NoContent();
    }
}


