using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.ErrorHandling;
using Tickefy.API.User.Requests;
using Tickefy.API.User.Responses;
using Tickefy.Application.User.Delete;
using Tickefy.Application.User.GetAll;
using Tickefy.Application.User.GetById;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.User
{
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    /// <summary>
    /// Handles API requests for users.
    /// </summary>
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator used to send user commands and queries.</param>
        /// <param name="mapper">The mapper used to convert user results to responses.</param>
        public UserController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>HTTP 200 OK with a list of <see cref="UserResponse"/>; 401 or 403 if unauthorized.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all users (Admin only)")]
        [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(_mapper.Map<List<UserResponse>>(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Gets a user by identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>HTTP 200 OK with a <see cref="UserResponse"/>; 401, 403, or 404 on failure.</returns>
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
            return result.Match(onSuccess: value => Ok(_mapper.Map<UserResponse>(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Gets the current user's profile.
        /// </summary>
        /// <returns>HTTP 200 OK with the current user's <see cref="UserResponse"/>; 401 if not authenticated.</returns>
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
                return Unauthorized("User ID is missing or invalid");

            var query = new GetUserByIdQuery(new UserId(userId));
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(onSuccess: value => Ok(_mapper.Map<UserResponse>(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Deletes a user by identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user to delete.</param>
        /// <returns>HTTP 204 No Content on success; 401, 403, or 404 on failure.</returns>
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

        /// <summary>
        /// Sets a user's role.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="request">The user role update request.</param>
        /// <returns>HTTP 200 OK on success; 400, 401, or 403 on failure.</returns>
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

        /// <summary>
        /// Updates the current user's profile.
        /// </summary>
        /// <param name="request">The profile update request.</param>
        /// <returns>HTTP 200 OK on success; 400 or 401 on failure.</returns>
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
                return Unauthorized("User ID is missing or invalid");

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
