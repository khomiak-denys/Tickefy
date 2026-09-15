using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.ErrorHandling;
using Tickefy.API.User.Requests;
using Tickefy.API.User.Responses;
using Tickefy.Application.Users.Delete;
using Tickefy.Application.Users.GetAll;
using Tickefy.Application.Users.GetById;
using Tickefy.Domain.Primitives;
using Tickefy.API.Common.Models;

namespace Tickefy.API.User
{
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all users (Admin only)")]
        [ProducesResponseType(typeof(PaginationResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetAllUsersQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<UserResponse>(value.Items.Select(UserResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve user by id (Admin only)")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery(new UserId(userId));
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(onSuccess: value => Ok(UserResponse.FromResult(value)),
                onFailure: this.ToActionResult);
        }

        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Retrieve current user profile")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var query = new GetUserByIdQuery(new UserId(userId));
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(onSuccess: value => Ok(UserResponse.FromResult(value)),
                onFailure: this.ToActionResult);
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete user by id (Admin only)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

        [HttpPatch("{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Set user role (Admin only)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetUserRoleAsync(Guid userId, [FromBody] SetUserRoleRequest request, CancellationToken cancellationToken)
        {
            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPatch("update-profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Update current user profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
