using FoodDeliveryPolaris.DTOs;
using FoodDeliveryPolaris.Exceptions;
using FoodDeliveryPolaris.Models;
using FoodDeliveryPolaris.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryPolaris.Controllers;

/// <summary>
/// API Controller for User management.
/// Provides RESTful endpoints for user operations.
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService, 
        IOrderService orderService,
        ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new user account (Register).
    /// </summary>
    /// <param name="request">The user information to register (without ID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user with generated ID.</returns>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">Validation failed or user already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> CreateUser(
        [FromBody] CreateUserRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var user = request.ToUser();
            var createdUser = await _userService.CreateUserAsync(user, cancellationToken);
            
            return CreatedAtAction(
                nameof(GetUserById), 
                new { id = createdUser.Id }, 
                createdUser);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("User creation validation failed: {Message}", ex.Message);
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
            _logger.LogWarning("Duplicate user creation attempt: {Message}", ex.Message);
            return BadRequest(new ProblemDetails
            {
                Title = "Duplicate Entity",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user creation");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while creating the user."
            });
        }
    }

    /// <summary>
    /// Gets a list of all users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all users.</returns>
    /// <response code="200">Users retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Gets a single user's profile.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found.</returns>
    /// <response code="200">User found and returned.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserById(
        int id, 
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        
        if (user == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "User Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"User with ID {id} was not found."
            });
        }

        return Ok(user);
    }

    /// <summary>
    /// Updates a user's profile information.
    /// </summary>
    /// <param name="id">The user ID to update.</param>
    /// <param name="request">The updated user information (without ID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">User successfully updated.</response>
    /// <response code="400">Validation failed or duplicate name.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<User>> UpdateUser(int id,
        [FromBody] UpdateUserRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var user = request.ToUser(id);
            var updatedUser = await _userService.UpdateUserAsync(id, user, cancellationToken);
            
            if (updatedUser == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"User with ID '{id}' was not found."
                });
            }

            return Ok(updatedUser);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("User update validation failed: {Message}", ex.Message);
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
            _logger.LogWarning("Duplicate user name in update: {Message}", ex.Message);
            return BadRequest(new ProblemDetails
            {
                Title = "Duplicate Entity",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user update");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while updating the user."
            });
        }
    }

    /// <summary>
    /// Deletes a user account.
    /// </summary>
    /// <param name="id">The user ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">User successfully deleted.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(
        int id, 
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _userService.DeleteUserAsync(id, cancellationToken);
            
            if (!deleted)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"User with ID {id} was not found."
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user deletion");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred while deleting the user."
            });
        }
    }

    /// <summary>
    /// Gets order history for a specific user.
    /// Returns all orders placed by the user with pagination support and user-friendly details.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="pageNumber">Page number (1-based). Default is 1.</param>
    /// <param name="pageSize">Number of orders per page. Default is 20.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of orders with total count.</returns>
    /// <response code="200">Order history retrieved successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id}/orders")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserOrderHistory(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "User Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"User with ID {id} was not found."
            });
        }

        var (orders, totalCount) = await _orderService.GetUserOrderHistoryAsync(
            id, pageNumber, pageSize, cancellationToken);

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
            userId = id,
            userName = user.Name,
            orders = orderResponses,
            pagination = new
            {
                currentPage = pageNumber,
                pageSize = pageSize,
                totalCount = totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };

        return Ok(response);
    }
}
