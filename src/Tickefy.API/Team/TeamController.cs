using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.Common.Models;
using Tickefy.API.ErrorHandling;
using Tickefy.API.Team.Requests;
using Tickefy.API.Team.Responses;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Teams.Delete;
using Tickefy.Application.Teams.RemoveMember;
using Tickefy.Application.Teams.GetById;
using Tickefy.Application.Teams.GetAll;
using Tickefy.Application.Teams.GetMy;

namespace Tickefy.API.Team
{
    /// <summary>
    /// Provides RESTful HTTP endpoints for managing organizational support teams, including creation, leadership assignment, member roster administration, and team lookup.
    /// </summary>
    [ApiController]
    [Route("api/v1/teams")]
    [Produces("application/json")]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TeamController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamController"/> class with required MediatR command orchestration dependencies.
        /// </summary>
        /// <param name="mediator">The MediatR mediator instance used to dispatch team administration commands and queries.</param>
        /// <param name="logger">The logger instance for structured logging.</param>
        public TeamController(IMediator mediator, ILogger<TeamController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new support team and assigns the currently authenticated user as the initial team leader.
        /// </summary>
        /// <param name="request">The team creation payload specifying the unique team name and optional descriptive metadata.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 201 Created status upon successful team registration; HTTP 400 Bad Request if the team name is missing or invalid; or HTTP 401 Unauthorized if unauthenticated.
        /// </returns>
        /// <remarks>
        /// The creator automatically inherits leadership privileges for the newly established team roster.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin, Requester")]
        [SwaggerOperation(Summary = "Handles request to create a new team")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTeamAsync([FromBody] CreateTeamRequest request, CancellationToken cancellationToken)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            _logger.LogInformation("Creating team {TeamName} by leader {LeaderId}", request.Name, leaderGuid);
            var command = request.ToCommand(new UserId(leaderGuid));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Created(), this.ToActionResult);
        }

        /// <summary>
        /// Appends a user account to an existing team roster, granting them operator rights within the team's ticket queue.
        /// </summary>
        /// <param name="teamId">The unique primary key GUID of the team to modify.</param>
        /// <param name="request">The member addition request specifying the user account identifier to add to the team.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status on successful roster addition; HTTP 403 Forbidden if the caller is not the designated team leader or an administrator; or HTTP 400 Bad Request if the target user is already a member.
        /// </returns>
        /// <remarks>
        /// Adding a member makes team-assigned queue tickets visible to that user for triage and work acceptance.
        /// </remarks>
        [HttpPatch("{teamId}/members")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Add a member to the team (ONLY TEAM LEADER)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddMemberAsync(Guid teamId, [FromBody] AddMemberRequest request, CancellationToken cancellationToken)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var memberGuid))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            _logger.LogInformation("Adding member to team {TeamId} by leader {LeaderId}", teamId, memberGuid);
            var command = request.ToCommand(new TeamId(teamId), new UserId(memberGuid));

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok(), this.ToActionResult);
        }

        /// <summary>
        /// Revokes team membership for a specified user account, removing their access to team-specific support queues.
        /// </summary>
        /// <param name="teamId">The unique primary key GUID of the target team.</param>
        /// <param name="memberId">The unique primary key GUID of the user to remove from the team roster.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 204 No Content status confirming removal; HTTP 403 Forbidden if attempted by an unauthorized user; or HTTP 400 Bad Request if removing the user would leave the team without a valid leader or if the user is not a member.
        /// </returns>
        /// <remarks>
        /// Removing an agent from a team does not automatically reassign any tickets currently assigned to that individual operator.
        /// </remarks>
        [HttpDelete("{teamId}/members/{memberId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Remove a member from the team (ONLY TEAM LEADER)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveMemberAsync(Guid teamId, Guid memberId, CancellationToken cancellationToken)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            _logger.LogInformation("Removing member {MemberId} from team {TeamId} by leader {LeaderId}", memberId, teamId, leaderGuid);
            var command = new RemoveMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

        /// <summary>
        /// Permanently disbands and deletes a support team record from the system.
        /// </summary>
        /// <param name="teamId">The unique primary key GUID of the team to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 204 No Content status on successful deletion; HTTP 403 Forbidden if the caller is not the team leader or an administrator; or HTTP 404 Not Found if the team record does not exist.
        /// </returns>
        /// <remarks>
        /// Deleting a team requires that all assigned tickets are either re-routed or completed prior to deletion to prevent orphaned support workflows.
        /// </remarks>
        [HttpDelete("{teamId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Delete a team by id (ONLY TEAM LEADER OR ADMIN)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTeamAsync(Guid teamId, CancellationToken cancellationToken)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            _logger.LogInformation("Deleting team {TeamId} by leader {LeaderId}", teamId, leaderGuid);
            var command = new DeleteTeamCommand(new TeamId(teamId), new UserId(leaderGuid));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

        /// <summary>
        /// Retrieves detailed information for a specific team, including its identity, leadership assignment, and complete member roster.
        /// </summary>
        /// <param name="teamId">The unique primary key GUID of the target team to inspect.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing the <see cref="TeamDetailResponse"/> view model; HTTP 401 Unauthorized if unauthenticated; or HTTP 404 Not Found if the team GUID is invalid.
        /// </returns>
        /// <remarks>
        /// Accessible by agents, managers, and administrators to facilitate collaboration and workload visibility.
        /// </remarks>
        [HttpGet("{teamId}")]
        [Authorize(Roles = "Agent, Admin, Manager")]
        [SwaggerOperation(Summary = "Retrieve team by id")]
        [ProducesResponseType(typeof(TeamDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving team {TeamId}", teamId);
            var query = new GetMyTeamQuery(new TeamId(teamId));
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(TeamDetailResponse.FromResult(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Retrieves a complete directory of all registered support teams across the organization for administrative oversight.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a list of <see cref="TeamResponse"/> summaries; or HTTP 403 Forbidden if the caller lacks administrative role privileges.
        /// </returns>
        /// <remarks>
        /// This endpoint is restricted to administrators and provides high-level organizational structure summaries without expanding full member rosters.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all teams")]
        [ProducesResponseType(typeof(PaginationResponse<TeamResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTeamsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all teams with page {Page}, pageSize {PageSize}", page, pageSize);
            var query = new GetAllTeamsQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TeamResponse>(value.Items.Select(TeamResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Retrieves all teams in which the currently authenticated user participates as either a leader or a roster member.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a list of matching <see cref="TeamResponse"/> records; or HTTP 401 Unauthorized if the user claim context is missing or malformed.
        /// </returns>
        /// <remarks>
        /// Used by the client interface to populate team-specific navigation menus and ticket queue filters for the logged-in user.
        /// </remarks>
        [HttpGet("my")]
        [Authorize]
        [SwaggerOperation(Summary = "Retrieve teams of the current user")]
        [ProducesResponseType(typeof(PaginationResponse<TeamResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyTeamAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var memberGuid))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            _logger.LogInformation("Retrieving teams for user {UserId} with page {Page}, pageSize {PageSize}", memberGuid, page, pageSize);
            var query = new GetTeamByUserIdQuery(new UserId(memberGuid)) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TeamResponse>(value.Items.Select(TeamResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }
    }
}
