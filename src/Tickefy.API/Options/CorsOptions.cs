using System.ComponentModel.DataAnnotations;

namespace Tickefy.API.Options;

public class CorsOptions
{
    public const string SectionName = nameof(CorsOptions);

    [Required]
    public required string Name { get; init; }
    [Required]
    public required string[] AllowedOrigins { get; init; }
    [Required]
    public required string[] AllowedMethods { get; init; }
    [Required]
    public required string[] AllowedHeaders { get; init; }
}
