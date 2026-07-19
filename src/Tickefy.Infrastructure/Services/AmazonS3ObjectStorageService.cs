using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tickefy.Application.Abstractions.Services;
using Tickefy.Infrastructure.Options;

namespace Tickefy.Infrastructure.Services;

public class AmazonS3ObjectStorageService : IObjectStorageService
{
    private readonly ObjectStorageOptions _options;
    private readonly AmazonS3Client _presignClient;
    private readonly string _bucket;
    private readonly Uri? _presignEndpointUri;
    private readonly ILogger<AmazonS3ObjectStorageService> _logger;

    public AmazonS3ObjectStorageService(
        IOptions<ObjectStorageOptions> options,
        ILogger<AmazonS3ObjectStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var endpoint = _options.Endpoint;
        var publicEndpoint = _options.PublicEndpoint;
        var accessKey = _options.AccessKey;
        var secretKey = _options.SecretKey;
        _bucket = _options.Bucket;
        var region = _options.Region;

        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        using var s3Client = new AmazonS3Client(credentials, BuildConfig(endpoint, region));

        var presignEndpoint = string.IsNullOrWhiteSpace(publicEndpoint)
            ? endpoint
            : publicEndpoint;
        _presignEndpointUri = TryCreateUri(presignEndpoint);
        _presignClient = presignEndpoint == endpoint
            ? s3Client
            : new AmazonS3Client(credentials, BuildConfig(presignEndpoint, region));
    }

    public Task<string> GetFileUrlAsync(string objectKey, TimeSpan? expires = null)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return Task.FromResult(string.Empty);
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = objectKey,
            Expires = DateTime.UtcNow.Add(expires ?? TimeSpan.FromMinutes(10))
        };

        var url = NormalizePresignedUrl(_presignClient.GetPreSignedURL(request));
        return Task.FromResult(url);
    }

    public Task<string> GetUploadUrlAsync(string objectKey, string? contentType = null, TimeSpan? expires = null)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return Task.FromResult(string.Empty);
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.Add(expires ?? TimeSpan.FromMinutes(10))
        };

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            request.ContentType = contentType;
        }

        var url = NormalizePresignedUrl(_presignClient.GetPreSignedURL(request));
        return Task.FromResult(url);
    }

    public async Task DeleteFileAsync(string objectKey)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = objectKey
        };

        try
        {
            var response = await _presignClient.DeleteObjectAsync(deleteRequest);
        }
        catch (AmazonS3Exception e)
        {
            _logger.LogWarning("AWS Error: {Message}", e.Message);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Error: {Message}", e.Message);
        }
    }

    private static AmazonS3Config BuildConfig(string? serviceUrl, string region)
    {
        var config = new AmazonS3Config
        {
            ForcePathStyle = true
        };

        if (!string.IsNullOrWhiteSpace(serviceUrl))
        {
            config.ServiceURL = serviceUrl;
            config.UseHttp = serviceUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
            config.AuthenticationRegion = region;
            return config;
        }

        config.RegionEndpoint = RegionEndpoint.GetBySystemName(region);
        return config;
    }

    private static Uri? TryCreateUri(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;
    }

    private string NormalizePresignedUrl(string url)
    {
        if (_presignEndpointUri is null)
        {
            return url;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var presignedUri))
        {
            return url;
        }

        if (!string.Equals(presignedUri.Host, _presignEndpointUri.Host, StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        if (string.Equals(presignedUri.Scheme, _presignEndpointUri.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        var builder = new UriBuilder(presignedUri)
        {
            Scheme = _presignEndpointUri.Scheme
        };

        if (!_presignEndpointUri.IsDefaultPort)
        {
            builder.Port = _presignEndpointUri.Port;
        }

        return builder.Uri.ToString();
    }
}
