using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tickefy.API.Ticket.Requests;
using Microsoft.AspNetCore.Authorization;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket
{
    [ApiController]
    [Route("api/v1/ticket")]
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
    }
}

