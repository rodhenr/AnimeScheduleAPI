using System.Text.Json.Serialization;
using AnimeScheduleAPI.Converters;

namespace AnimeScheduleAPI.DTOs;

public record AnimeSchedulesResponseDto(Schedules Page);

public record Schedules(List<AiringSchedule> AiringSchedules);

public record AiringSchedule
{
    public required int MediaId { get; init; }

    public required int Episode { get; init; }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public required DateTime AiringAt { get; init; }

    public required Media Media { get; init; }
}

public record Media
{
    public required string SiteUrl { get; init; }

    public required Titles? Title { get; init; }

    [JsonConverter(typeof(CoverImageConverter))]
    public required string? CoverImage { get; init; }

    public required string? Format { get; init; }

    public required string? Type { get; init; }

    public required string? CountryOfOrigin { get; init; }
}

public record Titles(string Romaji, string English);