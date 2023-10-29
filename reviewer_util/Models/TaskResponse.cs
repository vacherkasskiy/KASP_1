using System.Text.Json.Serialization;

namespace reviewer_util.Models;

public record TaskResponse
{
    [JsonPropertyName("path")] public string? Path { get; init; }

    [JsonPropertyName("reviewers")] public string[]? Reviewers { get; init; }
}