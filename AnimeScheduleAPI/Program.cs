using System.Text.Json.Serialization;
using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Services;
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

builder.Services.AddScoped<IAniListService, AniListService>();

builder.Services.AddHttpClient("AniListClient", c => { c.BaseAddress = new Uri("https://graphql.anilist.co"); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getSchedules",
        async (IAniListService aniListService, DateTime date, SearchTypesEnum searchType) =>
            Results.Ok(await aniListService.GetSchedules(date, searchType)))
    .WithName("getSchedules")
    .WithOpenApi();

app.Run();