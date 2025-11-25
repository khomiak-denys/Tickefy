using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.Auth.Requests;
using Tickefy.API.Auth.Responses;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AuthController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Handles request to register user")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var command = request.ToCommand();
            await _mediator.Send(command);
            return Created();
        }
        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Handles request to login user")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var command = request.ToCommand();
            var result = await _mediator.Send(command);

            var response = _mapper.Map<LoginResponse>(result);

            return Ok(response);
        }

        [Authorize]
        [HttpPatch("password")]
        [SwaggerOperation(Summary = "Handles request to reset user password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
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
