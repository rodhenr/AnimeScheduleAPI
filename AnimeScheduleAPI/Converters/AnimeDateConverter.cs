using System.Text.Json;
using System.Text.Json.Serialization;
using AnimeScheduleAPI.Extensions;

namespace AnimeScheduleAPI.Converters;

public class AnimeDateConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("year", out var yearElement) || yearElement.ValueKind == JsonValueKind.Null ||
            !root.TryGetProperty("month", out var monthElement) || monthElement.ValueKind == JsonValueKind.Null ||
            !root.TryGetProperty("day", out var dayElement) || dayElement.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        
        return new DateTime(yearElement.GetInt32(), monthElement.GetInt32(), dayElement.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mmZ"));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}