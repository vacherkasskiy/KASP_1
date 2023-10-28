using System.Text.Json.Serialization;

namespace KASP_1_Console.Models;

public record TaskResponse
{
    [JsonPropertyName("path")]
    public string? Path { get; init; }

    [JsonPropertyName("reviewers")]
    public string[]? Reviewers { get; init; }
}
