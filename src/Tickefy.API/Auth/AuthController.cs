using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.Auth.Requests;
using Tickefy.API.Auth.Responses;
using Tickefy.API.ErrorHandling;
using Tickefy.Application.Auth.Logout;
using Tickefy.Application.Auth.RefreshTokens;
using Tickefy.Domain.Primitives;

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
            Secure = true,
            SameSite = SameSiteMode.None
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
        /// <returns>HTTP 201 Created on success; 400 Bad Request if validation fails; 409 Conflict if the login is already taken.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = request.ToCommand();
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(
                onSuccess: _ => Created(),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Authenticates a user and returns a login response.
        /// </summary>
        /// <param name="request">The user login request.</param>
        /// <returns>HTTP 200 OK with a <see cref="LoginResponse"/> and a <c>refresh_token</c> cookie on success; 400 or 401 on failure.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = request.ToCommand();
            var result = await _mediator.Send(command, cancellationToken);

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
        /// <returns>HTTP 200 OK on success; 400, 401, or 404 on failure.</returns>
        [Authorize]
        [HttpPatch("password")]
        [SwaggerOperation(Summary = "Handles request to reset user password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        /// <summary>
        /// Refreshes the current session using the <c>refresh_token</c> cookie.
        /// </summary>
        /// <returns>HTTP 200 OK with a new <see cref="LoginResponse"/> and updated <c>refresh_token</c> cookie on success; 401 if the cookie is missing; 403 if the token is invalid or expired.</returns>
        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                _logger.LogInformation("Cookie not found");
                return Unauthorized();
            }

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

            return result.Match(
                onSuccess: value =>
                {
                    Response.Cookies.Append("refresh_token", result.Value.RefreshToken, _cookieOptions);
                    return Ok(_mapper.Map<LoginResponse>(value));
                },
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Logs out the current user by invalidating the <c>refresh_token</c> cookie.
        /// </summary>
        /// <returns>HTTP 200 OK on success; 401 if the cookie is missing; 404 if the token is not found.</returns>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                return Unauthorized();
            }

            var result = await _mediator.Send(new LogoutCommand(refreshToken), cancellationToken);

            Response.Cookies.Append("refresh_token", string.Empty, _cookieOptions);

            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
