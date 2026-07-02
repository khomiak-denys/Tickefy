using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
    [ApiController]
    [Route("api/v1/teams")]
    [Produces("application/json")]
    /// <summary>
    /// Handles API requests for teams and team membership.
    /// </summary>
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator used to send team commands and queries.</param>
        /// <param name="mapper">The mapper used to convert team results to responses.</param>
        public TeamController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new team for the current user.
        /// </summary>
        /// <param name="request">The team creation request.</param>
        /// <returns>HTTP 201 Created on success; 400 or 401 on failure.</returns>
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
                return Unauthorized("User ID is missing or invalid");

            var command = request.ToCommand(new UserId(leaderGuid));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Created(), this.ToActionResult);
        }

        /// <summary>
        /// Adds a member to a team.
        /// </summary>
        /// <param name="teamId">The identifier of the team.</param>
        /// <param name="request">The member addition request.</param>
        /// <returns>HTTP 200 OK on success; 400, 401, or 403 on failure.</returns>
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
                return Unauthorized("User ID is missing or invalid");

            var command = request.ToCommand(new TeamId(teamId), new UserId(memberGuid));

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok(), this.ToActionResult);
        }

        /// <summary>
        /// Removes a member from a team.
        /// </summary>
        /// <param name="teamId">The identifier of the team.</param>
        /// <param name="memberId">The identifier of the member to remove.</param>
        /// <returns>HTTP 204 No Content on success; 400, 401, or 403 on failure.</returns>
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
                return Unauthorized("User ID is missing or invalid");

            var command = new RemoveMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

        /// <summary>
        /// Deletes a team by identifier.
        /// </summary>
        /// <param name="teamId">The identifier of the team to delete.</param>
        /// <returns>HTTP 204 No Content on success; 401, 403, or 404 on failure.</returns>
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
                return Unauthorized("User ID is missing or invalid");

            var command = new DeleteTeamCommand(new TeamId(teamId), new UserId(leaderGuid));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

        /// <summary>
        /// Gets a team by identifier.
        /// </summary>
        /// <param name="teamId">The identifier of the team.</param>
        /// <returns>HTTP 200 OK with a <see cref="TeamDetailResponse"/>; 401 or 404 on failure.</returns>
        [HttpGet("{teamId}")]
        [Authorize(Roles = "Agent, Admin, Manager")]
        [SwaggerOperation(Summary = "Retrieve team by id")]
        [ProducesResponseType(typeof(TeamDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeamByIdAsync(Guid teamId, CancellationToken cancellationToken)
        {
            var query = new GetMyTeamQuery(new TeamId(teamId));
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(_mapper.Map<TeamDetailResponse>(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Gets all teams.
        /// </summary>
        /// <returns>HTTP 200 OK with a list of <see cref="TeamResponse"/>; 401 on failure.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all teams")]
        [ProducesResponseType(typeof(List<TeamResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTeamsAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllTeamsQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(_mapper.Map<List<TeamResponse>>(value)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Gets teams for the current user.
        /// </summary>
        /// <returns>HTTP 200 OK with a list of <see cref="TeamResponse"/>; 401 or 404 on failure.</returns>
        [HttpGet("my")]
        [Authorize]
        [SwaggerOperation(Summary = "Retrieve teams of the current user")]
        [ProducesResponseType(typeof(List<TeamResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyTeamAsync(CancellationToken cancellationToken)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var memberGuid))
                return Unauthorized("User ID is missing or invalid");

            var query = new GetTeamByUserIdQuery(new UserId(memberGuid));
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(_mapper.Map<List<TeamResponse>>(value)),
                onFailure: this.ToActionResult);
        }
    }
}
