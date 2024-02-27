using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using AnimeScheduleAPI.Models;

namespace AnimeScheduleAPI.Services;

public class AniListService : IAniListService
{
    private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://graphql.anilist.co") };

    public async Task<IEnumerable<Anime>> GetAnimesSchedule()
    {
        var query = """
            query {
                  Page {
                    media(season: WINTER, seasonYear: 2024) {
                      id
                      title {
                        romaji
                        english
                      }
                      coverImage {
                        large
                      }
                      episodes
                      siteUrl
                      externalLinks {
                        site
                        url
                      }
                      airingSchedule{
                        nodes {
                          id
                          episode
                          airingAt
                        }
                      }
                    }
                  }
                }
            """;

        var requestBody = new { query };
        var jsonString = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<ApiResponse>(result);

        return jsonResult.Data.Page.Media;
    }
}

public interface IAniListService
{
    public Task<IEnumerable<Anime>> GetAnimesSchedule();
}