namespace Rascal.Json;

/// <summary>
/// A set of options for a <see cref="ResultConverter{T}"/>.
/// </summary>
public readonly struct ResultConverterOptions
{
    public ResultConverterOptions() {}
    
    /// <summary>
    /// What to prefer when deserializing a JSON string where both <c>ok</c> and <c>err</c> are present.
    /// The default is <see cref="ResultPropertyPreference.PreferOk"/>.
    /// </summary>
    public ResultPropertyPreference PropertyPreference { get; init; } = ResultPropertyPreference.PreferOk;
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
