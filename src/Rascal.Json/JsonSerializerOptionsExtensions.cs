using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// Extensions for <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds JSON converters for <see cref="Result{T}"/> values.
    /// </summary>
    /// <param name="options">The source serializer options.</param>
    /// <param name="configureOptions">A function to configure the options passed to the converters.</param>
    public static JsonSerializerOptions AddResultConverters(
        this JsonSerializerOptions options,
        Func<ResultConverterOptions, ResultConverterOptions>? configureOptions = null)
    {
        var converterOptions = new ResultConverterOptions();
        if (configureOptions is not null) converterOptions = configureOptions(converterOptions);
        
        options.Converters.Add(converterOptions.ErrorConverter);
        
        var factory = new ResultConverterFactory(converterOptions);
        options.Converters.Add(factory);
        
        return options;
    }

    internal static JsonConverter<T> GetConverter<T>(this JsonSerializerOptions options) =>
        (JsonConverter<T>)options.GetConverter(typeof(T));
}
