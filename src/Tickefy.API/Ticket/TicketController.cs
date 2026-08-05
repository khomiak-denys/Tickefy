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
    /// <summary>
    /// Provides RESTful HTTP endpoints for orchestrating the complete lifecycle of support tickets, including creation, state transitions, queue assignments, and collaborative discussions.
    /// </summary>
    [ApiController]
    [Route("api/v1/tickets")]
    [Produces("application/json")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketController"/> class with required command dispatching dependencies.
        /// </summary>
        /// <param name="mediator">The MediatR mediator instance used to dispatch domain commands and queries to their corresponding application handlers.</param>
        public TicketController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Submits a request to create and immediately open a new support ticket under the authenticated user's account.
        /// </summary>
        /// <param name="request">The ticket initialization payload containing title, description, deadline, category, priority, and optional attachment references.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 201 Created status upon successful ticket generation; HTTP 400 Bad Request if validation constraints fail; or HTTP 401 Unauthorized if the user session is invalid.
        /// </returns>
        /// <remarks>
        /// Tickets created via this endpoint bypass the draft state and are immediately available for routing and agent triage.
        /// </remarks>
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
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Created(), this.ToActionResult);
        }

        /// <summary>
        /// Stores an unpublished draft ticket for the authenticated user, allowing subsequent modifications before formal submission to the triage queue.
        /// </summary>
        /// <param name="request">The draft creation payload specifying preliminary ticket details and attachment identifiers.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 201 Created status confirming draft storage; HTTP 400 Bad Request if mandatory structural validation fails; or HTTP 401 Unauthorized if unauthenticated.
        /// </returns>
        /// <remarks>
        /// Draft tickets are not visible to support agents and do not trigger SLA timers or routing workflows until explicitly published.
        /// </remarks>
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
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId));
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(Created(), this.ToActionResult);
        }

        /// <summary>
        /// Transitions a previously stored draft ticket into an active, open state, making it available for queue assignment and SLA tracking.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the draft ticket to publish. Must correspond to a ticket owned by the authenticated requester.</param>
        /// <param name="request">The publication payload containing finalized metadata, categories, and priority assignments.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status on successful publication; HTTP 403 Forbidden if the caller does not own the draft; or HTTP 404 Not Found if the target ticket identifier does not exist.
        /// </returns>
        /// <remarks>
        /// Once published, a ticket cannot be reverted to a draft state; subsequent modifications must occur via workflow state transition endpoints.
        /// </remarks>
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

        /// <summary>
        /// Retrieves the collection of all support tickets authored by or assigned to the currently authenticated user identity.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a list of <see cref="TicketResponse"/> summaries; or HTTP 401 Unauthorized if the user claim context is missing or malformed.
        /// </returns>
        /// <remarks>
        /// For requesters, this returns tickets they submitted; for agents, this includes tickets assigned to them for resolution.
        /// </remarks>
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
                return Unauthorized("User ID is missing or invalid");
            }
            var query = new GetMyTicketsQuery(new UserId(userId)) { Page = page, PageSize = pageSize };

            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TicketResponse>(value.Items.Select(TicketResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Retrieves comprehensive details, historical activity logs, attachments, and collaborative comments for a specific ticket identifier.
        /// </summary>
        /// <param name="TicketId">The unique primary key GUID of the target ticket to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing the detailed <see cref="TicketDetailsResponse"/> view model; HTTP 403 Forbidden if the user lacks access rights; or HTTP 404 Not Found if the ticket does not exist.
        /// </returns>
        /// <remarks>
        /// Access permissions are enforced based on user roles: requesters can only view their own tickets, whereas agents and administrators can view any ticket within their authorized queues.
        /// </remarks>
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

        /// <summary>
        /// Retrieves a comprehensive inventory of all support tickets across the entire system for administrative oversight and reporting.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing an unfiltered list of all <see cref="TicketResponse"/> records; or HTTP 403 Forbidden if the caller lacks administrative privileges.
        /// </returns>
        /// <remarks>
        /// This endpoint is strictly restricted to users bearing the Admin role. In large production datasets, callers should monitor payload sizes to avoid high bandwidth consumption.
        /// </remarks>
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

        /// <summary>
        /// Retrieves unassigned or queued support tickets available for triage and work acceptance by support agents within their team scope.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK response containing a list of queued <see cref="TicketResponse"/> records; or HTTP 403 Forbidden if the authenticated user is not an agent.
        /// </returns>
        /// <remarks>
        /// This endpoint is restricted to users bearing the Agent role. Tickets returned represent active work items waiting to be taken by an available operator.
        /// </remarks>
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
                return Unauthorized("User ID is missing or invalid");
            }

            var query = new GetQueueTicketsQuery(new UserId(userId)) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, cancellationToken);

            return result.Match(onSuccess: value => Ok(new PaginationResponse<TicketResponse>(value.Items.Select(TicketResponse.FromResult).ToList(), value.Page, value.PageSize, value.TotalCount)),
                onFailure: this.ToActionResult);
        }

        /// <summary>
        /// Appends a collaborative discussion message or progress update to an existing support ticket thread.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the ticket to comment on.</param>
        /// <param name="request">The comment payload encapsulating message text and any associated file attachment references.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 201 Created status upon successful comment registration; HTTP 400 Bad Request if the comment text is empty; or HTTP 404 Not Found if the target ticket does not exist.
        /// </returns>
        /// <remarks>
        /// Both requesters and assigned agents may participate in ticket discussions. Adding a comment does not alter the underlying workflow lifecycle state of the ticket.
        /// </remarks>
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
                return Unauthorized("User ID is missing or invalid");
            }

            var command = request.ToCommand(new UserId(userId), new TicketId(ticketId));

            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Created(), this.ToActionResult);
        }

        /// <summary>
        /// Assigns an unassigned or queued support ticket to the currently authenticated support agent, initiating active operator handling.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the ticket to take into work.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming ownership assignment; HTTP 403 Forbidden if the caller lacks agent privileges or if the ticket belongs to an unauthorized team; or HTTP 400 Bad Request if the ticket is already assigned.
        /// </returns>
        /// <remarks>
        /// Taking a ticket updates its assignment property and may transition its state depending on whether work is initiated immediately or staged for review.
        /// </remarks>
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

        /// <summary>
        /// Transitions an active ticket into a completed state, indicating that the assigned agent has fulfilled the support request or provided a resolution.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the ticket to mark as completed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming completion; HTTP 400 Bad Request if the ticket is not in a valid state for completion; or HTTP 403 Forbidden if attempted by an unauthorized user.
        /// </returns>
        /// <remarks>
        /// Completing a ticket stages it for requester verification; the ticket is not permanently archived or closed until the requester explicitly accepts the resolution.
        /// </remarks>
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

        /// <summary>
        /// Reverts a resolved or completed ticket back into an active operational state when the provided resolution is deemed unsatisfactory by the requester.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the completed ticket to reopen.</param>
        /// <param name="request">The justification payload containing the mandatory textual reason explaining why work must resume.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming reopening; HTTP 400 Bad Request if the reason is missing or invalid; or HTTP 403 Forbidden if attempted by a user other than the original requester or an administrator.
        /// </returns>
        /// <remarks>
        /// Reopening a ticket resumes SLA timers and notifies the assigned agent or team queue that additional investigation is required.
        /// </remarks>
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

        /// <summary>
        /// Terminates a support ticket workflow prior to completion, marking the item as cancelled and halting further agent processing.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the ticket to cancel.</param>
        /// <param name="request">The cancellation payload specifying the mandatory textual reason for terminating the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming cancellation; HTTP 400 Bad Request if the ticket is already closed or completed; or HTTP 403 Forbidden if the caller lacks authorization.
        /// </returns>
        /// <remarks>
        /// Cancellation is a terminal lifecycle state; once cancelled, a ticket cannot be reopened or transitioned back into an active queue.
        /// </remarks>
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

        /// <summary>
        /// Records an administrative failure outcome for a ticket, indicating that the requested work could not be completed due to technical or business constraints.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the ticket to mark as failed.</param>
        /// <param name="request">The failure justification payload detailing the technical obstacles or SLA violations that caused the failure.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming failure registration; HTTP 403 Forbidden if the caller is not an administrator; or HTTP 404 Not Found if the ticket does not exist.
        /// </returns>
        /// <remarks>
        /// This operation is strictly reserved for system administrators to handle unresolvable edge cases or compliance exceptions.
        /// </remarks>
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

        /// <summary>
        /// Confirms requester satisfaction with the delivered resolution, formally accepting completed work and transitioning the ticket to its final closed state.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the completed ticket to accept and close.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming closure; HTTP 400 Bad Request if the ticket is not currently in a completed awaiting-acceptance state; or HTTP 403 Forbidden if invoked by a non-owner.
        /// </returns>
        /// <remarks>
        /// Acceptance represents the final successful terminal state of a support ticket lifecycle, locking the entity against further edits or status changes.
        /// </remarks>
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

        /// <summary>
        /// Transitions an assigned ticket from a pending or accepted status into an actively in-progress state, indicating that operator investigation has begun.
        /// </summary>
        /// <param name="ticketId">The unique primary key GUID of the ticket on which work is starting.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// An HTTP 200 OK status confirming active work initiation; HTTP 400 Bad Request for invalid state transitions; or HTTP 403 Forbidden if attempted by an unauthorized agent.
        /// </returns>
        /// <remarks>
        /// Starting work logs an audit timestamp used to calculate initial response SLAs and active handling duration.
        /// </remarks>
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
