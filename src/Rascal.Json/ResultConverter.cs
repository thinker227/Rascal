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

        var valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));
        var errorConverter = (JsonConverter<string>)options.GetConverter(typeof(string));

        var ok = default(T);
        var readOk = false;

        var err = null as string;
        var readErr = false;

        bool readOkFirst;
        
        if (reader.ValueTextEquals("ok"))
        {
            reader.Read();
            
            ok = valueConverter.Read(ref reader, typeof(T), options);
            reader.Read();
            
            readOk = true;
            readOkFirst = true;
        }
        else if (reader.ValueTextEquals("err"))
        {
            reader.Read();
            
            err = errorConverter.Read(ref reader, typeof(string), options);
            reader.Read();
            
            readErr = true;
            readOkFirst = false;
        }
        else throw new JsonException("Expected property 'ok' or 'err'.");

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
            if (reader.ValueTextEquals("ok"))
            {
                reader.Read();
                
                if (readOk) throw new JsonException("Duplicate 'ok' properties.");
            
                ok = valueConverter.Read(ref reader, typeof(T), options);
                reader.Read();
                
                readOk = true;
            }
            else if (reader.ValueTextEquals("err"))
            {
                reader.Read();
                
                if (readErr) throw new JsonException("Duplicate 'err' properties.");
            
                err = errorConverter.Read(ref reader, typeof(string), options);
                reader.Read();
                
                readErr = true;
            }
            else throw new JsonException("Expected property 'ok' or 'err'.");
        }

        if (reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException("Expected '}'.");

        reader.Read();

        var result = (readOk, readErr) switch
        {
            (true, false) => new Result<T>(ok!),
            (false, true) => new Result<T>(new StringError(err!)),
            (true, true) => converterOptions.PropertyPreference switch
            {
                ResultPropertyPreference.PreferOk => new Result<T>(ok!),
                ResultPropertyPreference.PreferErr => new Result<T>(new StringError(err!)),
                ResultPropertyPreference.First => readOkFirst
                    ? new Result<T>(ok!)
                    : new Result<T>(new StringError(err!)),
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
                
                writer.WritePropertyName("ok");
                converter.Write(writer, x, options);
            },
            e =>
            {
                writer.WritePropertyName("err");
                writer.WriteStringValue(e.Message);
            }
        );
        
        writer.WriteEndObject();
    }
}