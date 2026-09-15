using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickefy.API.ActivityLog.Requests;
using Tickefy.API.ActivityLog.Responses;
using Tickefy.Application.ActivityLogs.GetAll;
using Tickefy.Application.ActivityLogs.GetByTicketId;
using Tickefy.Domain.Primitives;
using Tickefy.API.Common.Models;

namespace Tickefy.API.ActivityLog
{
    [ApiController]
    [Route("api/v1/logs")]
    [Produces("application/json")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActivityLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginationResponse<LogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllLogsRequest request, CancellationToken cancellationToken)
        {
            var query = request.ToQuery();
            var result = await _mediator.Send(query, cancellationToken);
            var response = new PaginationResponse<LogResponse>(result.Items.Select(LogResponse.FromResult).ToList(), result.Page, result.PageSize, result.TotalCount);

            return Ok(response);
        }

        [HttpGet("ticket/{ticketId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginationResponse<LogResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid ticketId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetLogsByTicketIdQuery(new TicketId(ticketId)) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);
            var response = new PaginationResponse<LogResponse>(result.Items.Select(LogResponse.FromResult).ToList(), result.Page, result.PageSize, result.TotalCount);

            return Ok(response);
        }
    }
}
