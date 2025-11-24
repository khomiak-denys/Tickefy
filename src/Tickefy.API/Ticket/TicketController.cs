using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tickefy.API.Ticket.Requests;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Ticket.GetAll;
using Tickefy.Application.Ticket.GetById;
using Tickefy.Application.Ticket.GetMy;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket
{
    [ApiController]
    [Route("api/v1/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TicketController(
            IMediator mediator,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(CreateTicketRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));
            await _mediator.Send(command);

            return Created();
        }

        [HttpGet]
        [Authorize]
        [Route("my")]
        public async Task<IActionResult> GetMyTicketsAsync()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }
            var query = new GetMyTicketsQuery(new UserId(userId));

            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<TicketResponse>>(result);

            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        [Route("{TicketId}")]
        public async Task<IActionResult> GetTicketByIdAsync(Guid TicketId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var query = new GetTicketByIdQuery(new UserId(userId), roles, new TicketId(TicketId));
            var result = await _mediator.Send(query);

            var response = _mapper.Map<TicketDetailsResponse>(result);
            
            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTicketsAsync()
        {
            var query = new GetAllTicketsQuery();
            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<TicketResponse>>(result);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Requester, Agent")]
        [Route("{ticketId}/comment")]
        public async Task<IActionResult> PostCommentAsync(Guid ticketId, [FromBody] PostCommentRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId), new TicketId(ticketId));

            await _mediator.Send(command);
            return Created();
        }

    }
}

