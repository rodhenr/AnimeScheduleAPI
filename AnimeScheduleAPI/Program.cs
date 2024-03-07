using System.Text.Json.Serialization;
using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Models;
using AnimeScheduleAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AnimeScheduleAPI",
        Description = "An API to retrieve anime schedule from AniList API",
        Contact = new OpenApiContact { Name = "Rodrigo Henrique", Email = "https://github.com/rodhenr" }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(b =>
    {
        b.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IAniListService, AniListService>();

builder.Services.AddHttpClient("AniListClient", c => { c.BaseAddress = new Uri("https://graphql.anilist.co"); });

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
            
            if (cache.TryGetValue(cacheKey, out List<AiringSchedules>? schedules)) return Results.Ok(schedules);

            schedules = await aniListService.GetSchedules(date, searchType);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(7));

            cache.Set(cacheKey, schedules, cacheEntryOptions);

            return Results.Ok(schedules);
        })
    .WithName("getSchedules")
    .WithOpenApi();

app.Run();