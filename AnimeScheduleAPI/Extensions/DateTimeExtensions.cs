namespace AnimeScheduleAPI.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime date) => ((DateTimeOffset)date).ToUnixTimeSeconds();

    public static long UnixTimestampOfFirstDayOfWeek(this DateTime date) =>
        ((DateTimeOffset)date.AddDays(-date.DaysSinceStartOfWeek())).ToUnixTimeSeconds();

    public static long UnixTimestampOfLastDayOfWeek(this DateTime date) =>
        ((DateTimeOffset)date.AddDays(6 - date.DaysSinceStartOfWeek())).ToUnixTimeSeconds();

    private static int DaysSinceStartOfWeek(this DateTime date) => date.DayOfWeek - DayOfWeek.Sunday;
}