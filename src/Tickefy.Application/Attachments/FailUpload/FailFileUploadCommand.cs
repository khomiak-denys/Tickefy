using Tickefy.Domain.Common.Results;
using Tickefy.Application.Abstractions.Messaging;
using Tickefy.Domain.Primitives;

namespace Tickefy.Application.Attachments.FailUpload
{
    public class FailFileUploadCommand : ICommand<Result>
    {
        public required AttachmentId AttachmentId { get; init; }
        public required long SizeBytes { get; init; }
    }
}

