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
    [ApiController]
    [Route("api/v1/teams")]
    [Produces("application/json")]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TeamController> _logger;

        public TeamController(IMediator mediator, ILogger<TeamController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

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

            var command = request.ToCommand(new UserId(leaderGuid));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Created(), this.ToActionResult);
        }

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

            var command = request.ToCommand(new TeamId(teamId), new UserId(memberGuid));

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok(), this.ToActionResult);
        }

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

            var command = new RemoveMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

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

            var command = new DeleteTeamCommand(new TeamId(teamId), new UserId(leaderGuid));
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(NoContent(), this.ToActionResult);
        }

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

            return result.Match(onSuccess: value => Ok(TeamDetailResponse.FromResult(value)),
                onFailure: this.ToActionResult);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all teams")]
        [ProducesResponseType(typeof(PaginationResponse<TeamResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTeamsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetAllTeamsQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TeamResponse>(value.Items.Select(TeamResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

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

            var query = new GetTeamByUserIdQuery(new UserId(memberGuid)) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TeamResponse>(value.Items.Select(TeamResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }
    }
}
