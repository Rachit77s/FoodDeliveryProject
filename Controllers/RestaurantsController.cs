using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryPolaris.Controllers;

/// <summary>
/// API Controller for Restaurant management.
/// </summary>
[ApiController]
[Route("api/restaurants")]
[Produces("application/json")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    private readonly ILogger<RestaurantsController> _logger;

    public RestaurantsController(IRestaurantService restaurantService, ILogger<RestaurantsController> logger)
    {
        _restaurantService = restaurantService ?? throw new ArgumentNullException(nameof(restaurantService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new restaurant profile.
    /// </summary>
    /// <param name="request">The restaurant information to register (without ID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created restaurant with generated ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Restaurant), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Restaurant>> CreateRestaurant(
        [FromBody] CreateRestaurantRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var restaurant = request.ToRestaurant();
            var createdRestaurant = await _restaurantService.CreateRestaurantAsync(restaurant, cancellationToken);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = createdRestaurant.Id }, createdRestaurant);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message,
                Extensions = { ["errors"] = ex.Errors }
            });
        }
        catch (DuplicateEntityException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Duplicate Entity",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Gets a list of all restaurants (for browsing).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Restaurant>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Restaurant>>> GetAllRestaurants(CancellationToken cancellationToken)
    {
        var restaurants = await _restaurantService.GetAllRestaurantsAsync(cancellationToken);
        return Ok(restaurants);
    }

    /// <summary>
    /// Gets a single restaurant's details (menu, etc.).
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Restaurant), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Restaurant>> GetRestaurantById(int id, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id, cancellationToken);

        if (restaurant == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Restaurant Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Restaurant with ID '{id}' was not found."
            });
        }

        return Ok(restaurant);
    }

    /// <summary>
    /// Updates a restaurant's info (name, hours, etc.).
    /// </summary>
    /// <param name="id">The restaurant ID to update.</param>
    /// <param name="request">The updated restaurant information (without ID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated restaurant.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Restaurant), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Restaurant>> UpdateRestaurant(int id,
        [FromBody] UpdateRestaurantRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var restaurant = request.ToRestaurant(id);
            var updatedRestaurant = await _restaurantService.UpdateRestaurantAsync(id, restaurant, cancellationToken);

            if (updatedRestaurant == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Restaurant Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Restaurant with ID '{id}' was not found."
                });
            }

            return Ok(updatedRestaurant);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message,
                Extensions = { ["errors"] = ex.Errors }
            });
        }
        catch (DuplicateEntityException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Duplicate Entity",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Deletes a restaurant profile.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRestaurant(int id, CancellationToken cancellationToken)
    {
        var deleted = await _restaurantService.DeleteRestaurantAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Restaurant Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Restaurant with ID '{id}' was not found."
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Restaurant accepts an order.
    /// Updates order status from Placed to Accepted and automatically assigns nearest available rider.
    /// </summary>
    /// <param name="id">Restaurant ID</param>
    /// <param name="orderId">Order ID to accept</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated order with acceptance confirmation and rider assignment (if available)</returns>
    /// <response code="200">Order accepted successfully</response>
    /// <response code="400">Invalid status transition or business rule violation</response>
    /// <response code="404">Order not found</response>
    [HttpPost("{id}/orders/{orderId}/accept")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> AcceptOrder(
        int id,
        int orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            // Note: Could add validation that order.RestaurantId == id
            // For now, orderId is sufficient
            var orderService = HttpContext.RequestServices.GetRequiredService<IOrderService>();
            var orderResponse = await orderService.AcceptOrderAsync(orderId, cancellationToken);

            _logger.LogInformation(
                "Restaurant {RestaurantId} accepted order {OrderId}",
                id, orderId);

            return Ok(orderResponse);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Order not found while accepting");
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while accepting order");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while accepting order");
            return StatusCode(500, new { error = "An unexpected error occurred while accepting the order" });
        }
    }
}


