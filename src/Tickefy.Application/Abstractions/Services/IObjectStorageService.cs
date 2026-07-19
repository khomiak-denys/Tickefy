namespace Tickefy.Application.Abstractions.Services;

public interface IObjectStorageService
{
    Task<string> GetFileUlrAsync(string objectKey, TimeSpan? expires = null);
    Task<string> GetUploadUrlAsync(string objectKey, TimeSpan? expires = null);
    Task DeleteFileAsync(string objectKey);
}
