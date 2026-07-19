namespace Tickefy.API.Attachments.Requests;

public class UploadFileRequest
{
    public required string ClientFileId { get; init; }
    public required string FileName { get; init; }
    public required long SizeBytes { get; init; }
}
