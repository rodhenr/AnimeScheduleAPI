using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Models;

public record ApiResponse
{
    [JsonPropertyName("data")] public Data Data { get; init; }
}

public record Data
{
    [JsonPropertyName("Page")] public Page Page { get; init; }
}

public record Page
{
    [JsonPropertyName("airingSchedules")] public List<AiringSchedules> AiringSchedules { get; init; }
}

public record AiringSchedules
{
    [JsonPropertyName("mediaId")] public int MediaId { get; init; }
    
    [JsonPropertyName("episode")] public int Episode { get; init; }
    
    [JsonPropertyName("airingAt")] public DateTime AiringAt { get; init; }
    
    [JsonPropertyName("media")] public Media Media { get; init; }
}

public record Media
{
    [JsonPropertyName("siteUrl")] public string Url { get; init; } = null!;
    
    [JsonPropertyName("title")] public Title Titles { get; init; }

    [JsonPropertyName("coverImage")] public CoverImage CoverImage { get; init; }
    
    [JsonPropertyName("format")] public string Format { get; init; }
    
    [JsonPropertyName("type")] public string Type { get; init; }
}

public record Title
{
    [JsonPropertyName("romaji")] public string Romaji { get; init; } = null!;

    [JsonPropertyName("english")] public string English { get; init; } = null!;
}

public record CoverImage
{
    [JsonPropertyName("extraLarge")] public string Url { get; init; } = null!;
}