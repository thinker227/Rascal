namespace Rascal;

/// <summary>
/// An abstract error.
/// </summary>
public abstract class Error
{
    /// <summary>
    /// The message used to display the error.
    /// </summary>
    public abstract string Message { get; }

    /// <summary>
    /// Implicitly converts a string into a <see cref="StringError"/>.
    /// </summary>
    /// <param name="message">The message of the error.</param>
    public static implicit operator Error(string message) =>
        new StringError(message);
}

/// <summary>
/// An error which displays a simple string.
/// </summary>
/// <param name="message">The message to display.</param>
public sealed class StringError(string message) : Error
{
    public override string Message => message;

    /// <summary>
    /// A statically accessible default "Result has no value." error.
    /// </summary>
    internal static StringError DefaultError { get; } = new("Result has no value.");
}

/// <summary>
/// An error which is constructed from an exception.
/// </summary>
/// <param name="exception">The exception in the error.</param>
public sealed class ExceptionError(Exception exception) : Error
{
    public Exception Exception { get; } = exception;

    /// <summary>
    /// The exception in the error.
    /// </summary>
    public override string Message => Exception.Message;
}

/// <summary>
/// An error which is a combination of other errors.
/// </summary>
public sealed class AggregateError(IReadOnlyCollection<Error> errors) : Error
{
    public IReadOnlyCollection<Error> Errors { get; } = errors;

    public override string Message => string.Join("\n", Errors.Select(x => x.Message));
}
