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
    [Pure]
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
    [Pure]
    public static Result<T> NotNull<T>(this T? x, Error? error = null)
        where T : class =>
        x is not null
            ? new(x)
            : new(error ?? new NullError("Value was null."));

    /// <inheritdoc cref="NotNull{T}(T, Error)"/>
    [Pure]
    public static Result<T> NotNull<T>(this T? x, Error? error = null)
        where T : struct =>
        x is not null
            ? new(x.Value)
            : new(error ?? new NullError("Value was null."));

    /// <summary>
    /// Gets the value within the result,
    /// or <see langword="null"/> if the result does not contain a value.
    /// </summary>
    /// <remarks>
    /// This method differs from <see cref="Result{T}.GetValueOrDefault()"/> in that
    /// it specifically targets value types and returns <see langword="null"/> as opposed
    /// to the default value for the type in case the result does not contain a value.
    /// Can also be understood as mapping the value to <c>T?</c> and calling
    /// <see cref="Result{T}.GetValueOrDefault()"/> on the returned result.
    /// </remarks>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="result">The result to get the value in.</param>
    /// <returns>The value contained in <paramref name="result"/>,
    /// or <see langword="null"/> if <paramref name="result"/> does not contain a value.</returns>
    [Pure]
    public static T? GetValueOrNull<T>(this Result<T> result) where T : struct =>
        result.Map(x => (T?)x).GetValueOrDefault();

    /// <summary>
    /// Gets a result containing the value that is associated with the specified key in a dictionary,
    /// or an error if the key is not present.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the  dictionary.</typeparam>
    /// <param name="dict">The dictionary to try locate the key in.</param>
    /// <param name="key">The key to locate.</param>
    /// <returns>A result containing the value associated with <paramref name="key"/> in the dictionary,
    /// or an error if the key is not present.</returns>
    public static Result<TValue> TryGetValueR<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dict,
        TKey key,
        Error? error = null) =>
        dict.TryGetValue(key, out var x)
            ? new(x)
            : new(error
                ?? new NotFoundError($"Dictionary does not contain key '{key}'."));

    /// <summary>
    /// Gets a result containing the element at the specified index in the list,
    /// or an error if the index is out of range of the list.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="list">The list to index.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A result containing the value at <paramref name="index"/> in the list,
    /// or an error if the index is out of range of the list.</returns>
    public static Result<T> Index<T>(this IReadOnlyList<T> list, int index, Error? error = null) =>
        index < list.Count
            ? new(list[index])
            : new(error ?? new NotFoundError(
                   $"Index {index} is out of range for the list, " +
                   $"which has a count of {list.Count}."));
}
