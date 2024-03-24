using System.Net;
using AnimeScheduleAPI.DTOs;
using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Exceptions;
using AnimeScheduleAPI.Services;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Moq;
using HttpResponseMessage = System.Net.Http.HttpResponseMessage;

namespace AnimeScheduleAPI.Tests;

public class AniListServiceTests
{
    private readonly AniListService _aniListService;
    private readonly Mock<IGraphQLClient> _mockClient;

    public AniListServiceTests()
    {
        _mockClient = new Mock<IGraphQLClient>();
        _aniListService = new AniListService(_mockClient.Object);
    }

    [Fact]
    public async Task GetSchedules_ReturnsDailySchedules()
    {
        // Arrange
        var schedules = new Schedules(
        [
            new AiringSchedule
            {
                MediaId = 1,
                Episode = 1,
                AiringAt = DateTime.Now,
                Media = new Media
                {
                    SiteUrl = "https://example1.com",
                    Title = new Titles("Title 1", "English Title 11"),
                    CoverImage = "cover_image_url",
                    Format = "TV",
                    Type = "Anime",
                    CountryOfOrigin = "JP"
                }
            },
            new AiringSchedule
            {
                MediaId = 2,
                Episode = 1,
                AiringAt = DateTime.Now,
                Media = new Media
                {
                    SiteUrl = "https://example2.com",
                    Title = new Titles("Title 2", "English Title 2"),
                    CoverImage = "cover_image_url",
                    Format = "TV",
                    Type = "Anime",
                    CountryOfOrigin = "JP"
                }
            }
        ]);

        var emptyResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = new AnimeSchedulesResponseDto(
                new Schedules([]))
        };

