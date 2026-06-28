using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.Auth.Requests;
using Tickefy.API.Auth.Responses;
using Tickefy.API.ErrorHandling;
using Tickefy.Application.Auth.Logout;
using Tickefy.Application.Auth.RefreshToken;
using Tickefy.Domain.Primitives;
using Tickefy.Domain.RefreshToken;

namespace Tickefy.API.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    /// <summary>
    /// Handles API requests for authentication and password management.
    /// </summary>
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private CookieOptions _cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7),
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator used to send authentication commands.</param>
        /// <param name="mapper">The mapper used to convert authentication results to responses.</param>
        /// <param name="logger">The logger used to write authentication logs.</param>
        public AuthController(
            IMediator mediator,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The user registration request.</param>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var command = request.ToCommand();
            var result = await _mediator.Send(command);
            return result.Match(
                onSuccess: _ => Created(),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Authenticates a user and returns a login response.
        /// </summary>
        /// <param name="request">The user login request.</param>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var command = request.ToCommand();
            var result = await _mediator.Send(command);

            return result.Match(
                onSuccess: value =>
                {
                    Response.Cookies.Append("refresh_token", result.Value.RefreshToken, _cookieOptions);
                    return Ok(_mapper.Map<LoginResponse>(value));
                },
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Updates the current user's password.
        /// </summary>
        /// <param name="request">The password update request.</param>
        [Authorize]
        [HttpPatch("password")]
        [SwaggerOperation(Summary = "Handles request to reset user password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));

            var result = await _mediator.Send(command);

            return result.Match(Ok(), this.ToActionResult);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));

            return result.Match(
                onSuccess: value =>
                {
                    Response.Cookies.Append("refresh_token", result.Value.RefreshToken, _cookieOptions);
                    return Ok(_mapper.Map<LoginResponse>(value));
                },
                onFailure: this.ToActionResult);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new LogoutCommand(refreshToken));

            Response.Cookies.Append("refresh_token", string.Empty, _cookieOptions);

            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
