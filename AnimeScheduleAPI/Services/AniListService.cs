using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnimeScheduleAPI.Models;

namespace AnimeScheduleAPI.Services;

public class AniListService : IAniListService
{
    private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://graphql.anilist.co") };
    private const string Query = "query { Page { media(format_not: MOVIE, season: WINTER, seasonYear:  2024) { id title { romaji english } coverImage { large } episodes siteUrl externalLinks { site url } airingSchedule{ nodes { id episode airingAt } } } } }";
    
    public async Task<IEnumerable<Anime>> GetAnimesSchedule()
    {
        var requestBody = new { query = Query };
        var jsonString = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("", content);
        response.EnsureSuccessStatusCode();

        var jsonResult = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse>(jsonResult);

        if (result is null) throw new Exception("No results found!");

        return result.Data.Page.Media;
    }
}

public interface IAniListService
{
    public Task<IEnumerable<Anime>> GetAnimesSchedule();
}