using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tickefy.API.Team.Requests;
using Tickefy.API.Team.Responses;
using Tickefy.Domain.Primitives;
using Tickefy.Application.Team.Delete;
using Tickefy.Application.Team.AddMember;
using Tickefy.Application.Team.RemoveMember;
using Tickefy.Application.Team.GetById;
using Tickefy.Application.Team.GetAll;
using Tickefy.Application.Team.GetMy;

namespace Tickefy.API.Team
{
    [ApiController]
    [Route("api/v1/teams")]
    [Produces("application/json")]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TeamController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Requester")]
        [SwaggerOperation(Summary = "Handles request to create a new team")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTeamAsync([FromBody] CreateTeamRequest request)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
                return Unauthorized("User ID is missing or invalid");

            var command = request.ToCommand(new UserId(leaderGuid));
            await _mediator.Send(command);
            return Created();
        }

        [HttpPatch("{teamId}/members/{memberId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Add a member to the team (ONLY TEAM LEADER)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddMemberAsync(Guid teamId, Guid memberId)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
                return Unauthorized("User ID is missing or invalid");

            var command = new AddMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("{teamId}/members/{memberId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Remove a member from the team (ONLY TEAM LEADER)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveMemberAsync(Guid teamId, Guid memberId)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
                return Unauthorized("User ID is missing or invalid");

            var command = new RemoveMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{teamId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Delete a team by id (ONLY TEAM LEADER OR ADMIN)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTeamAsync(Guid teamId)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
                return Unauthorized("User ID is missing or invalid");

            var command = new DeleteTeamCommand(new TeamId(teamId), new UserId(leaderGuid));
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("{teamId}")]
        [Authorize(Roles = "Admin, Manager")]
        [SwaggerOperation(Summary = "Retrieve team by id")]
        [ProducesResponseType(typeof(TeamDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeamByIdAsync(Guid teamId)
        {
            var query = new GetMyTeamQuery(new TeamId(teamId));
            var result = await _mediator.Send(query);
            var response = _mapper.Map<TeamDetailResponse>(result);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Retrieve all teams")]
        [ProducesResponseType(typeof(List<TeamResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTeamsAsync()
        {
            var query = new GetAllTeamsQuery();
            var result = await _mediator.Send(query);
            var response = _mapper.Map<List<TeamResponse>>(result);
            return Ok(response);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Agent, Manager")]
        [SwaggerOperation(Summary = "Retrieve team of the current user")]
        [ProducesResponseType(typeof(TeamDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyTeamAsync()
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
                return Unauthorized("User ID is missing or invalid");

            var query = new GetTeamByUserIdQuery(new UserId(leaderGuid));
            var result = await _mediator.Send(query);
            var response = _mapper.Map<TeamDetailResponse>(result);
            return Ok(response);
        }
    }
}
