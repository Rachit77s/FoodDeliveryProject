using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryPolaris.Controllers;

/// <summary>
/// API Controller for Rider management.
/// Provides RESTful endpoints for rider operations.
/// </summary>
[ApiController]
[Route("api/riders")]
[Produces("application/json")]
public class RidersController : ControllerBase
{
    private readonly IRiderService _riderService;
    private readonly IOrderService _orderService;
    private readonly ILogger<RidersController> _logger;

    public RidersController(
        IRiderService riderService, 
        IOrderService orderService,
        ILogger<RidersController> logger)
    {
        _riderService = riderService ?? throw new ArgumentNullException(nameof(riderService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new rider account (Register).
    /// </summary>
    /// <param name="request">The rider information to register (without ID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created rider with generated ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RiderResponse>> CreateRider(
        [FromBody] CreateRiderRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var rider = request.ToRider();
            var createdRider = await _riderService.CreateRiderAsync(rider, cancellationToken);
            
            var riderResponse = new RiderResponse
            {
                Id = createdRider.Id,
                Name = createdRider.Name,
                CurrentLocation = createdRider.CurrentLocation,
                Email = createdRider.Email,
                Phone = createdRider.Phone,
                VehicleNumber = createdRider.VehicleNumber,
                RiderStatus = createdRider.RiderStatus.ToString()
            };
            
            return CreatedAtAction(nameof(GetRiderById), new { id = createdRider.Id }, riderResponse);
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
    /// Gets a list of all riders with user-friendly details.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RiderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RiderResponse>>> GetAllRiders(CancellationToken cancellationToken)
    {
        var riders = await _riderService.GetAllRidersAsync(cancellationToken);
        
        var riderResponses = riders.Select(r => new RiderResponse
        {
            Id = r.Id,
            Name = r.Name,
            CurrentLocation = r.CurrentLocation,
            Email = r.Email,
            Phone = r.Phone,
            VehicleNumber = r.VehicleNumber,
            RiderStatus = r.RiderStatus.ToString()
        }).ToList();
        
        return Ok(riderResponses);
    }

    /// <summary>
    /// Gets a single rider's profile with user-friendly details.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiderResponse>> GetRiderById(int id, CancellationToken cancellationToken)
    {
        var rider = await _riderService.GetRiderByIdAsync(id, cancellationToken);
        
        if (rider == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Rider Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Rider with ID {id} was not found."
            });
        }

        var riderResponse = new RiderResponse
        {
            Id = rider.Id,
            Name = rider.Name,
            CurrentLocation = rider.CurrentLocation,
            Email = rider.Email,
            Phone = rider.Phone,
            VehicleNumber = rider.VehicleNumber,
            RiderStatus = rider.RiderStatus.ToString()
        };

        return Ok(riderResponse);
    }

    /// <summary>
    /// Updates a rider's profile (e.g., vehicle, status).
    /// </summary>
    /// <param name="id">The rider ID to update.</param>
    /// <param name="request">The updated rider information (without ID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated rider.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiderResponse>> UpdateRider(int id, 
        [FromBody] UpdateRiderRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var existingRider = await _riderService.GetRiderByIdAsync(id, cancellationToken);
            if (existingRider == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Rider Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Rider with ID {id} was not found."
                });
            }

            var rider = request.ToRider(id, existingRider);
            var updatedRider = await _riderService.UpdateRiderAsync(id, rider, cancellationToken);
            
            var riderResponse = new RiderResponse
            {
                Id = updatedRider.Id,
                Name = updatedRider.Name,
                CurrentLocation = updatedRider.CurrentLocation,
                Email = updatedRider.Email,
                Phone = updatedRider.Phone,
                VehicleNumber = updatedRider.VehicleNumber,
                RiderStatus = updatedRider.RiderStatus.ToString()
            };
            
            return Ok(riderResponse);
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
    /// Deletes/deactivates a rider account.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRider(int id, CancellationToken cancellationToken)
    {
        var deleted = await _riderService.DeleteRiderAsync(id, cancellationToken);
        
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Rider Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Rider with ID {id} was not found."
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Gets order history for a specific rider.
    /// Returns all orders assigned to the rider with pagination support and user-friendly details.
    /// </summary>
    /// <param name="id">The rider ID.</param>
    /// <param name="pageNumber">Page number (1-based). Default is 1.</param>
    /// <param name="pageSize">Number of orders per page. Default is 20.</param>
    /// <param name="includeInProgress">Include orders that are in progress (not completed). Default is true.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of orders with total count.</returns>
    /// <response code="200">Order history retrieved successfully.</response>
    /// <response code="404">Rider not found.</response>
    [HttpGet("{id}/orders")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRiderOrderHistory(int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeInProgress = true,
        CancellationToken cancellationToken = default)
    {
        var rider = await _riderService.GetRiderByIdAsync(id, cancellationToken);
        if (rider == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Rider Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Rider with ID {id} was not found."
            });
        }

        var (orders, totalCount) = await _orderService.GetRiderOrderHistoryAsync(
            id, pageNumber, pageSize, includeInProgress, cancellationToken);

        // Convert orders to OrderResponse DTOs with string status
        var orderResponses = new List<OrderResponse>();
        foreach (var order in orders)
        {
            var orderResponse = await _orderService.GetOrderByIdWithDetailsAsync(order.Id, cancellationToken);
            if (orderResponse != null)
            {
                orderResponses.Add(orderResponse);
            }
        }

        var response = new
        {
            riderId = id,
            riderName = rider.Name,
            orders = orderResponses,
            pagination = new
            {
                currentPage = pageNumber,
                pageSize = pageSize,
                totalCount = totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                includeInProgress = includeInProgress
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Updates a rider's current GPS location.
    /// Used by riders to update their location as they move, enabling accurate distance calculations
    /// for rider assignment to orders.
    /// </summary>
    /// <param name="id">The rider ID whose location to update</param>
    /// <param name="request">The new location coordinates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated rider with new location</returns>
    /// <response code="200">Location updated successfully</response>
    /// <response code="400">Invalid location coordinates</response>
    /// <response code="404">Rider not found</response>
    [HttpPut("{id}/location")]
    [ProducesResponseType(typeof(RiderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiderResponse>> UpdateRiderLocation(int id,
        [FromBody] UpdateRiderLocationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updatedRider = await _riderService.UpdateRiderLocationAsync(
                id, 
                request.NewLocation, 
                cancellationToken);

            _logger.LogInformation(
                "Rider {RiderId} location updated to ({Lat}, {Lon})",
                id, request.NewLocation.Lat, request.NewLocation.Lon);

            var riderResponse = new RiderResponse
            {
                Id = updatedRider.Id,
                Name = updatedRider.Name,
                CurrentLocation = updatedRider.CurrentLocation,
                Email = updatedRider.Email,
                Phone = updatedRider.Phone,
                VehicleNumber = updatedRider.VehicleNumber,
                RiderStatus = updatedRider.RiderStatus.ToString()
            };

            return Ok(riderResponse);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Rider not found while updating location");
            return NotFound(new ProblemDetails
            {
                Title = "Rider Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid location data");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Location",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating rider location");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Location Update Failed",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while updating rider location"
            });
        }
    }
}







