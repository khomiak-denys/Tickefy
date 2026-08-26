namespace Tickefy.Application.Abstractions.Services;

public interface IObjectStorageService
{
    Task<string> GetFileUrlAsync(string objectKey, TimeSpan? expires = null);
    Task<string> GetUploadUrlAsync(string objectKey, long contentLength, string? contentType = null, TimeSpan? expires = null);
    Task DeleteFileAsync(string objectKey);
}
