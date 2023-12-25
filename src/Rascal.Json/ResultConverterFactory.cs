using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// A factory for <see cref="ResultConverter{T}"/>.
/// </summary>
/// <remarks>
/// Please use <see cref="JsonSerializerOptionsExtensions.AddResultConverters"/>
/// to register this converter as it includes additional handling for the converter.
/// </remarks>
/// <seealso cref="ResultConverter{T}"/>
/// <seealso cref="JsonSerializerOptionsExtensions.AddResultConverters"/>
public sealed class ResultConverterFactory(ResultConverterOptions converterOptions) : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsConstructedGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var type = typeToConvert.GenericTypeArguments[0];
        var converterType = typeof(ResultConverter<>).MakeGenericType(type);
        var instance = (JsonConverter)Activator.CreateInstance(converterType, converterOptions)!;
        return instance;
    }
}
