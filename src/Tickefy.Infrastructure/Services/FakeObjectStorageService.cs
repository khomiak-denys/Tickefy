using Tickefy.Application.Abstractions.Services;

namespace Tickefy.Infrastructure.Services;

public class FakeObjectStorageService : IObjectStorageService
{
    public Task<string> GetFileUlrAsync(string objectKey, TimeSpan? expires = null)
    {
        return Task.FromResult($"FileURL for {objectKey}");
    }

    public Task<string> GetUploadUrlAsync(string objectKey, TimeSpan? expires = null)
    {
        return Task.FromResult($"UploadURL for {objectKey}");
    }

    public Task DeleteFileAsync(string objectKey)
    {
        throw new NotImplementedException();
    }
}
