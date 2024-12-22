using Rascal.Errors;

namespace Rascal;

/// <summary>
/// An error containing a simple message.
/// Makes up the other half of a <see cref="Result{T}"/> which might be an error.
/// </summary>
/// <remarks>
/// An error is conceptually akin to an exception but without the ability to be thrown,
/// meant to be a more lightweight type meant to be wrapped in a <see cref="Result{T}"/>.
/// An error fundamentally only contains a single string message, however other more concrete types such as
/// <see cref="ExceptionError"/> or <see cref="AggregateError"/> may define other properties.
/// Errors are meant to be small, specific, and descriptive, such that they are easy to match over
/// and provide specific handling for specific kinds of errors.
/// </remarks>
public abstract class Error
{
    /// <summary>
    /// The message used to display the error.
    /// </summary>
    public abstract string Message { get; }

    /// <summary>
    /// A statically accessible default "Result has no value." error.
    /// </summary>
    internal static Error DefaultValueError { get; } = new StringError("Result has no value.");

    /// <summary>
    /// Gets a string representation of the error.
    /// Returns <see cref="Message"/> by default.
    /// </summary>
    public override string ToString() => Message;

    /// <summary>
    /// Implicitly converts a string into a <see cref="StringError"/>.
    /// </summary>
    /// <param name="message">The message of the error.</param>
    public static implicit operator Error(string message) =>
        new StringError(message);

    /// <summary>
    /// Implicitly converts an exception into an <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    public static implicit operator Error(Exception exception) =>
        new ExceptionError(exception);
}
