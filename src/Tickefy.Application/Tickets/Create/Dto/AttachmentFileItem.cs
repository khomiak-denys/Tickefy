namespace Tickefy.Application.Tickets.Create.Dto;

public sealed record AttachmentFileItem(
    string ClientFileId,
    string FileName,
    long SizeBytes
);
