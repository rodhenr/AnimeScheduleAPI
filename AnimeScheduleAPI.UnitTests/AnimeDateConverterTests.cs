using System.Text;
using System.Text.Json;
using AnimeScheduleAPI.Converters;

namespace AnimeScheduleAPI.Tests;

public class AnimeDateConverterTests
{
    private readonly JsonSerializerOptions _jsonOptions = new();

    public AnimeDateConverterTests()
    {
        _jsonOptions.Converters.Add(new AnimeDateConverter());
    }

    [Theory]
    [InlineData("{\"year\": 2024, \"month\": 3, \"day\": 26}", "2024-03-26T00:00:00")]
    public void Deserialize_ValidJson_ReturnsDateTime(string json, string expectedDateTimeString)
    {
        // Act
        var result = JsonSerializer.Deserialize<DateTime?>(json, _jsonOptions);

        // Assert
        Assert.Equal(DateTime.Parse(expectedDateTimeString), result);
    }

    [Theory]
    [InlineData("null")]
    public void Deserialize_NullJson_ReturnsNull(string json)
    {
        // Act
        var result = JsonSerializer.Deserialize<DateTime?>(json, _jsonOptions);

        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData("{\"year\": 2024, \"month\": 3}")]
    public void Deserialize_MissingRequiredProperties_ReturnsNull(string json)
    {
        // Act
        var result = JsonSerializer.Deserialize<DateTime?>(json, _jsonOptions);

        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData("{\"year\": null, \"month\": 3, \"day\": 26}")]
    public void Deserialize_NullProperty_ReturnsNull(string json)
    {
        // Act
        var result = JsonSerializer.Deserialize<DateTime?>(json, _jsonOptions);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("2024-03-26T00:00:00")]
    public void Serialize_DateTimeValue_ReturnsValidJson(string expectedJson)
    {
        // Arrange
        var dateTime = new DateTime(2024, 3, 26);

        // Act
        var json = JsonSerializer.Serialize(dateTime, _jsonOptions);

        // Assert
        Assert.Equal(expectedJson, json.Trim('"'));
    }

    [Theory]
    [InlineData("null")]
    public void Serialize_NullDateTime_ReturnsNullJson(string expectedJson)
    {
        // Arrange
        DateTime? dateTime = null;

        // Act
        var json = JsonSerializer.Serialize(dateTime, _jsonOptions);

        // Assert
        Assert.Equal(expectedJson, json);
    }
    
    [Theory]
    [InlineData("2024-03-26T00:00Z")]
    public void Serialize_DateTimeValue_WritesValidJsonString(string expectedJson)
    {
        // Arrange
        var dateTime = new DateTime(2024, 3, 26);
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        new AnimeDateConverter().Write(writer, dateTime, _jsonOptions);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Equal(expectedJson, json.Trim('"'));
    }

    [Theory]
    [InlineData("null")]
    public void Serialize_NullDateTime_WritesNullValue(string expectedJson)
    {
        // Arrange
        DateTime? dateTime = null;
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        new AnimeDateConverter().Write(writer, dateTime, _jsonOptions);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Equal(expectedJson, json);
    }
}