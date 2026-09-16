using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Tickefy.API.Common.Models;
using Tickefy.API.ErrorHandling;
using Tickefy.API.Ticket.Requests;
using Tickefy.API.Ticket.Responses;
using Tickefy.Application.Tickets.Accept;
using Tickefy.Application.Tickets.Cancel;
using Tickefy.Application.Tickets.Complete;
using Tickefy.Application.Tickets.Fail;
using Tickefy.Application.Tickets.GetAll;
using Tickefy.Application.Tickets.GetById;
using Tickefy.Application.Tickets.GetMy;
using Tickefy.Application.Tickets.GetQueue;
using Tickefy.Application.Tickets.Revise;
using Tickefy.Application.Tickets.StartWork;
using Tickefy.Application.Tickets.Take;
using Tickefy.Domain.Primitives;

namespace Tickefy.API.Ticket
{
    [ApiController]
    [Route("api/v1/tickets")]
    [Produces("application/json")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketController> _logger;

        public TicketController(
            IMediator mediator,
            ILogger<TicketController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Handles request to create ticket")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Created(), this.ToActionResult);
        }

        [HttpPost("draft")]
        [Authorize]
        [SwaggerOperation(Summary = "Handles request to create draft ticket")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDraftAsync(CreateDraftTicketRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Created(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize]
        [Route("{ticketId:guid}/publish")]
        [SwaggerOperation(Summary = "Handles request to publish ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishAsync(Guid ticketId, PublishTicketRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = request.ToCommand(new UserId(userId), roles, new TicketId(ticketId));
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpGet]
        [Authorize]
        [Route("my")]
        [SwaggerOperation(Summary = "Returns tickets for current user")]
        [ProducesResponseType(typeof(PaginationResponse<TicketResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyTicketsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var query = new GetMyTicketsQuery(new UserId(userId)) { Page = page, PageSize = pageSize };

            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TicketResponse>(value.Items.Select(TicketResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        [HttpGet]
        [Authorize]
        [Route("{TicketId}")]
        [SwaggerOperation(Summary = "Handles request to retrieve ticket by id")]
        [ProducesResponseType(typeof(TicketDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTicketByIdAsync(Guid TicketId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var query = new GetTicketByIdQuery(new UserId(userId), roles, new TicketId(TicketId));
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(TicketDetailsResponse.FromResult(value)),
                onFailure: this.ToActionResult);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Handles request to retrieve all tickets for admin")]
        [ProducesResponseType(typeof(PaginationResponse<TicketResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTicketsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetAllTicketsQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TicketResponse>(value.Items.Select(TicketResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        [HttpGet]
        [Authorize(Roles = "Agent")]
        [Route("queue")]
        [SwaggerOperation(Summary = "Handles request to retrieve all tickets for agent")]
        [ProducesResponseType(typeof(PaginationResponse<TicketResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQueueTicketsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var query = new GetQueueTicketsQuery(new UserId(userId)) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TicketResponse>(value.Items.Select(TicketResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        [HttpPost]
        [Authorize(Roles = "Requester, Agent")]
        [Route("{ticketId}/comment")]
        [SwaggerOperation(Summary = "Handles request to create comment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCommentAsync(Guid ticketId, [FromBody] PostCommentRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId), new TicketId(ticketId));

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Created(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize(Roles = "Agent,Admin")]
        [Route("{ticketId}/take")]
        [SwaggerOperation(Summary = "Handles request to complete ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TakeTicketAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new TakeTicketCommand
            {
                UserId = new UserId(userId),
                Roles = roles,
                TicketId = new TicketId(ticketId)
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize(Roles = "Agent,Admin")]
        [Route("{ticketId:guid}/complete")]
        [SwaggerOperation(Summary = "Handles request to complete ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompleteTicketAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
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

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize(Roles = "Requester,Admin")]
        [Route("{ticketId:guid}/reopen")]
        [SwaggerOperation(Summary = "Handles request to reopen ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReopenTicketAsync(Guid ticketId, ReasonForTicketActionRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("User ID claim is missing or invalid in authenticated context");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new ReopenTicketCommand
            {
                UserId = new UserId(userId),
                Roles = roles,
                TicketId = new TicketId(ticketId),
                Reason = request.Reason
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize(Roles = "Requester,Admin")]
        [Route("{ticketId:guid}/cancel")]
        [SwaggerOperation(Summary = "Handles request to cancel ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelTicketAsync(Guid ticketId, ReasonForTicketActionRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt: Missing or invalid User ID claim in CancelTicket");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new CancelTicketCommand
            (
                new UserId(userId),
                roles,
                new TicketId(ticketId),
                request.Reason
            );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{ticketId:guid}/fail")]
        [SwaggerOperation(Summary = "Handles request to fail ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FailTicketAsync(Guid ticketId, ReasonForTicketActionRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt: Missing or invalid User ID claim in FailTicket");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new FailTicketCommand
            {
                UserId = new UserId(userId),
                Roles = roles,
                TicketId = new TicketId(ticketId),
                Reason = request.Reason
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize]
        [Route("{ticketId:guid}/accept")]
        [SwaggerOperation(Summary = "Handles request to accept work and finish ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt: Missing or invalid User ID claim in AcceptTicket");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new AcceptTicketCommand
            {
                UserId = new UserId(userId),
                Roles = roles,
                TicketId = new TicketId(ticketId)
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut]
        [Authorize(Roles = "Agent,Admin")]
        [Route("{ticketId:guid}/start-work")]
        [SwaggerOperation(Summary = "Handles request to start work on ticket")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StartWorkAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt: Missing or invalid User ID claim in StartWork");
                return Unauthorized("User ID is missing or invalid");
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var command = new StartWorkTicketCommand
            {
                UserId = new UserId(userId),
                Roles = roles,
                TicketId = new TicketId(ticketId)
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
