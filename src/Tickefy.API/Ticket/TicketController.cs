using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Tickefy.API.Ticket.Requests;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Ticket.Complete;
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
        [SwaggerOperation(Summary = "Handles request to create ticket")]
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
        [SwaggerOperation(Summary ="Handles the request to retrieve all tickets created by current user")]
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
        [SwaggerOperation(Summary = "Handles request to retrieve ticket by id")]
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
        [SwaggerOperation(Summary = "Handles request to retrieve all tickets for admin")]
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
        [SwaggerOperation(Summary = "Handles request to create comment")]
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

        [HttpPost]
        [Authorize(Roles = "Agent, Admin")]
        [Route("{ticketId}/complete")]
        [SwaggerOperation(Summary = "Handles request to complete ticket")]
        public async Task<IActionResult> CompleteTicketAsync(Guid ticketId)
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

            var command = new CompleteTicketCommand
            {
                UserId = new UserId(userId),
                Roles = roles,
                TicketId = new TicketId(ticketId)
            };

            await _mediator.Send(command);

            return Ok();
        }

    }
}

