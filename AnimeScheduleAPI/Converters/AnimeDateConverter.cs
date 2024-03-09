using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Converters;

public class AnimeDateConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var year = root.GetProperty("year").GetInt32();
        var month = root.GetProperty("month").GetInt32();
        var day = root.GetProperty("day").GetInt32();

        return new DateTime(year, month, day);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mmZ"));
}