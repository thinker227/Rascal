using System.Text.Json;

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
        
        var factory = new ResultConverterFactory(converterOptions);
        options.Converters.Add(factory);
        
        return options;
    }
}
