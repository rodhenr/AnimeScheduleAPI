using System.Text.Json.Serialization;
using AnimeScheduleAPI.Converters;

namespace AnimeScheduleAPI.DTOs;

public record AnimeInfoResponseDto(AnimeInfo Media);

public record AnimeInfo
{
    public required int Id { get; init; }

    public required int? IdMal { get; init; }

    public required string? Source { get; init; }

    public required string? SiteUrl { get; init; }

    public required Titles? Title { get; init; }

    [JsonConverter(typeof(CoverImageConverter))]
    public required string? CoverImage { get; init; }

    public required string? Format { get; init; }

    public required string? Type { get; init; }

    public required string? CountryOfOrigin { get; init; }

    public required string? Status { get; init; }

    public required string? Description { get; init; }

    public required int? AverageScore { get; init; }

    public required string? Season { get; init; }

    public required int? SeasonYear { get; init; }

    public required int? Episodes { get; init; }

    [JsonConverter(typeof(AnimeDateConverter))]
    public required DateTime? StartDate { get; init; }

    [JsonConverter(typeof(AnimeDateConverter))]
    public required DateTime? EndDate { get; init; }

    public required IEnumerable<string>? Genres { get; init; }

    public required NextAiringEpisode? NextAiringEpisode { get; init; }
}

public record NextAiringEpisode
{
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public required DateTime AiringAt { get; init; }

    public required int? Episode { get; init; }
}