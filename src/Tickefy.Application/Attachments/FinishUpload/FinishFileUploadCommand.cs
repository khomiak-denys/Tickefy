using Tickefy.Domain.Common.Results;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;


namespace Tickefy.Application.Attachments.FinishUpload
{
    public class FinishFileUploadCommand : ICommand<Result>
    {
        public required AttachmentId AttachmentId { get; init; }
        public required long SizeBytes { get; init; }
    }
}
