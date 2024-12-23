using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// Converts <see cref="Result{T}"/> values to and from JSON.
/// </summary>
/// <typeparam name="T">The type of the value in the result.</typeparam>
/// <remarks>
/// This type should not itself be registered as a JSON converter
/// because it is not general over all results, merely a single type.
/// Instead use <see cref="JsonSerializerOptionsExtensions.AddResultConverters"/>
/// to register converters for <see cref="Result{T}"/>.
/// </remarks>
/// <seealso cref="ResultConverterFactory"/>
/// <seealso cref="JsonSerializerOptionsExtensions.AddResultConverters"/>
public sealed class ResultConverter<T>(ResultConverterOptions converterOptions) : JsonConverter<Result<T>> 
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected '{'.");

        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException("Expected property name.");

        var valueConverter = options.GetConverter<T>();
        var errorConverter = options.GetConverter<Error>();

        var okName = converterOptions.OkPropertyName;
        var errorName = converterOptions.ErrorPropertyName;
        var comparison = options.PropertyNameCaseInsensitive
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.InvariantCulture;

        var ok = default(T);
        var readOk = false;

        var err = null as Error;
        var readErr = false;

        bool readOkFirst;

        // This could potentially be done in a better way which doesn't allocate a string
        // for the property name, since ReadOnlySpan<char>.Equals *does* support StringComparison.
        var propertyName = reader.GetString()!;
        reader.Read();
        
        if (propertyName.Equals(okName, comparison))
        {
            ok = valueConverter.Read(ref reader, typeof(T), options);
            reader.Read();
            
            readOk = true;
            readOkFirst = true;
        }
        else if (propertyName.Equals(errorName, comparison))
        {
            err = errorConverter.Read(ref reader, typeof(string), options);
            reader.Read();
            
            readErr = true;
            readOkFirst = false;
        }
        else throw new JsonException(GetExpectedPropertyMessage());

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
            propertyName = reader.GetString()!;
            reader.Read();
            
            if (propertyName.Equals(okName, comparison))
            {
                if (readOk) throw new JsonException($"Duplicate '{okName}' properties.");
            
                ok = valueConverter.Read(ref reader, typeof(T), options);
                reader.Read();
                
                readOk = true;
            }
            else if (propertyName.Equals(errorName, comparison))
            {
                if (readErr) throw new JsonException($"Duplicate '{errorName}' properties.");
            
                err = errorConverter.Read(ref reader, typeof(Error), options);
                reader.Read();
                
                readErr = true;
            }
            else throw new JsonException(GetExpectedPropertyMessage());
        }

        if (reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException("Expected '}'.");

        var result = (readOk, readErr) switch
        {
            (true, false) => new Result<T>(ok!),
            (false, true) => new Result<T>(err!),
            (true, true) => converterOptions.PropertyPreference switch
            {
                ResultPropertyPreference.PreferOk => new Result<T>(ok!),
                ResultPropertyPreference.PreferErr => new Result<T>(err!),
                ResultPropertyPreference.First => readOkFirst
                    ? new Result<T>(ok!)
                    : new Result<T>(err!),
                _ => throw new UnreachableException(),
            },
            _ => throw new UnreachableException(),
        };

        return result;

        string GetExpectedPropertyMessage() =>
            options.PropertyNameCaseInsensitive
                ? $"Expected property '{okName}' or '{errorName}' (case-insensitive)."
                : $"Expected property '{okName}' or '{errorName}'.";
    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        value.Switch(
            x =>
            {
                var converter = (JsonConverter<T>)options.GetConverter(typeof(T));

                var okName = options.PropertyNamingPolicy is not null
                    ? options.PropertyNamingPolicy.ConvertName(converterOptions.OkPropertyName)
                    : converterOptions.OkPropertyName;
                writer.WritePropertyName(okName);
                converter.Write(writer, x, options);
            },
            e =>
            {
                var errorName = options.PropertyNamingPolicy is not null
                    ? options.PropertyNamingPolicy.ConvertName(converterOptions.ErrorPropertyName)
                    : converterOptions.ErrorPropertyName;
                writer.WritePropertyName(errorName);
                writer.WriteStringValue(e.Message);
            }
        );
        
        writer.WriteEndObject();
    }
}
