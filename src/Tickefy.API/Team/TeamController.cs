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
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TeamController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Handles request to create a new team")]
        public async Task<IActionResult> CreateTeamAsync([FromBody] CreateTeamRequest request)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(leaderGuid));

            await _mediator.Send(command);

            return Created();
        }

        [HttpPatch]
        [Authorize(Roles = "Admin, Manager")]
        [Route("{teamId}/members/{memberId}")]
        [SwaggerOperation(Summary = "Handles request to add a member to the team (ONLY TEAM LEADER)")]
        public async Task<IActionResult> AddMemberAsync(Guid teamId, Guid memberId)
        {

            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = new AddMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Manager")]
        [Route("{teamId}/members/{memberId}")]
        [SwaggerOperation(Summary = "Handles request to remove a member from the team (ONLY TEAM LEADER)")]
        public async Task<IActionResult> RemoveMemberAsync(Guid teamId, Guid memberId)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = new RemoveMemberCommand(
                new UserId(memberId),
                new UserId(leaderGuid),
                new TeamId(teamId)
            );

            await _mediator.Send(command);

            return NoContent(); 
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Manager")]
        [Route("{teamId}")]
        [SwaggerOperation(Summary = "Handles request to delete the team by id (ONLY TEAM LEADER OR ADMIN)")]
        public async Task<IActionResult> DeleteTeamAsync(Guid teamId)
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = new DeleteTeamCommand(new TeamId(teamId), new UserId(leaderGuid));
            await _mediator.Send(command);

            return NoContent();
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [Route("{teamId}")]
        [SwaggerOperation(Summary = "Handles request to retrieve team by id")]
        public async Task<IActionResult> GetTeamByIdAsync(Guid teamId)
        {
            var query = new GetMyTeamQuery(new TeamId(teamId));

            var result = await _mediator.Send(query);

            var response = _mapper.Map<TeamDetailResponse>(result);

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Handles request to all teams")]
        public async Task<IActionResult> GetAllTeamsAsync()
        {
            var query = new GetAllTeamsQuery();

            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<TeamResponse>>(result);

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [Route("my")]
        [SwaggerOperation(Summary = "Handles request to retrieve team by id")]
        public async Task<IActionResult> GetMyTeamAsync()
        {
            var leaderIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(leaderIdClaim) || !Guid.TryParse(leaderIdClaim, out var leaderGuid))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var query = new GetTeamByUserIdQuery(new UserId(leaderGuid));

            var result = await _mediator.Send(query);

            var response = _mapper.Map<TeamDetailResponse>(result);

            return Ok(response);
        }


    }
}