using System.Text;
using System.Text.Json;
using AnimeScheduleAPI.Models;

namespace AnimeScheduleAPI.Services;

public class AniListService : IAniListService
{
    private readonly IHttpClientFactory _clientFactory;
    private const string Query = "query { Page { media(format_not: MOVIE, season: WINTER, seasonYear:  2024) { id title { romaji english } coverImage { large } episodes siteUrl externalLinks { site url } airingSchedule{ nodes { id episode airingAt } } } } }";

    public AniListService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public async Task<IEnumerable<Anime>> GetAnimesSchedule()
    {
        var client = _clientFactory.CreateClient("AniListClient");
        
        var requestBody = new { query = Query };
        var jsonString = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync(string.Empty, content);
        response.EnsureSuccessStatusCode();

        var jsonResult = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse>(jsonResult);

        return result is null ? Enumerable.Empty<Anime>() : result.Data.Page.Media;
    }
}

public interface IAniListService
{
    public Task<IEnumerable<Anime>> GetAnimesSchedule();
}