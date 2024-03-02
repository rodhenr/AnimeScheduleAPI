using AnimeScheduleAPI.Enums;
using AnimeScheduleAPI.Models;

namespace AnimeScheduleAPI.Services;

public interface IAniListService
{
    public Task<List<AiringSchedules>> GetSchedules(DateTime date, SearchTypesEnum searchType);
}