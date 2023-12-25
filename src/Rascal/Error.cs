using System.Collections.Immutable;

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
/// <param name="errors">The errors the error consists of.</param>
public sealed class AggregateError(IEnumerable<Error> errors) : Error
{
    /// <summary>
    /// The errors the error consists of.
    /// </summary>
    public ImmutableArray<Error> Errors { get; } = errors.ToImmutableArray();

    public override string Message => string.Join("\n", Errors.Select(x => x.Message));
}

/// <summary>
/// An error which represents something which isn't found.
/// </summary>
/// <param name="message">A message which describes the thing which wasn't found.</param>
public sealed class NotFoundError(string message) : Error
{
    public override string Message => message;
}

/// <summary>
/// An error which represents an invalid operation.
/// </summary>
/// <param name="message">An optional message describing the operation and why it is invalid.</param>
public sealed class InvalidOperationError(string? message = null) : Error
{
    public override string Message => message ?? "Invalid operation.";
}

/// <summary>
/// An error which represents something which is <see langword="null"/>.
/// </summary>
/// <param name="message">An optional message describing the thing which is <see langword="null"/>.</param>
public sealed class NullError(string? message = null) : Error
{
    public override string Message => message ?? "Value is null.";
}

/// <summary>
/// An error which represents a validation failure on some object.
/// </summary>
/// <param name="message">A message describing the validation.</param>
/// <param name="source">The optional object which failed validation.</param>
public sealed class ValidationError(string message, object? source) : Error
{
    /// <summary>
    /// The optional object which failed validation.
    /// </summary>
    public object? Source => source;

    public override string Message => message;
}

/// <summary>
/// An error which represents a failure to parse something.
/// </summary>
/// <param name="message">A message describing the failure.</param>
/// <param name="source">The optional object which was attempted to be parsed.</param>
public sealed class ParseError(string message, object? source) : Error
{
    /// <summary>
    /// The optional object which was attempted to be parsed.
    /// </summary>
    public object? Source => source;

    public override string Message => message;
}

/// <summary>
/// An error which represents the cancellation of a task.
/// </summary>
/// <param name="message">The optional message describing the task cancellation.</param>
public sealed class CancellationError(string? message = null) : Error
{
    public override string Message => message ?? "Task was cancelled.";
}
