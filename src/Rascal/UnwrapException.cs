namespace Rascal;

/// <summary>
/// The exception thrown when a <see cref="Result{T}"/> is attempted to be unwrapped but is an error.
/// </summary>
public sealed class UnwrapException : Exception
{
    /// <summary>
    /// Creates a new <see cref="UnwrapException"/>.
    /// </summary>
    public UnwrapException() : base("Cannot unwrap result because it does not have a value.") {}

    /// <summary>
    /// Creates a new <see cref="UnwrapException"/>.
    /// </summary>
    /// <param name="error">An error message.</param>
    public UnwrapException(string error) : base(error) {}
}
