using Tickefy.Application.Attachments.Upload;

namespace Tickefy.API.Attachments.Response;

public record AttachmentUploadResponse(
    string ClientFileId,
    Guid AttachmentId,
    string FileName,
    string UploadUrl
)
{
    public static AttachmentUploadResponse FromResult(AttachmentUploadResult result)
    {
        return new AttachmentUploadResponse(
            result.ClientFileId,
            result.AttachmentId,
            result.FileName,
            result.UploadUrl
        );
    }
}

