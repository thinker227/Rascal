namespace Rascal;

/// <summary>
/// Class containing extensions for or relating to <see cref="Result{T}"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Takes a result containing another result and un-nests the inner result.
    /// </summary>
    /// <remarks>
    /// This operation is the same as applying <see cref="Result{T}.Then{TNew}"/>
    /// with the identity function, eg. <c>r.Then(x => x)</c>.
    /// </remarks>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="result">The result to un-nest.</param>
    public static Result<T> Unnest<T>(this Result<Result<T>> result) =>
        result.Then(x => x);

    /// <summary>
    /// Turns a nullable value into a result containing a non-null value
    /// or an error if the value is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// Note that this method has two variants:
    /// one for reference types and one for nullable value types.
    /// </remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="x">The value to turn into a result.</param>
    /// <param name="error">The error to return if the value is null.</param>
    /// <returns>A result containing <paramref name="x"/> if it is not <see langword="null"/>,
    /// or otherwise <paramref name="error"/>.</returns>
    public static Result<T> NotNull<T>(this T? x, Error? error = null)
        where T : class =>
        x is not null
            ? new(x)
            : new(error ?? new StringError("Value was null."));

    /// <inheritdoc cref="NotNull{T}(T, Error)"/>
    public static Result<T> NotNull<T>(this T? x, Error? error = null)
        where T : struct =>
        x is not null
            ? new(x.Value)
            : new(error ?? new StringError("Value was null."));
}
