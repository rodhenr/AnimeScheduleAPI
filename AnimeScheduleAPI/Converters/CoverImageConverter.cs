using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnimeScheduleAPI.Converters;

public class CoverImageConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var document))
            return null;

        try
        {
            var root = document.RootElement;
            return root.TryGetProperty("extraLarge", out var extraLargeElement) ? extraLargeElement.GetString() : null;
        }
        finally
        {
            document.Dispose();
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}