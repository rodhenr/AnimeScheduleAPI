using System.Text.Json.Serialization;
using AnimeScheduleAPI.Helpers;

namespace AnimeScheduleAPI.Models;

public record ApiResponse
{
    [JsonPropertyName("data")] 
    public Data Data { get; init; }
}

public record Data
{
    [JsonPropertyName("Page")] 
    public Page Page { get; init; }
}

public record Page
{
    [JsonPropertyName("media")] 
    public IEnumerable<Anime> Media { get; init; }
}

public record Anime
{
    [JsonPropertyName("id")] 
    public int Id { get; init; }

    [JsonPropertyName("title")] 
    public Title Titles { get; init; }

    [JsonPropertyName("coverImage")] 
    public CoverImage CoverImage { get; init; }

    [JsonPropertyName("episodes")] 
    public int? Episodes { get; init; }

    [JsonPropertyName("siteUrl")] 
    public string SiteUrl { get; init; } = null!;

    [JsonPropertyName("externalLinks")] 
    public IEnumerable<ExternalLink> ExternalLinks { get; init; }

    [JsonPropertyName("airingSchedule")] 
    public AiringSchedule AiringSchedule { get; init; }
}

public record Title
{
    [JsonPropertyName("romaji")] 
    public string Romaji { get; init; } = null!;

    [JsonPropertyName("english")] 
    public string English { get; init; } = null!;
}

public record CoverImage
{
    [JsonPropertyName("large")] 
    public string Large { get; init; } = null!;
}

public record ExternalLink
{
    [JsonPropertyName("site")] 
    public string Site { get; init; } = null!;

    [JsonPropertyName("url")] 
    public string Url { get; init; } = null!;
}

public record AiringSchedule
{
    [JsonPropertyName("nodes")] 
    public IEnumerable<Node> Nodes { get; init; }
}

public record Node
{
    [JsonPropertyName("airingAt")] 
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime AiringAt { get; init; }

    [JsonPropertyName("episode")] 
    public int? Episode { get; init; }
}