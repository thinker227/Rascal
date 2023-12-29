using System.Diagnostics.CodeAnalysis;

namespace Rascal;

public readonly partial struct Result<T>
{
    /// <summary>
    /// Matches over the value or error of the result and returns another value.
    /// Can be conceptualized as an exhaustive <c>switch</c> expression matching
    /// all possible values of the type.
    /// </summary>
    /// <typeparam name="TResult">The type to return from the match.</typeparam>
    /// <param name="ifValue">The function to apply to the
    /// value of the result if it does contain a value.</param>
    /// <param name="ifError">The function to apply to the
    /// result's error if it does not contain a value.</param>
    /// <returns>The result of applying either
    /// <paramref name="ifValue"/> or <paramref name="ifError"/>
    /// on the value or error of the result.</returns>
    [Pure]
    public TResult Match<TResult>(Func<T, TResult> ifValue, Func<Error, TResult> ifError) =>
        HasValue
            ? ifValue(value!)
            : ifError(Error);

    /// <summary>
    /// Matches over the value or error of the result and invokes an effectful action onto the value or error.
    /// Can be conceptualized as an exhaustive <c>switch</c> statement matching
    /// all possible values of the type.
    /// </summary>
    /// <param name="ifValue">The function to call with the
    /// value of the result if it does contain a value.</param>
    /// <param name="ifError">The function to call with the
    /// result's error if it does not contain a value.</param>
    public void Switch(Action<T> ifValue, Action<Error> ifError)
    {
        if (HasValue) ifValue(value!);
        else ifError(Error);
    }

    /// <summary>
    /// Tries to get the value from the result.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <returns>Whether the result contains a value.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
#else
    public bool TryGetValue(out T value)
#endif
    {
        value = this.value!;
        return HasValue;
    }

    /// <summary>
    /// Tries to get the value from the result.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <param name="error">The error of the result.</param>
    /// <returns>Whether the result contains a value.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetValue([MaybeNullWhen(false)] out T value, [MaybeNullWhen(true)] out Error error)
#else
    public bool TryGetValue(out T value, out Error error)
#endif
    {
        value = this.value!;
        error = !HasValue
            ? Error
            : null!;
        
        return HasValue;
    }

    /// <summary>
    /// Tries to get an error from the result.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    /// <returns>Whether the result contains an error.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetError([MaybeNullWhen(false)] out Error error)
#else
    public bool TryGetError(out Error error)
#endif
    {
        error = !HasValue
            ? Error
            : null!;

        return !HasValue;
    }

    /// <summary>
    /// Tries to get an error from the result.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    /// <param name="value">The value of the result.</param>
    /// <returns>Whether the result contains an error.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetError([MaybeNullWhen(false)] out Error error, [MaybeNullWhen(true)] out T value)
#else
    public bool TryGetError(out Error error, out T value)
#endif
    {
        error = !HasValue
            ? Error
            : null!;
        value = this.value!;

        return !HasValue;
    }

    /// <summary>
    /// Gets the value within the result,
    /// or <see langword="default"/> if the result does not contain a value.
    /// </summary>
    [Pure]
    public T? GetValueOrDefault() =>
        HasValue
            ? value
            : default;

    /// <summary>
    /// Gets the value within the result,
    /// or a default value if the result does not contain a value.
    /// </summary>
    /// <param name="default">The default value to return if the result does not contain a value.</param>
    [Pure]
    public T GetValueOrDefault(T @default) =>
        HasValue
            ? value!
            : @default;

    /// <summary>
    /// Gets the value within the result,
    /// or a default value if the result does not contain a value.
    /// </summary>
    /// <param name="getDefault">A function to get a default value to return
    /// if the result does not contain a value.</param>
    [Pure]
    public T GetValueOrDefault(Func<T> getDefault) =>
        HasValue
            ? value!
            : getDefault();

    /// <summary>
    /// Unwraps the value in the result.
    /// Throws an <see cref="UnwrapException"/> if the result does not contain a value.
    /// </summary>
    /// <remarks>
    /// This API is <b>unsafe</b> in the sense that it might intentionally throw an exception.
    /// Please only use this API if the caller knows without a reasonable shadow of a doubt
    /// that this operation is safe, or that an exception is acceptable to be thrown.
    /// </remarks>
    /// <returns>The value in the result.</returns>
    /// <exception cref="UnwrapException">The result does not contain a value.</exception>
    [Pure]
    public T Unwrap() =>
        HasValue
            ? value!
            : throw new UnwrapException();

    /// <inheritdoc cref="Unwrap"/>
    [Pure]
    public static explicit operator T(Result<T> result) =>
        result.Unwrap();

    /// <summary>
    /// Expects the result to contain a value, throwing an <see cref="UnwrapException"/>
    /// with a specified message if the result does not contain a value.
    /// </summary>
    /// <param name="error">The message to construct the <see cref="UnwrapException"/>
    /// to throw using if the result does not contain a value.</param>
    /// <remarks>
    /// This API is <b>unsafe</b> in the sense that it might intentionally throw an exception.
    /// Please only use this API if the caller knows without a reasonable shadow of a doubt
    /// that this operation is safe, or that an exception is acceptable to be thrown.
    /// </remarks>
    /// <returns>The value in the result.</returns>
    /// <exception cref="UnwrapException">The result does not contain a value.</exception>
    [Pure]
    public T Expect(string error) =>
        HasValue
            ? value!
            : throw new UnwrapException(error);
}
