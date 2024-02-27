using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Models;

public record ApiResponse
{
    [JsonPropertyName("data")] 
    public Data? Data { get; init; }
}

public record Data
{
    [JsonPropertyName("Page")] 
    public Page? Page { get; set; }
}

public record Page
{
    [JsonPropertyName("media")] 
    public IEnumerable<Anime> Media { get; init; }
}

public record Anime
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }

    [JsonPropertyName("title")] 
    public Title Titles { get; set; }

    [JsonPropertyName("coverImage")] 
    public CoverImage CoverImage { get; set; }

    [JsonPropertyName("episodes")] 
    public int? Episodes { get; set; }

    [JsonPropertyName("siteUrl")] 
    public string SiteUrl { get; set; } = null!;

    [JsonPropertyName("externalLinks")] 
    public IEnumerable<ExternalLink> ExternalLinks { get; set; }

    [JsonPropertyName("airingSchedule")] 
    public AiringSchedule AiringSchedule { get; set; }
}

public record Title
{
    [JsonPropertyName("romaji")] 
    public string Romaji { get; set; } = null!;

    [JsonPropertyName("english")] 
    public string English { get; set; } = null!;
}

public record CoverImage
{
    [JsonPropertyName("large")] 
    public string Large { get; set; } = null!;
}

public record ExternalLink
{
    [JsonPropertyName("site")] 
    public string Site { get; set; } = null!;

    [JsonPropertyName("url")] 
    public string Url { get; set; } = null!;
}

public record AiringSchedule
{
    [JsonPropertyName("nodes")] 
    public IEnumerable<Node> Nodes { get; set; }
}

public record Node
{
    [JsonPropertyName("airingAt")] 
    public int? AiringAt { get; set; }

    [JsonPropertyName("episode")] 
    public int? Episode { get; set; }
}