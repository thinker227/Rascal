using System.Diagnostics;

namespace Rascal;

/// <summary>
/// A type which contains either an ok value or an error.
/// </summary>
/// <typeparam name="T">The type of an ok value.</typeparam>
[DebuggerTypeProxy(typeof(ResultDebugProxy<>))]
public readonly partial struct Result<T>
{
    internal readonly T? value;
    internal readonly Error? error;

    internal Error Error => error ?? Error.DefaultValueError;

    /// <summary>
    /// Whether the result has an ok value or not.
    /// </summary>
    public bool IsOk { get; }

    /// <summary>
    /// Creates a new result with an ok value.
    /// </summary>
    /// <param name="value">The ok value.</param>
    public Result(T value)
    {
        IsOk = true;
        this.value = value;
        error = null;
    }

    /// <summary>
    /// Creates a new result with an error.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    public Result(Error error)
    {
        IsOk = false;
        value = default;
        this.error = error;
    }

    /// <summary>
    /// Gets a string representation of the result.
    /// </summary>
    [Pure]
    public override string ToString() =>
        IsOk
            ? $"Ok {{ {value} }}"
            : $"Error {{ {Error.Message} }}";
}
