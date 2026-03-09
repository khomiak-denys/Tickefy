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
    public class UserController : ControllerBase 
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UserController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all users (Admin only)")]
        [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);

            return result.Match(onSuccess: value => Ok(_mapper.Map<List<UserResponse>>(value)),
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
        public async Task<IActionResult> GetByIdAsync(Guid userId)
        {
            var query = new GetUserByIdQuery(new UserId(userId));
            var result = await _mediator.Send(query);
            return result.Match(onSuccess: value => Ok(_mapper.Map<UserResponse>(value)),
                onFailure: this.ToActionResult);
        }

        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(Summary = "Retrieve current user profile")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID is missing or invalid");

            var query = new GetUserByIdQuery(new UserId(userId));
            var result = await _mediator.Send(query);
            return result.Match(onSuccess: value => Ok(_mapper.Map<UserResponse>(value)),
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
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            var command = new DeleteUserCommand(new UserId(userId));
            var result = await _mediator.Send(command);
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
        public async Task<IActionResult> SetUserRoleAsync(Guid userId, [FromBody] SetUserRoleRequest request)
        {
            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command);
            return result.Match(NoContent(), this.ToActionResult);
        }

        [HttpPatch("update-profile")]
        [Authorize]
        [SwaggerOperation(Summary = "Update current user profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("User ID is missing or invalid");

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command);
            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
