using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
    public class UserController : ControllerBase 
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UserController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Handles request to retrieve all users (ONLY ADMIN)")]
        public async Task<IActionResult> GetAllAsync()
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<UserResponse>>(result);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("{userId}")]
        [SwaggerOperation(Summary = "Handles request to retrieve user by id (ONLY ADMIN)")]
        public async Task<IActionResult> GetByIdAsync(Guid userId)
        {
            var query = new GetUserByIdQuery(new UserId(userId));
            var result = await _mediator.Send(query);

            var response = _mapper.Map<UserResponse>(result);
            return Ok(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{userId}")]
        [SwaggerOperation(Summary = "Handles request to delete user by id (ONLY ADMIN)")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            var query = new DeleteUserCommand(new UserId(userId));
            await _mediator.Send(query);

            return NoContent();
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        [Route("{userId}")]
        [SwaggerOperation(Summary = "Handles request to set user role (ONLY ADMIN)")]
        public async Task<IActionResult> SetUserRoleAsync(Guid userId, [FromBody] SetUserRoleRequest request)
        {
            var command = request.ToCommand(new UserId(userId));
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPatch]
        [Authorize]
        [Route("update-profile")]
        [SwaggerOperation(Summary = "Handles request to update user profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));

            await _mediator.Send(command);

            return Ok();
        }
    }
}
