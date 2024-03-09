namespace AnimeScheduleAPI.DTOs;

public record AnimeInfoResponseDto(AnimeInfo Media);

public record AnimeInfo(
    int Id,
    int IdMal,
    string Source,
    string SiteUrl,
    Title Title,
    CoverImage CoverImage,
    string Format,
    string Type,
    string CountryOfOrigin,
    string Status,
    string Description,
    int? AverageScore,
    string Season,
    int? SeasonYear,
    int Episodes,
    AnimeDate StartDate,
    AnimeDate EndDate,
    IEnumerable<string> Genres,
    NextAiringEpisode NextAiringEpisode);

public record AnimeDate(int Year, int Month, int Day);

public record NextAiringEpisode(DateTime AiringAt, int Episode);