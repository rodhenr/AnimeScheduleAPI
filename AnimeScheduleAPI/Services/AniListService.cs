using System.Net;
using GraphQL.Client.Http;
using AnimeScheduleAPI.DTOs;
using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Exceptions;
using AnimeScheduleAPI.Extensions;
using AnimeScheduleAPI.GraphQLQueries;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace AnimeScheduleAPI.Services;

[RegisterScoped]
public class AniListService : IAniListService
{
    private readonly IGraphQLClient _client;

    public AniListService(IGraphQLClient client)
    {
        _client = client;
    }

    public async Task<List<AiringSchedule>> GetSchedules(DateTime date, SearchTypesEnum searchType)
    {
        var (startUnix, endUnix) = GetDateBoundariesInUnixTimestamps(date, searchType);

        var scheduleList = new List<AiringSchedule>();
        var page = 1;

        do
        {
            var query = AniListQueries.BuildSchedulesQuery(startUnix, endUnix, page);
            var result = await ExecuteQueryAsync<AnimeSchedulesResponseDto>(query);

            var data = result.Page.AiringSchedules;

            if (data.Count == 0) break;

            scheduleList.AddRange(data);
            page++;
        } while (true);

        return scheduleList;
    }

    public async Task<AnimeInfo> GetAnimeInfo(int id)
    {
        var query = AniListQueries.BuildAnimeInfoQuery(id);
        var result = await ExecuteQueryAsync<AnimeInfoResponseDto>(query);

        return result.Media;
    }

    private async Task<TRequest> ExecuteQueryAsync<TRequest>(GraphQLRequest query) where TRequest : class
    {
        var response = await _client.SendQueryAsync<TRequest>(query);

        var graphQlHttpResponse = response.AsGraphQLHttpResponse();
        if (graphQlHttpResponse.StatusCode != HttpStatusCode.OK || graphQlHttpResponse.Data == null)
        {
            throw new GraphQlQueryException("The query encountered an error.");
        }

        return response.Data;
    }

    private static (long initialDateUnix, long finalDateUnix) GetDateBoundariesInUnixTimestamps(DateTime date,
        SearchTypesEnum searchType)
    {
        var initialDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        var finalDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Utc);

        return searchType switch
        {
            SearchTypesEnum.Daily => (initialDate.ToUnixTimestamp(), finalDate.ToUnixTimestamp()),
            SearchTypesEnum.Weekly => (initialDate.UnixTimestampOfFirstDayOfWeek(),
                finalDate.UnixTimestampOfLastDayOfWeek()),
            _ => throw new InvalidSearchTypeException("Unsupported search type.")
        };
    }
}