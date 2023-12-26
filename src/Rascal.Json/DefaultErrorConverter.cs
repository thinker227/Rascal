using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// The default converter for <see cref="Error"/> values.
/// </summary>
public sealed class DefaultErrorConverter : JsonConverter<Error>
{
    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string.");
        
        var message = reader.GetString();

        return new FromJsonError(message!);
    }

    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Message);
}
