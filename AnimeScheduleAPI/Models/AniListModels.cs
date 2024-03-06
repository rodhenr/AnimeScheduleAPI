using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Models;

public record ApiResponse
{
    [JsonPropertyName("data")] public required Data Data { get; init; }
}

public record Data
{
    [JsonPropertyName("Page")] public required Page Page { get; init; }
}

public record Page
{
    [JsonPropertyName("airingSchedules")] public required List<AiringSchedules> AiringSchedules { get; init; }
}

public record AiringSchedules
{
    [JsonPropertyName("mediaId")] public required int MediaId { get; init; }
    
    [JsonPropertyName("episode")] public required int Episode { get; init; }
    
    [JsonPropertyName("airingAt")] public required DateTime AiringAt { get; init; }
    
    [JsonPropertyName("media")] public required Media Media { get; init; }
}

public record Media
{
    [JsonPropertyName("siteUrl")] public required string Url { get; init; }
    
    [JsonPropertyName("title")] public required Title Titles { get; init; }

    [JsonPropertyName("coverImage")] public required CoverImage CoverImage { get; init; }
    
    [JsonPropertyName("format")] public required string Format { get; init; }
    
    [JsonPropertyName("type")] public required string Type { get; init; }
    
    [JsonPropertyName("countryOfOrigin")] public required string CountryOfOrigin { get; init; }
}

public record Title
{
    [JsonPropertyName("romaji")] public required string Romaji { get; init; }

    [JsonPropertyName("english")] public required string English { get; init; }
}

public record CoverImage
{
    [JsonPropertyName("extraLarge")] public required string Url { get; init; }
}