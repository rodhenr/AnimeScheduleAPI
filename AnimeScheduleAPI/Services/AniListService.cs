using System.Text;
using System.Text.Json;
using AnimeScheduleAPI.Extensions;
using AnimeScheduleAPI.Models;
using AnimeScheduleAPI.Helpers;

namespace AnimeScheduleAPI.Services;

public class AniListService : IAniListService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _jsonOptions = new();

    public AniListService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _jsonOptions.Converters.Add(new UnixTimestampConverter());
    }

    /// <summary>
    /// Retrieves the weekly airing schedules.
    /// </summary>
    /// <returns>A collection of <see cref="AiringSchedules"/> representing the weekly airing schedules.</returns>
    public async Task<IEnumerable<AiringSchedules>> GetWeeklySchedule(DateTime date)
    {
        var client = _clientFactory.CreateClient("AniListClient");

        var initialDateUnix = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc).UnixTimestampOfFirstDayOfWeek();
        var finalDateUnix = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Utc).UnixTimestampOfLastDayOfWeek();

        var scheduleList = new List<AiringSchedules>();
        var page = 1;

        do
        {
            var query = BuildQuery(initialDateUnix, finalDateUnix, page);
            var requestBody = new { query };
            var jsonString = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(string.Empty, content);
            response.EnsureSuccessStatusCode();

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse>(jsonResult, _jsonOptions);

            if (result?.Data.Page.AiringSchedules.Any() ?? false)
            {
                scheduleList.AddRange(result.Data.Page.AiringSchedules);
                page++;
            }
            else
            {
                break;
            }
        } while (true);

        return scheduleList;
    }

    private static string BuildQuery(long initialDate, long finalDate, int page) =>
        $"query{{Page(page: {page}){{airingSchedules(airingAt_greater:{initialDate},airingAt_lesser:{finalDate}){{mediaId episode airingAt media{{siteUrl title{{romaji english}} coverImage{{extraLarge}} format type}}}}}}}}";
}

public interface IAniListService
{
    public Task<IEnumerable<AiringSchedules>> GetWeeklySchedule(DateTime date);
}