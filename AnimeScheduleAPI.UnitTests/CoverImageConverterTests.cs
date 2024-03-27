using System.Text;
using System.Text.Json;
using AnimeScheduleAPI.Converters;

namespace AnimeScheduleAPI.Tests;

public class CoverImageConverterTests
{
    private readonly JsonSerializerOptions _jsonOptions = new();
    private readonly CoverImageConverter _converter = new();
    
    public CoverImageConverterTests()
    {
        _jsonOptions.Converters.Add(new CoverImageConverter());
    }

    [Theory]
    [InlineData("{\"extraLarge\":\"image.jpg\"}", "image.jpg")]
    [InlineData("{\"extraLarge\":\"\"}", "")]
    [InlineData("{}", null)]
    public void Read_ShouldExtractExtraLargeProperty(string json, string expected)
    {
        // Arrange

        // Act
        var result = JsonSerializer.Deserialize<string>(json, _jsonOptions);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("image.jpg", "\"image.jpg\"")]
    [InlineData("", "\"\"")]
    public void Write_ShouldSerializeStringValue(string value, string expectedJson)
    {
        // Arrange
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        _converter.Write(writer, value, _jsonOptions);
        writer.Flush();
        var result = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Equal(expectedJson, result);
    }
}