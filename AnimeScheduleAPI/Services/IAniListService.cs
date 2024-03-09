using AnimeScheduleAPI.DTOs;
using AnimeScheduleAPI.Enums;

namespace AnimeScheduleAPI.Services;

public interface IAniListService
{
    public Task<List<AiringSchedule>> GetSchedules(DateTime date, SearchTypesEnum searchType);
    public Task<AnimeInfo> GetAnimeInfo(int id);
}