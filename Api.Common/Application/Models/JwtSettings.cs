using System.ComponentModel.DataAnnotations;

namespace Api.Common.Application.Models;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public required string SecretKey { get; init; }
    [Required]
    public required string Issuer { get; init; }
    [Required]
    public required string Audience { get; init; }
    [Required]
    public required int ExpirationInMinutes { get; init; }
}