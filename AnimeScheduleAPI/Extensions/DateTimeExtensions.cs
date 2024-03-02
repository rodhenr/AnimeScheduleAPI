namespace AnimeScheduleAPI.Extensions;

public static class DateTimeExtensions
{
    public static long UnixTimestampOfFirstDayOfWeek(this DateTime date)
    {
        return ((DateTimeOffset)date.AddDays(-date.DaysSinceStartOfWeek())).ToUnixTimeSeconds();
    }

    public static long UnixTimestampOfLastDayOfWeek(this DateTime date)
    {
        return ((DateTimeOffset)date.AddDays(6 - date.DaysSinceStartOfWeek())).ToUnixTimeSeconds();
    }

    private static int DaysSinceStartOfWeek(this DateTime date)
    {
        return date.DayOfWeek - DayOfWeek.Sunday;
    }
}