        var queryResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = new AnimeSchedulesResponseDto(schedules)
        };

        _mockClient.SetupSequence(client =>
                client.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GraphQLHttpResponse<AnimeSchedulesResponseDto>(queryResponse,
                new HttpResponseMessage().Headers, HttpStatusCode.OK))
            .ReturnsAsync(
                new GraphQLHttpResponse<AnimeSchedulesResponseDto>(emptyResponse, new HttpResponseMessage().Headers,
                    HttpStatusCode.OK));

        // Act
        var result = await _aniListService.GetSchedules(DateTime.Now, SearchTypesEnum.Daily);

        // Assert
        _mockClient.Verify(service => service.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));

        Assert.Equal(result, schedules.AiringSchedules);
    }

    [Fact]
    public async Task GetSchedules_ReturnsWeeklySchedules()
    {
        // Arrange
        var schedules = new Schedules(
        [
            new AiringSchedule
            {
                MediaId = 1,
                Episode = 1,
                AiringAt = DateTime.Now,
                Media = new Media
                {
                    SiteUrl = "https://example1.com",
                    Title = new Titles("Title 1", "English Title 11"),
                    CoverImage = "cover_image_url",
                    Format = "TV",
                    Type = "Anime",
                    CountryOfOrigin = "JP"
                }
            },
            new AiringSchedule
            {
                MediaId = 2,
                Episode = 1,
                AiringAt = DateTime.Now,
                Media = new Media
                {
                    SiteUrl = "https://example2.com",
                    Title = new Titles("Title 2", "English Title 2"),
                    CoverImage = "cover_image_url",
                    Format = "TV",
                    Type = "Anime",
                    CountryOfOrigin = "JP"
                }
            }
        ]);

        var emptyResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = new AnimeSchedulesResponseDto(
                new Schedules([]))
        };

        var queryResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = new AnimeSchedulesResponseDto(schedules)
        };

        _mockClient.SetupSequence(client =>
                client.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GraphQLHttpResponse<AnimeSchedulesResponseDto>(queryResponse,
                new HttpResponseMessage().Headers, HttpStatusCode.OK))
            .ReturnsAsync(
                new GraphQLHttpResponse<AnimeSchedulesResponseDto>(emptyResponse, new HttpResponseMessage().Headers,
                    HttpStatusCode.OK));

        // Act
        var result = await _aniListService.GetSchedules(DateTime.Now, SearchTypesEnum.Weekly);

        // Assert
        _mockClient.Verify(service => service.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));

        Assert.Equal(result, schedules.AiringSchedules);
    }

    [Fact]
    public async Task GetSchedules_ThrowsException_WhenSearchTypesEnumIsInvalid()
    {
        // Arrange
        var unsupportedSearchType = (SearchTypesEnum)999;
        var emptyResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = new AnimeSchedulesResponseDto(
                new Schedules([]))
        };

        var mockClient = new Mock<IGraphQLClient>();
        var mockResponse = new GraphQLHttpResponse<AnimeSchedulesResponseDto>(
            emptyResponse,
            new HttpResponseMessage().Headers,
            HttpStatusCode.BadRequest);

        mockClient.Setup(client =>
                client.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var service = new AniListService(mockClient.Object);

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<InvalidSearchTypeException>(() =>
                service.GetSchedules(DateTime.Now, unsupportedSearchType));

        Assert.Equal("Unsupported search type.", exception.Message);
    }

    [Fact]
    public async Task GetSchedules_ThrowsException_WhenStatusCodeIsNot200()
    {
        // Arrange
        var emptyResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = new AnimeSchedulesResponseDto(
                new Schedules([]))
        };

        var mockClient = new Mock<IGraphQLClient>();
        var mockResponse = new GraphQLHttpResponse<AnimeSchedulesResponseDto>(
            emptyResponse,
            new HttpResponseMessage().Headers,
            HttpStatusCode.BadRequest);

        mockClient.Setup(client =>
                client.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var service = new AniListService(mockClient.Object);

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<GraphQlQueryException>(() =>
                service.GetSchedules(DateTime.Now, SearchTypesEnum.Daily));

        Assert.Equal("The query encountered an error.", exception.Message);
    }

    [Fact]
    public async Task GetSchedules_ThrowsException_WhenDataIsNull()
    {
        // Arrange
        var emptyResponse = new GraphQLResponse<AnimeSchedulesResponseDto>
        {
            Data = null!
        };

        var mockClient = new Mock<IGraphQLClient>();
        var mockResponse = new GraphQLHttpResponse<AnimeSchedulesResponseDto>(
            emptyResponse,
            new HttpResponseMessage().Headers,
            HttpStatusCode.OK);

        mockClient.Setup(client =>
                client.SendQueryAsync<AnimeSchedulesResponseDto>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var service = new AniListService(mockClient.Object);

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<GraphQlQueryException>(() =>
                service.GetSchedules(DateTime.Now, SearchTypesEnum.Daily));

        Assert.Equal("The query encountered an error.", exception.Message);
    }

    [Fact]
    public async Task GetAnimeInfo_ReturnsAnimeInfo()
    {
        // Arrange
        var expectedAnimeInfo = new AnimeInfo
        {
            Id = 1,
            IdMal = 123,
            Source = "Source",
            SiteUrl = "https://example.com",
            Title = new Titles("Romaji Title", "English Title"),
            CoverImage = "https://example.com/cover.jpg",
            Format = "TV",
            Type = "Anime",
            CountryOfOrigin = "Japan",
            Status = "Finished Airing",
            Description = "Description",
            AverageScore = 85,
            Season = "Spring",
            SeasonYear = 2024,
            Episodes = 12,
            StartDate = new DateTime(2024, 4, 1),
            EndDate = new DateTime(2024, 6, 30),
            Genres = new List<string> { "Action", "Adventure" },
            NextAiringEpisode = new NextAiringEpisode { AiringAt = new DateTime(2024, 7, 1), Episode = 13 }
        };

        var queryResponse = new GraphQLResponse<AnimeInfoResponseDto>
        {
            Data = new AnimeInfoResponseDto(expectedAnimeInfo)
        };

        _mockClient.Setup(client =>
                client.SendQueryAsync<AnimeInfoResponseDto>(It.IsAny<GraphQLRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GraphQLHttpResponse<AnimeInfoResponseDto>(queryResponse,
                new HttpResponseMessage().Headers, HttpStatusCode.OK));

        // Act
        var result = await _aniListService.GetAnimeInfo(1);

        // Assert
        _mockClient.Verify(service => service.SendQueryAsync<AnimeInfoResponseDto>(It.IsAny<GraphQLRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(result, expectedAnimeInfo);
    }
}