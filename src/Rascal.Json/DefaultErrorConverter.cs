using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// The default converter for <see cref="Error"/> values.
/// Uses <see cref="Error.Message"/> to read from and write to JSON.
/// Error types are erased and uniformly replaced with <see cref="FromJsonError"/>.
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
