using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickefy.API.Attachments.Requests;
using Tickefy.API.ErrorHandling;
using Tickefy.Domain.Primitives;
using Swashbuckle.AspNetCore.Annotations;


namespace Tickefy.API.Attachments
{
    [ApiController]
    [Authorize]
    [Route("api/v1/attachments")]
    [Produces("application/json")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttachmentsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPut("{attachmentId:guid}/finish")]
        [SwaggerOperation(Summary = "Handles request to finish attachment upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FinishUploadAsync([FromRoute] Guid attachmentId, FinishFileUploadRequest request)
        {
            var result = await _mediator.Send(request.ToCommand(new AttachmentId(attachmentId)));

            return result.Match(Ok(), this.ToActionResult);
        }

        [HttpPut("{attachmentId:guid}/fail")]
        [SwaggerOperation(Summary = "Handles request to fail attachment upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FailUploadAsync(Guid attachmentId, FailFileUploadRequest request)
        {
            var result = await _mediator.Send(request.ToCommand(new AttachmentId(attachmentId)));

            return result.Match(Ok(), this.ToActionResult);
        }
    }
}
