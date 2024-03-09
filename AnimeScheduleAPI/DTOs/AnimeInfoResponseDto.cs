using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.DTOs;

public record AnimeInfoResponseDto
{
    public required AnimeInfo Media { get; init; }
}

public record AnimeInfo
{
    public required int Id { get; init; }

    public required int IdMal { get; init; }

    public required string SiteUrl { get; init; }

    public required Title Title { get; init; }

    public required CoverImage CoverImage { get; init; }

    public required string Format { get; init; }

    public required string Type { get; init; }

    public required string CountryOfOrigin { get; init; }

    public required string Status { get; init; }

    public required string Description { get; init; }

    public required int AverageScore { get; init; }

    public required string Season { get; init; }

    public required int SeasonYear { get; init; }

    public required int Episodes { get; init; }

    public required AnimeDate StartDate { get; init; }

    public required AnimeDate EndDate { get; init; }

    public required IEnumerable<string> Genres { get; init; }

    public required NextAiringEpisode NextAiringEpisode { get; init; }
}

public record AnimeDate
{
    public required int Year { get; init; }
    public required int Month { get; init; }
    public required int Day { get; init; }
}

public record NextAiringEpisode
{
    public required DateTime AiringAt { get; init; }
    public required int Episode { get; init; }
}