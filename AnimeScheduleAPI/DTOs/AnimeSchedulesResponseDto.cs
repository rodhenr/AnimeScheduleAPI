using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.DTOs;

public record AnimeSchedulesResponseDto(Schedules Page);

public record Schedules(List<AiringSchedule> AiringSchedules);

public record AiringSchedule(int MediaId, int Episode, DateTime AiringAt, Media Media);

public record Media(
    string SiteUrl,
    Title Title,
    CoverImage CoverImage,
    string Format,
    string Type,
    string CountryOfOrigin);

public record Title(string Romaji, string English);

public record CoverImage
{
    [JsonPropertyName("extraLarge")] public required string Url { get; init; }
}