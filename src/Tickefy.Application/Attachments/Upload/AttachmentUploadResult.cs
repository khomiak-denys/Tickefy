namespace Tickefy.Application.Attachments.Upload;

public record AttachmentUploadResult(
    string ClientFileId,
    Guid AttachmentId,
    string FileName,
    string UploadUrl
);
