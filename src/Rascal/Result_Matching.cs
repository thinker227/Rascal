using System.Diagnostics.CodeAnalysis;

namespace Rascal;

public readonly partial struct Result<T>
{
    /// <summary>
    /// Matches over the ok value or error of the result and returns another value.
    /// Can be conceptualized as an exhaustive <c>switch</c> expression matching
    /// all possible values of the type.
    /// </summary>
    /// <typeparam name="TResult">The type to return from the match.</typeparam>
    /// <param name="ifOk">The function to apply to the ok value of the result if the result is ok.</param>
    /// <param name="ifError">The function to apply to the result's error if the result is an error.</param>
    /// <returns>The result of applying either
    /// <paramref name="ifOk"/> or <paramref name="ifError"/>
    /// on the ok value or error of the result.</returns>
    [Pure]
    public TResult Match<TResult>(Func<T, TResult> ifOk, Func<Error, TResult> ifError) =>
        IsOk
            ? ifOk(value!)
            : ifError(Error);

    /// <summary>
    /// Matches over the ok value or error of the result and invokes an effect-ful action onto the ok value or error.
    /// Can be conceptualized as an exhaustive <c>switch</c> statement matching all possible values of the type.
    /// </summary>
    /// <param name="ifOk">The function to call with the ok value of the result if the result is ok.</param>
    /// <param name="ifError">The function to call with the result's error if the result is an error.</param>
    public void Switch(Action<T> ifOk, Action<Error> ifError)
    {
        if (IsOk) ifOk(value!);
        else ifError(Error);
    }

    /// <summary>
    /// Asynchronously matches over the ok value or error of the result and returns another value.
    /// Can be conceptualized as an exhaustive <c>switch</c> expression matching
    /// all possible values of the type.
    /// </summary>
    /// <typeparam name="TResult">The type to return from the match.</typeparam>
    /// <param name="ifOk">The function to apply to the ok value of the result if the result is ok.</param>
    /// <param name="ifError">The function to apply to the result's error if the result is an error.</param>
    /// <returns>The result of applying either
    /// <paramref name="ifOk"/> or <paramref name="ifError"/>
    /// on the ok value or error of the result.</returns>
    [Pure]
    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> ifOk, Func<Error, Task<TResult>> ifError) =>
        IsOk
            ? await ifOk(value!)
            : await ifError(Error);

    /// <summary>
    /// Asynchronously matches over the ok value or error of the result
    /// and invokes an effect-ful action onto the ok value or error.
    /// Can be conceptualized as an exhaustive <c>switch</c> statement matching all possible values of the type.
    /// </summary>
    /// <param name="ifOk">The function to call with the ok value of the result if the result is ok.</param>
    /// <param name="ifError">The function to call with the result's error if the result is an error.</param>
    public async Task SwitchAsync(Func<T, Task> ifOk, Func<Error, Task> ifError)
    {
        if (IsOk) await ifOk(value!);
        else await ifError(Error);
    }

    /// <summary>
    /// Tries to get the ok value from the result.
    /// </summary>
    /// <param name="value">The ok value of the result.</param>
    /// <returns>Whether the result is ok.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
#else
    public bool TryGetValue(out T value)
#endif
    {
        value = this.value!;
        return IsOk;
    }

    /// <summary>
    /// Tries to get the ok value from the result.
    /// </summary>
    /// <param name="value">The ok value of the result.</param>
    /// <param name="error">The error of the result.</param>
    /// <returns>Whether the result is ok.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetValue([MaybeNullWhen(false)] out T value, [MaybeNullWhen(true)] out Error error)
#else
    public bool TryGetValue(out T value, out Error error)
#endif
    {
        value = this.value!;
        error = !IsOk
            ? Error
            : null!;
        
        return IsOk;
    }

    /// <summary>
    /// Tries to get an error from the result.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    /// <returns>Whether the result is an error.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetError([MaybeNullWhen(false)] out Error error)
#else
    public bool TryGetError(out Error error)
#endif
    {
        error = !IsOk
            ? Error
            : null!;

        return !IsOk;
    }

    /// <summary>
    /// Tries to get an error from the result.
    /// </summary>
    /// <param name="error">The error of the result.</param>
    /// <param name="value">The ok value of the result.</param>
    /// <returns>Whether the result is an error.</returns>
    [Pure]
#if NETCOREAPP
    public bool TryGetError([MaybeNullWhen(false)] out Error error, [MaybeNullWhen(true)] out T value)
#else
    public bool TryGetError(out Error error, out T value)
#endif
    {
        error = !IsOk
            ? Error
            : null!;
        value = this.value!;

        return !IsOk;
    }

    /// <summary>
    /// Gets the ok value of the result, or <see langword="default"/> if the result is not ok.
    /// </summary>
    [Pure]
    public T? GetValueOrDefault() =>
        IsOk
            ? value
            : default;

    /// <summary>
    /// Gets the ok value of the result, or a default value if the result is not ok.
    /// </summary>
    /// <param name="default">The default value to return if the result is not ok.</param>
    [Pure]
    public T GetValueOr(T @default) =>
        IsOk
            ? value!
            : @default;

    /// <summary>
    /// Gets the ok value of the result, or a default value if the result is not ok.
    /// </summary>
    /// <param name="getDefault">A function to get a default value to return
    /// if the result is not ok.</param>
    [Pure]
    public T GetValueOr(Func<T> getDefault) =>
        IsOk
            ? value!
            : getDefault();

    /// <summary>
    /// Unwraps the ok value of the result.
    /// Throws an <see cref="UnwrapException"/> if the result is not ok.
    /// </summary>
    /// <remarks>
    /// This API is <b>unsafe</b> in the sense that it might intentionally throw an exception.
    /// Please only use this API if the caller knows without a reasonable shadow of a doubt
    /// that this operation is safe, or that an exception is acceptable to be thrown.
    /// </remarks>
    /// <returns>The ok value of the result.</returns>
    /// <exception cref="UnwrapException">The result is not ok.</exception>
    [Pure]
    public T Unwrap() =>
        IsOk
            ? value!
            : throw new UnwrapException();

    /// <inheritdoc cref="Unwrap"/>
    [Pure]
    public static explicit operator T(Result<T> result) =>
        result.Unwrap();

    /// <summary>
    /// Expects the result to be ok, throwing an <see cref="UnwrapException"/>
    /// with a specified message if the result is not ok.
    /// </summary>
    /// <param name="error">The message to construct the <see cref="UnwrapException"/>
    /// to throw using if the result is not ok.</param>
    /// <remarks>
    /// This API is <b>unsafe</b> in the sense that it might intentionally throw an exception.
    /// Please only use this API if the caller knows without a reasonable shadow of a doubt
    /// that this operation is safe, or that an exception is acceptable to be thrown.
    /// </remarks>
    /// <returns>The ok value of the result.</returns>
    /// <exception cref="UnwrapException">The result is not ok.</exception>
    [Pure]
    public T Expect(string error) =>
        IsOk
            ? value!
            : throw new UnwrapException(error);
}
