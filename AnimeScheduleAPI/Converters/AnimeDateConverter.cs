using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Converters;

public class AnimeDateConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("year", out var yearElement) || !root.TryGetProperty("month", out var monthElement) ||
            !root.TryGetProperty("day", out var dayElement))
        {
            return null;
        }

        if (yearElement.ValueKind == JsonValueKind.Null || monthElement.ValueKind == JsonValueKind.Null ||
            dayElement.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var year = root.GetProperty("year").GetInt32();
        var month = root.GetProperty("month").GetInt32();
        var day = root.GetProperty("day").GetInt32();

        return new DateTime(year, month, day);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mmZ"));
        }
        else
        {
            writer.WriteNullValue(); // Write null if the DateTime? value is null
        }
    }
}