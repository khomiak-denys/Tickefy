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
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private CookieOptions _cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        public AuthController(
            IMediator mediator,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = request.ToCommand();
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(
                onSuccess: value =>
                {
                    Response.Cookies.Append("refresh_token", result.Value.RefreshToken, _cookieOptions);
                    return StatusCode(StatusCodes.Status201Created, LoginResponse.FromResult(value));
                },
                onFailure: this.ToActionResult);
        }

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
                    return Ok(LoginResponse.FromResult(value));
                },
                onFailure: this.ToActionResult);
        }

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
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));


            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                _logger.LogWarning("{RefreshToken} cookie not found", "refresh_token");
                return Unauthorized();
            }

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

            return result.Match(
                onSuccess: value =>
                {
                    Response.Cookies.Append("refresh_token", result.Value.RefreshToken, _cookieOptions);
                    return Ok(LoginResponse.FromResult(value));
                },
                onFailure: this.ToActionResult);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                _logger.LogWarning("{RefreshToken} cookie not found", "refresh_token");
                return Unauthorized();
            }

            var result = await _mediator.Send(new LogoutCommand(refreshToken), cancellationToken);

            Response.Cookies.Append("refresh_token", string.Empty, _cookieOptions);

            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
