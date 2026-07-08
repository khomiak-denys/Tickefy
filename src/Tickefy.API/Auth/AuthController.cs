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
    /// <summary>
    /// Provides RESTful HTTP endpoints for orchestrating user authentication, identity onboarding, session token lifecycle management, and credential updates.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class with required command mediation and diagnostic logging dependencies.
        /// </summary>
        /// <param name="mediator">The MediatR instance used to dispatch authentication and registration commands to domain handlers.</param>
        /// <param name="logger">The diagnostic logger used to capture authentication flows, cookie parsing errors, and security events.</param>
        public AuthController(
            IMediator mediator,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Processes a new user account onboarding request, validates account uniqueness, creates domain records, and issues initial cryptographic authentication credentials.
        /// </summary>
        /// <param name="request">The data transfer object containing user identity attributes and plaintext secret credentials. Must pass model validation rules prior to execution.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 201 Created response containing a <see cref="LoginResponse"/> payload and an HttpOnly <c>refresh_token</c> cookie upon success; HTTP 400 Bad Request if validation rules fail; or HTTP 409 Conflict if the requested login identifier is already in use.
        /// </returns>
        /// <remarks>
        /// This endpoint sets a secure, HTTP-only cookie with a 7-day expiration duration. Clients must be configured to accept and transmit credentials/cookies for seamless token refreshing.
        /// </remarks>
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

        /// <summary>
        /// Evaluates user credentials against stored cryptographic hashes, establishing an authenticated session and issuing JWT access and refresh token credentials upon success.
        /// </summary>
        /// <param name="request">The login credentials payload containing the account username and candidate plaintext password.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a <see cref="LoginResponse"/> with an access token and an HttpOnly <c>refresh_token</c> cookie upon successful verification; HTTP 401 Unauthorized if authentication fails; or HTTP 400 Bad Request for malformed payloads.
        /// </returns>
        /// <remarks>
        /// To mitigate brute-force credential stuffing attacks, clients should implement appropriate rate limiting and account lockout strategies when repeated 401 responses occur.
        /// </remarks>
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

        /// <summary>
        /// Updates the cryptographic password hash for the currently authenticated user session after validating the existing password secret.
        /// </summary>
        /// <param name="request">The credential change payload specifying the existing current password and the proposed new password secret.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status on successful credential update; HTTP 401 Unauthorized if the authentication context is missing or invalid; or HTTP 400 Bad Request if the new password fails password complexity validation.
        /// </returns>
        /// <remarks>
        /// This endpoint requires a valid JWT access token bearing a NameIdentifier claim. Modifying the password does not automatically revoke active refresh tokens across other devices unless explicitly handled by session revocation policies.
        /// </remarks>
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
        /// Exchanges a valid, non-expired refresh token cookie for a newly minted JWT access token and a rotated refresh token cookie to extend session lifetime.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a fresh <see cref="LoginResponse"/> and updated HttpOnly <c>refresh_token</c> cookie; HTTP 401 Unauthorized if the cookie is missing from the request headers; or HTTP 403 Forbidden if the token has been revoked, expired, or tampered with.
        /// </returns>
        /// <remarks>
        /// This method enforces refresh token rotation; upon successful exchange, the previously presented refresh token is invalidated to prevent replay attacks.
        /// </remarks>
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
                    return Ok(LoginResponse.FromResult(value));
                },
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Terminates the current authenticated user session by revoking the stored refresh token record and clearing the client browser cookie value to an empty string.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response confirming session termination; HTTP 401 Unauthorized if the refresh token cookie is absent; or HTTP 404 Not Found if the token was already removed or unrecognized in persistence storage.
        /// </returns>
        /// <remarks>
        /// Note that logging out invalidates refresh capabilities but cannot remotely revoke stateless JWT access tokens until their natural expiration timestamp is reached.
        /// </remarks>
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
