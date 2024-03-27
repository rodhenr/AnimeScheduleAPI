using System.Text;
using System.Text.Json;
using AnimeScheduleAPI.Converters;

namespace AnimeScheduleAPI.Tests;

public class UnixDateTimeConverterTests
{
    private readonly JsonSerializerOptions _jsonOptions = new();
    
    public UnixDateTimeConverterTests()
    {
        _jsonOptions.Converters.Add(new UnixDateTimeConverter());
    }
    
    [Theory]
    [InlineData(1658862600, "2022-07-26T19:10:00.0000000Z")] // DateTime string, Unix time
    [InlineData(1704105600, "2024-01-01T10:40:00.0000000Z")]
    [InlineData(1704105599, "2024-01-01T10:39:59.0000000Z")]
    public void Read_ConvertsUnixTimeToDateTime(long unixTime, string expected)
    {
        // Arrange
        var json = $"{{\"unixTime\": {unixTime}}}";
        using var doc = JsonDocument.Parse(json);
        var reader = doc.RootElement.GetProperty("unixTime").GetRawText();

        // Act
        var result = JsonSerializer.Deserialize<DateTime>(reader, _jsonOptions);

        // Assert
        Assert.Equal(DateTime.Parse(expected).ToUniversalTime(), result);
    }

    [Theory]
    [InlineData("2023-03-26T12:30:00Z", "2023-03-26T09:30Z")]
    [InlineData("2024-01-01T00:00:00Z", "2023-12-31T21:00Z")]
    [InlineData("2023-12-31T23:59:59Z", "2023-12-31T20:59Z")]
    public void Write_ConvertsDateTimeToFormattedString(string inputDateTimeString, string expected)
    {
        // Arrange
        var dateTime = DateTime.Parse(inputDateTimeString);
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        JsonSerializer.Serialize(writer, dateTime, _jsonOptions);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Equal(expected, json.Trim('"'));
    }
}