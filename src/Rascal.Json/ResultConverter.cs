﻿using System.Diagnostics;
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

        var ok = default(T);
        var readOk = false;

        var err = null as Error;
        var readErr = false;

        bool readOkFirst;
        
        if (reader.ValueTextEquals(okName))
        {
            reader.Read();
            
            ok = valueConverter.Read(ref reader, typeof(T), options);
            reader.Read();
            
            readOk = true;
            readOkFirst = true;
        }
        else if (reader.ValueTextEquals(errorName))
        {
            reader.Read();
            
            err = errorConverter.Read(ref reader, typeof(string), options);
            reader.Read();
            
            readErr = true;
            readOkFirst = false;
        }
        else throw new JsonException($"Expected property '{okName}' or '{errorName}'.");

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals(converterOptions.OkPropertyName))
            {
                reader.Read();
                
                if (readOk) throw new JsonException($"Duplicate '{okName}' properties.");
            
                ok = valueConverter.Read(ref reader, typeof(T), options);
                reader.Read();
                
                readOk = true;
            }
            else if (reader.ValueTextEquals(errorName))
            {
                reader.Read();
                
                if (readErr) throw new JsonException($"Duplicate '{errorName}' properties.");
            
                err = errorConverter.Read(ref reader, typeof(Error), options);
                reader.Read();
                
                readErr = true;
            }
            else throw new JsonException($"Expected property '{okName}' or '{errorName}'.");
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
    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        value.Switch(
            x =>
            {
                var converter = (JsonConverter<T>)options.GetConverter(typeof(T));
                
                writer.WritePropertyName(converterOptions.OkPropertyName);
                converter.Write(writer, x, options);
            },
            e =>
            {
                writer.WritePropertyName(converterOptions.ErrorPropertyName);
                writer.WriteStringValue(e.Message);
            }
        );
        
        writer.WriteEndObject();
    }
}
