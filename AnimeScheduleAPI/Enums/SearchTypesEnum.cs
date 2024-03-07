using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchTypesEnum
{
    Daily,
    Weekly
}