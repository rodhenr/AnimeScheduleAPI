using System.Text;
using System.Text.Json;
using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Exceptions;
using AnimeScheduleAPI.Extensions;
using AnimeScheduleAPI.Converters;
using AnimeScheduleAPI.DTOs;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AnimeScheduleAPI.Services;

public class AniListService : IAniListService
{
    private readonly GraphQLHttpClient _graphQlClient;

    public AniListService(IHttpClientFactory clientFactory)
    {
        var graphQlHttpClientOptions = new GraphQLHttpClientOptions
        {
            EndPoint = new Uri("https://graphql.anilist.co")
        };

        var jsonOptions = new JsonSerializerOptions
        {
            Converters = { new DataConverter() }
        };

        _graphQlClient = new GraphQLHttpClient(graphQlHttpClientOptions, new SystemTextJsonSerializer(jsonOptions),
            clientFactory.CreateClient());
    }

    /// <summary>
    /// Asynchronously retrieves a collection of airing schedules based on the specified date and search type from the AniList API.
    /// </summary>
    /// <param name="date">The date to fetch the schedules for.</param>
    /// <param name="searchType">The type of search, either daily or weekly.</param>
    /// <returns>A collection of AiringSchedules for the specified date range.</returns>
    public async Task<List<AiringSchedule>> GetSchedules(DateTime date, SearchTypesEnum searchType)
    {
        var (startUnix, endUnix) = GetDateBoundariesInUnixTimestamps(date, searchType);

        var scheduleList = new List<AiringSchedule>();
        var page = 1;

        do
        {
            var result = await FetchAiringSchedules(startUnix, endUnix, page);
            if (result.Count == 0) break;

            scheduleList.AddRange(result);
            page++;
        } while (true);

        return scheduleList;
    }

    public async Task<AnimeInfo> GetAnimeInfo(int id)
    {
        var query = new GraphQLRequest
        {
            Query = $$"""
                      {
                        Media(id:{{id}}){
                          id
                          siteUrl
                          coverImage {
                            extraLarge
                          }
                          title {
                            romaji
                            english
                          }
                          type
                          format
                          status
                          description
                          startDate {
                            year
                            month
                            day
                          }
                          endDate {
                            year
                            month
                            day
                          }
                          idMal
                          season
                          seasonYear
                          episodes
                          countryOfOrigin
                          genres
                          averageScore
                          nextAiringEpisode {
                            airingAt
                            episode
                          }
                        }
                      }
                      """
        };

        var response = await _graphQlClient.SendQueryAsync<AnimeInfoResponseDto>(query);

        return response.Data.Media;
    }

    /// <summary>
    /// Asynchronously fetches a collection of airing schedules from the AniList API.
    /// </summary>
    /// <param name="initialDate">The Unix timestamp representing the initial date.</param>
    /// <param name="finalDate">The Unix timestamp representing the final date.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>A collection of airing schedules, or an empty list if no data is available.</returns>
    private async Task<List<AiringSchedule>> FetchAiringSchedules(long initialDate, long finalDate, int page)
    {
        var query = BuildQuery(initialDate, finalDate, page);

        //response.EnsureSuccessStatusCode();

        var r = await _graphQlClient.SendQueryAsync<object>(query);

        var response = await _graphQlClient.SendQueryAsync<AnimeSchedulesResponseDto>(query);

        return response.Data.Page.AiringSchedules;
    }

    /// <summary>
    /// Constructs a GraphQL query string for fetching airing schedules within a specified date range and page number.
    /// </summary>
    /// <param name="initialDate">The Unix timestamp representing the start of the date range.</param>
    /// <param name="finalDate">The Unix timestamp representing the end of the date range.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <returns>A formatted GraphQL query string.</returns>
    private static GraphQLRequest BuildQuery(long initialDate, long finalDate, int page)
    {
        return new GraphQLRequest
        {
            Query = $$"""
                      {
                          Page(page: {{page}}) {
                              airingSchedules(airingAt_greater:{{initialDate}},airingAt_lesser:{{finalDate}}) {
                                  mediaId
                                  episode
                                  airingAt
                                  media {
                                      siteUrl
                                      title {
                                          romaji
                                          english
                                      }
                                      coverImage {
                                          extraLarge
                                      }
                                      format
                                      type
                                      countryOfOrigin
                                  }
                              }
                          }
                      }
                      """
        };
    }

    /// <summary>
    /// Calculates the start and end Unix timestamps for a given date based on the specified search type.
    /// </summary>
    /// <param name="date">The date to calculate the boundaries for.</param>
    /// <param name="searchType">The type of search, either daily or weekly.</param>
    /// <returns>A tuple containing the start and end Unix timestamps for the specified date range.</returns>
    /// <exception cref="InvalidSearchTypeException">Thrown when an unsupported search type is provided.</exception>
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