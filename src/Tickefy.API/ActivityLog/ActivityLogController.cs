using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickefy.API.ActivityLog.Requests;
using Tickefy.API.ActivityLog.Responses;
using Tickefy.Application.ActivityLog.GetAll;
using Tickefy.Application.ActivityLog.GetByTicketId;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.ActivityLog
{
    [ApiController]
    [Route("api/v1/logs")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public ActivityLogController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllLogsRequest request)
        {
            var query = request.ToQuery();
            
            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<LogResponse>>(result);

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("ticket/{ticketId}")]
        public async Task<IActionResult> GetByIdAsync(Guid ticketId)
        {
            var query = new GetLogsByTicketIdQuery(new TicketId(ticketId));

            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<LogResponse>>(result);

            return Ok(response);
        }
    }
}
