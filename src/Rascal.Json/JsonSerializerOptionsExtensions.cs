using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// Extensions for <see cref="JsonSerializerOptions"/>.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds JSON converters for serializing and deserializing <see cref="Result{T}"/> values.
    /// <br/><br/>
    /// Results are serialized as objects.
    /// By default, ok values will be serialized with the property name <c>Ok</c>,
    /// and error values with the name <c>Err</c>,
    /// however this can be configured using
    /// <see cref="ResultConverterOptions.OkPropertyName"/> and
    /// <see cref="ResultConverterOptions.ErrorPropertyName"/>
    /// in the object passed to <paramref name="configureOptions"/>.
    /// <br/><br/>
    /// When serializing, the JSON object will only contain the property representing
    /// the state of the result, i.e. only <i>either</i> the <c>Ok</c> or <c>Err</c> property by default.
    /// When deserializing, however, both properties may be present,
    /// in which case the ok property will be chosen by default, but this can be configured using
    /// <see cref="ResultConverterOptions.PropertyPreference"/>.
    /// <br/><br/>
    /// Errors serialized and deserialized to/from JSON use <see cref="Error.Message"/>
    /// and serialize/deserialize as JSON strings. When deserialized, errors are uniformly
    /// replaced with <see cref="FromJsonError"/>.
    /// </summary>
    /// <param name="options">The source serializer options.</param>
    /// <param name="configureOptions">A function to configure the options passed to the converters.</param>
    /// <returns>The same serializer options.</returns>
    /// <seealso cref="ResultConverterOptions"/>
    /// <seealso cref="FromJsonError"/>
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
