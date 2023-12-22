using System.Diagnostics.CodeAnalysis;

namespace Rascal;

/// <summary>
/// A type which contains either a successful value or an error.
/// </summary>
/// <typeparam name="T">The type of a successful value.</typeparam>
public readonly partial struct Result<T>
{
    internal readonly T? value;
    internal readonly Error? error;

    internal Error Error => error ?? StringError.DefaultError;

    /// <summary>
    /// Whether the result has a value or not.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Creates a new result with a successful value.
    /// </summary>
    /// <param name="value">The successful value.</param>
    public Result(T value)
    {
        HasValue = true;
        this.value = value;
        error = null;
    }

    /// <summary>
    /// Creates a new result with an error.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    public Result(Error error)
    {
        HasValue = false;
        value = default;
        this.error = error;
    }

    [Pure]
    public override string ToString() =>
        HasValue
            ? $"Ok {{ {value} }}"
            : $"Error {{ {Error.Message} }}";
}
