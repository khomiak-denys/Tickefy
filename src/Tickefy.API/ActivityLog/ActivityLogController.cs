using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickefy.API.ActivityLog.Requests;
using Tickefy.API.ActivityLog.Responses;
using Tickefy.Application.ActivityLogs.GetAll;
using Tickefy.Application.ActivityLogs.GetByTicketId;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.ActivityLog
{
    /// <summary>
    /// Provides RESTful HTTP endpoints for querying system-wide audit trails, historical event records, and ticket lifecycle transitions.
    /// </summary>
    [ApiController]
    [Route("api/v1/logs")]
    [Produces("application/json")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogController"/> class with required command mediation and result mapping dependencies.
        /// </summary>
        /// <param name="mediator">The MediatR mediator instance used to dispatch activity log retrieval queries to application handlers.</param>
        /// <param name="mapper">The AutoMapper instance used to translate internal activity log domain entities into API response DTOs.</param>
        public ActivityLogController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of activity audit logs across the entire platform for security and compliance monitoring.
        /// </summary>
        /// <param name="request">The query parameters specifying pagination offsets, limits, date ranges, and entity filters.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a list of matching <see cref="LogResponse"/> records; or HTTP 403 Forbidden if the caller lacks administrative privileges.
        /// </returns>
        /// <remarks>
        /// Due to high log ingestion volumes, callers should always specify appropriate page size and date filters to avoid excessive query execution times.
        /// </remarks>
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
        /// Retrieves the complete audit history of all state changes, comments, and operator actions recorded against a specific support ticket.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the target ticket whose audit logs are being requested.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing the historical <see cref="LogResponse"/> timeline; HTTP 403 Forbidden if unauthorized; or HTTP 404 Not Found if the ticket does not exist.
        /// </returns>
        /// <remarks>
        /// Currently restricted to system administrators for audit verification and dispute resolution.
        /// </remarks>
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
