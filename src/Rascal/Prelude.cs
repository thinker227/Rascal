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
    /// Creates a result containing an ok value.
    /// </summary>
    /// <typeparam name="T">The type of the ok value.</typeparam>
    /// <param name="value">The ok value to create the result from.</param>
    [Pure]
    public static Result<T> Ok<T>(T value) =>
        new(value);

    /// <summary>
    /// Creates a result containing an error.
    /// </summary>
    /// <typeparam name="T">The type of an ok value in the result.</typeparam>
    /// <param name="error">The error to create the result from.</param>
    [Pure]
    public static Result<T> Err<T>(Error error) =>
        new(error);

#if NET7_0_OR_GREATER // Support for generic math

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
    [Pure]
    public static Result<T> ParseR<T>(
        string? s,
        IFormatProvider? provider = null,
        Error? error = null)
        where T : IParsable<T> =>
        T.TryParse(s, provider, out var x)
            ? new(x)
            : new(error ?? new ParseError(
                $"Could not parse '{s}' to type {typeof(T)}.",
                s));

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
    [Pure]
    public static Result<T> ParseR<T>(
        ReadOnlySpan<char> s,
        IFormatProvider? provider = null,
        Error? error = null)
        where T : ISpanParsable<T> =>
        T.TryParse(s, provider, out var x)
            ? new(x)
            : new(error ?? new ParseError(
                $"Could not parse '{s}' to type {typeof(T)}.",
                s.ToString()));

#endif

    /// <summary>
    /// Tries to execute a function and return the result.
    /// If the function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <typeparam name="T">The type the function returns.</typeparam>
    /// <param name="function">The function to try execute.</param>
    /// <returns>A result containing the return value of the function
    /// or an <see cref="ExceptionError"/> containing the exception thrown by the function.</returns>
    [Pure]
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

    /// <summary>
    /// Applies a transform function onto a value until the function returns an error.
    /// </summary>
    /// <typeparam name="T">The type of the value transform.</typeparam>
    /// <param name="start">The starting value.</param>
    /// <param name="transform">A function which transforms a value into
    /// either a new value or an error.</param>
    /// <returns>A sequence of successful values produced by repeatedly applying
    /// <paramref name="transform"/> onto the previous value until an error is returned.
    /// Includes <paramref name="start"/> if it has a value,
    /// otherwise the sequence will be empty.</returns>
    [Pure]
    public static IEnumerable<T> Iterate<T>(Result<T> start, Func<T, Result<T>> transform)
    {
        var current = start;

        while (current.TryGetValue(out var x))
        {
            yield return x;
            current = transform(x);
        }
    }

    /// <summary>
    /// Applies a transform function onto a value until the function returns an error.
    /// </summary>
    /// <typeparam name="T">The type of the value transform.</typeparam>
    /// <param name="start">The starting value.</param>
    /// <param name="transform">A function which transforms a value into
    /// either a new value or an error.</param>
    /// <returns>A sequence of successful values produced by repeatedly applying
    /// <paramref name="transform"/> onto the previous value
    /// starting with <paramref name="start"/> until an error is returned.
    /// <paramref name="start"/> will always be the first element.</returns>
    [Pure]
    public static IEnumerable<T> Iterate<T>(T start, Func<T, Result<T>> transform) =>
        Iterate(Ok(start), transform);
}
