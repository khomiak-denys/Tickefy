using System.ComponentModel.DataAnnotations;

namespace Tickefy.Infrastructure.Options;

public class ObjectStorageOptions
{
    public const string SectionName = nameof(ObjectStorageOptions);

    [Required]
    public required string Endpoint { get; init; }
    [Required]
    public required string PublicEndpoint { get; init; }
    [Required]
    public required string AccessKey { get; init; }
    [Required]
    public required string SecretKey { get; init; }
    [Required]
    public required string Bucket { get; init; }
    [Required]
    public required string Region { get; init; }
    public bool ForcePathStyle { get; init; } = true;
}
