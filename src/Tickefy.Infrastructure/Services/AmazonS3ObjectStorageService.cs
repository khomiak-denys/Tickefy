using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Infrastructure.Options;
using System.Net;

namespace Tickefy.Infrastructure.Services;

public class AmazonS3ObjectStorageService(
    IAmazonS3 client,
    IOptions<ObjectStorageOptions> options,
    ILogger<AmazonS3ObjectStorageService> logger) : IObjectStorageService
{
    private ObjectStorageOptions Options => options.Value;

    public async Task<string> GetUploadUrlAsync(string objectKey, long contentLength, string? contentType = null, TimeSpan? expires = null)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return string.Empty;
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = Options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(expires ?? TimeSpan.FromMinutes(10)),
            ContentType = contentType
        };

        request.Headers.ContentLength = contentLength;

        return await client.GetPreSignedURLAsync(request);
    }

    public async Task<string> GetFileUrlAsync(string objectKey, TimeSpan? expires = null)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return string.Empty;
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = Options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expires ?? TimeSpan.FromMinutes(10))
        };

        var url = await client.GetPreSignedURLAsync(request);
        return NormalizeToPublicEndpoint(url);
    }

    public async Task DeleteFileAsync(string objectKey, CancellationToken cancellationToken)
    {
        try
        {
            await client.DeleteObjectAsync(Options.Bucket, objectKey, cancellationToken);
        }
        catch (AmazonS3Exception e)
        {
            logger.LogWarning("AWS S3 Error deleting {Key}: {Message}", objectKey, e.Message);
        }
        catch (Exception e)
        {
            logger.LogWarning("Error deleting {Key}: {Message}", objectKey, e.Message);
        }
    }

    public async Task<long?> GetContentLengthAsync(string objectKey, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetObjectMetadataAsync(Options.Bucket, objectKey, cancellationToken);

            return response.ContentLength;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <summary>
    /// Rewrites the host/scheme of the signed URL to match the public CDN endpoint.
    /// Used only for download URLs. Upload URLs must always keep the private endpoint host
    /// so the AWS4 signature remains valid.
    /// </summary>
    private string NormalizeToPublicEndpoint(string url)
    {
        if (string.IsNullOrWhiteSpace(Options.PublicEndpoint))
        {
            return url;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var signedUri) ||
            !Uri.TryCreate(Options.PublicEndpoint, UriKind.Absolute, out var publicUri))
        {
            return url;
        }

        var builder = new UriBuilder(signedUri)
        {
            Scheme = publicUri.Scheme,
            Host = publicUri.Host,
            Port = publicUri.IsDefaultPort ? -1 : publicUri.Port
        };

        return builder.Uri.ToString();
    }
}
