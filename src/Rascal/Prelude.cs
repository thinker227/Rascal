using System.Numerics;

namespace Rascal;

/// <summary>
/// A class containing various utility methods, a 'prelude' to the rest of the library.
/// </summary>
/// <remarks>
/// This class is meant to be imported statically, eg. <c>using static Rascal.Prelude;</c>.
/// Recommended to be imported globally via a global using statement.
/// </remarks>
public static class Prelude
{
    /// <summary>
    /// Creates a result containing a value.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="value">The value to create the result from.</param>
    public static Result<T> Ok<T>(T value) =>
        new(value);

    /// <summary>
    /// Creates a result containing an error.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="error">The error to create the result from.</param>
    public static Result<T> Err<T>(Error error) =>
        new(error);

    /// <summary>
    /// Tries to parse a string into a value,
    /// returning a result indicating whether the operation succeeded.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An object that provides culture-specific
    /// formatting information about <paramref name="s"/>.</param>
    /// <param name="error">The error produced if the string cannot be parsed.</param>
    /// <returns>A result containing either the successfully parsed value
    /// or <paramref name="error"/> if the string could not be parsed.</returns>
    public static Result<T> ParseR<T>(
        string? s,
        IFormatProvider? provider = null,
        Error? error = null)
        where T : IParsable<T> =>
        T.TryParse(s, provider, out var x)
            ? new(x)
            : new(error ?? new StringError(
                $"Could not parse '{s}' to type {typeof(T)}."));

    /// <summary>
    /// Tries to parse a span of characters into a value,
    /// returning a result indicating whether the operation succeeded.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">An object that provides culture-specific
    /// formatting information about <paramref name="s"/>.</param>
    /// <param name="error">The error produced if the string cannot be parsed.</param>
    /// <returns>A result containing either the successfully parsed value
    /// or <paramref name="error"/> if the string could not be parsed.</returns>
    public static Result<T> ParseR<T>(
        ReadOnlySpan<char> s,
        IFormatProvider? provider = null,
        Error? error = null)
        where T : ISpanParsable<T> =>
        T.TryParse(s, provider, out var x)
            ? new(x)
            : new(error ?? new StringError(
                $"Could not parse '{s}' to type {typeof(T)}."));

    /// <summary>
    /// Tries to execute a function and return the result.
    /// If the function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <typeparam name="T">The type the function returns.</typeparam>
    /// <param name="function">The function to try execute.</param>
    /// <returns>A result containing the return value of the function
    /// or an <see cref="ExceptionError"/> containing the exception thrown by the function.</returns>
    public static Result<T> Try<T>(Func<T> function)
    {
        try
        {
            return new(function());
        }
        catch (Exception e)
        {
            return new(new ExceptionError(e));
        }
    }
}
