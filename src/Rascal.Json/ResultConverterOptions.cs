using System.Text.Json.Serialization;

namespace Rascal.Json;

/// <summary>
/// A set of options for a <see cref="ResultConverter{T}"/>.
/// </summary>
public readonly struct ResultConverterOptions
{
    public ResultConverterOptions() {}
    
    /// <summary>
    /// What to prefer when deserializing a JSON object where both <c>ok</c> and <c>err</c> are present.
    /// The default is <see cref="ResultPropertyPreference.PreferOk"/>.
    /// </summary>
    public ResultPropertyPreference PropertyPreference { get; init; } = ResultPropertyPreference.PreferOk;

    /// <summary>
    /// The converter used to serialize <see cref="Error"/>s.
    /// </summary>
    public JsonConverter<Error> ErrorConverter { get; init; } = new DefaultErrorConverter();

    /// <summary>
    /// Sets what to prefer when deserializing a JSON object where both <c>ok</c> and <c>err</c> are present.
    /// </summary>
    /// <param name="propertyPreference">The preference to set.</param>
    /// <returns>A new <see cref="ResultConverterOptions"/> with <see cref="PropertyPreference"/> set.</returns>
    public ResultConverterOptions WithPropertyPreference(ResultPropertyPreference propertyPreference) =>
        this with { PropertyPreference = propertyPreference };

    /// <summary>
    /// Sets the converter used to serialize <see cref="Error"/>s.
    /// </summary>
    /// <param name="errorConverter">The converter to use.</param>
    /// <returns>A new <see cref="ResultConverterOptions"/> with <see cref="PropertyPreference"/> set.</returns>
    public ResultConverterOptions WithErrorConverter(JsonConverter<Error> errorConverter) =>
        this with { ErrorConverter = errorConverter };
}

/// <summary>
/// What to prefer when deserializing a JSON string where both <c>ok</c> and <c>err</c> are present.
/// </summary>
public enum ResultPropertyPreference
{
    /// <summary>
    /// Prefer <c>ok</c>.
    /// </summary>
    PreferOk,
    /// <summary>
    /// Prefer <c>err</c>.
    /// </summary>
    PreferErr,
    /// <summary>
    /// Prefer the first property encountered.
    /// </summary>
    First,
}
