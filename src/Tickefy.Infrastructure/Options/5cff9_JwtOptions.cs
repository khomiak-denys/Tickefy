using System.ComponentModel.DataAnnotations;

namespace Tickefy.Infrastructure.Options
{
    public class JwtSettings
    {
        public const string SectionName = nameof(JwtSettings);
        [Required]
        public required string Key { get; init; }
        [Required]
        public required string Issuer { get; init; }
        [Required]
        public required string Audience { get; init; }
        [Required]
        public required int TokenValidityMins { get; init; }
    }
}
