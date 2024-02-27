using AnimeScheduleAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAniListService, AniListService>();

builder.Services.AddHttpClient("AniListClient", c =>
{
    c.BaseAddress = new Uri("https://graphql.anilist.co");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getSchedule", async (IAniListService aniListService) => Results.Ok(await aniListService.GetAnimesSchedule()))
    .WithName("getSchedule")
    .WithOpenApi();

app.Run();