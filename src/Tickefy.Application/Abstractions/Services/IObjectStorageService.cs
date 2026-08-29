namespace Tickefy.Application.Abstractions.Services;

public interface IObjectStorageService
{
    /// <summary>
    /// Returns a pre-signed GET URL for downloading an object.
    /// If a PublicEndpoint is configured, rewrites the host so the client receives a CDN-facing URL.
    /// </summary>
    Task<string> GetFileUrlAsync(string objectKey, TimeSpan? expires = null);

    /// <summary>
    /// Returns a pre-signed PUT URL for uploading an object via the private S3 API endpoint.
    /// </summary>
    Task<string> GetUploadUrlAsync(string objectKey, long contentLength, string? contentType = null, TimeSpan? expires = null);

    /// <summary>
    /// Deletes file from object storage.
    /// </summary>
    /// <param name="objectKey"></param>
    /// <returns>Task</returns>
    Task DeleteFileAsync(string objectKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve size of content in bytes
    /// </summary>
    /// <param name="objectKey"></param>
    /// <returns>Null if file isn't found</returns>
    Task<long?> GetContentLengthAsync(string objectKey, CancellationToken cancellationToken = default);
}
