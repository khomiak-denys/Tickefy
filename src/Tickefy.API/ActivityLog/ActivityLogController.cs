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
    [Produces("application/json")]
    /// <summary>
    /// Handles API requests for activity log entries.
    /// </summary>
    public class ActivityLogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator used to send activity log queries.</param>
        /// <param name="mapper">The mapper used to convert activity log results to responses.</param>
        public ActivityLogController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all activity log entries using the specified paging request.
        /// </summary>
        /// <param name="request">The activity log query request.</param>
        /// <returns>HTTP 200 OK with a list of <see cref="LogResponse"/>; 400, 401, or 403 on failure.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<LogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllLogsRequest request, CancellationToken cancellationToken)
        {
            var query = request.ToQuery();
            var result = await _mediator.Send(query, cancellationToken);
            var response = _mapper.Map<List<LogResponse>>(result);

            return Ok(response);
        }

        /// <summary>
        /// Gets activity log entries for a ticket.
        /// </summary>
        /// <param name="ticketId">The identifier of the ticket.</param>
        /// <returns>HTTP 200 OK with a list of <see cref="LogResponse"/>; 400, 401, 403, or 404 on failure.</returns>
        [HttpGet("ticket/{ticketId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<LogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var query = new GetLogsByTicketIdQuery(new TicketId(ticketId));
            var result = await _mediator.Send(query, cancellationToken);
            var response = _mapper.Map<List<LogResponse>>(result);

            return Ok(response);
        }
    }
}
