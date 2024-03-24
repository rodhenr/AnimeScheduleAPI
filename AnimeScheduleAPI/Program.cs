using AnimeScheduleAPI.DTOs;
using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Services;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.EnableAnnotations(); });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(b =>
    {
        b.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AutoRegister();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(
    new GraphQLHttpClientOptions
    {
        EndPoint = new Uri(builder.Configuration["AniListGraphQLServerUri"]!)
    },
    new SystemTextJsonSerializer())
);

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getSchedules",
        async ([FromServices] IMemoryCache cache, [FromServices] IAniListService aniListService,
            [FromQuery] DateTime date, [FromQuery] SearchTypesEnum searchType) =>
        {
            var cacheKey = $"{date:yyyy-MM-dd}_{searchType}";

            if (cache.TryGetValue(cacheKey, out List<AiringSchedule>? schedules)) return Results.Ok(schedules);

            schedules = await aniListService.GetSchedules(date, searchType);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));

            cache.Set(cacheKey, schedules, cacheEntryOptions);

            return Results.Ok(schedules);
        })
    .WithName("getSchedules")
    .WithOpenApi();

app.MapGet("/getAnimeInfo",
        async ([FromServices] IMemoryCache cache, [FromServices] IAniListService aniListService, [FromQuery] int id) =>
        {
            var cacheKey = $"info_{id}";

            if (cache.TryGetValue(cacheKey, out AnimeInfo? animeInfo)) return Results.Ok(animeInfo);

            animeInfo = await aniListService.GetAnimeInfo(id);

            var expirationTime = animeInfo?.NextAiringEpisode?.AiringAt ?? DateTime.Now.AddDays(3);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            cache.Set(cacheKey, animeInfo, cacheEntryOptions);

            return Results.Ok(animeInfo);
        })
    .WithName("getAnimeInfo")
    .WithOpenApi();

app.Run();