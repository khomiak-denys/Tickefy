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
    /// <summary>
    /// Provides RESTful HTTP endpoints for administering user identity accounts, retrieving profile metadata, modifying system role assignments, and removing accounts.
    /// </summary>
    [ApiController]
    [Route("api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class with required command dispatching dependencies.
        /// </summary>
        /// <param name="mediator">The MediatR instance used to dispatch user domain commands and queries to their corresponding handlers.</param>
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a complete directory of all registered user accounts in the application for administrative management.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a list of <see cref="UserResponse"/> models; or HTTP 403 Forbidden if the caller lacks administrative privileges.
        /// </returns>
        /// <remarks>
        /// This endpoint is restricted strictly to administrators. Callers should be aware that querying unpaginated user lists in large systems may cause increased memory consumption.
        /// </remarks>
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

        /// <summary>
        /// Retrieves profile details and assigned roles for a specific user account by its unique identifier.
        /// </summary>
        /// <param name="userId">The unique primary key GUID of the target user account to inspect.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing the target <see cref="UserResponse"/> data; HTTP 403 Forbidden if unauthorized; or HTTP 404 Not Found if the user ID does not exist.
        /// </returns>
        /// <remarks>
        /// Administrative privilege is required to inspect arbitrary user profiles by ID; regular users should use the `/me` endpoint to query their own data.
        /// </remarks>
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

        /// <summary>
        /// Retrieves profile metadata and assigned role claims for the currently authenticated user session.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing the authenticated user's <see cref="UserResponse"/> profile; or HTTP 401 Unauthorized if session authentication is missing or invalid.
        /// </returns>
        /// <remarks>
        /// This endpoint extracts the user identity directly from the NameIdentifier claim within the request authorization token or cookie context.
        /// </remarks>
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
            return result.Match(onSuccess: value => Ok(UserResponse.FromResult(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Permanently deletes a user account record and revokes all associated identity access privileges from the system.
        /// </summary>
        /// <param name="userId">The unique primary key GUID of the user account to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 204 No Content status on successful deletion; HTTP 403 Forbidden if attempted by a non-administrator; or HTTP 404 Not Found if the account does not exist.
        /// </returns>
        /// <remarks>
        /// Deleting a user account cannot be undone. Administrators must ensure any active tickets or team leadership assignments held by this user are transferred prior to deletion.
        /// </remarks>
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
        /// Updates system authorization role assignments for a specific user account, altering their operational privileges within the platform.
        /// </summary>
        /// <param name="userId">The unique primary key GUID of the user whose role assignments are being modified.</param>
        /// <param name="request">The role update payload specifying the target authorization role name (e.g., Admin, Agent, Manager, Requester).</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status on successful role modification; HTTP 400 Bad Request if the specified role is invalid; or HTTP 403 Forbidden if invoked by a non-administrator.
        /// </returns>
        /// <remarks>
        /// Role updates take effect immediately for authorization checks on new requests, though active JWT tokens may still carry old role claims until re-issued.
        /// </remarks>
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
        /// Updates profile attributes such as display name or contact preferences for the currently authenticated user account.
        /// </summary>
        /// <param name="request">The profile modification payload containing the updated personal identity attributes.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming profile update; HTTP 400 Bad Request if validation rules fail; or HTTP 401 Unauthorized if unauthenticated.
        /// </returns>
        /// <remarks>
        /// Users can only modify their own personal profile attributes via this endpoint; role assignments and account identifiers remain immutable here.
        /// </remarks>
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
