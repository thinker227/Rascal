namespace Rascal;

/// <summary>
/// Extensions for or relating to <see cref="Result{T}"/>.
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
    /// <typeparam name="T">The type of the ok value in the result.</typeparam>
    /// <param name="result">The result to un-nest.</param>
    [Pure]
    public static Result<T> Unnest<T>(this Result<Result<T>> result) =>
        result.Then(x => x);

    /// <summary>
    /// Turns a nullable value into a result containing a non-null ok value
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
    /// Gets the ok value of the result,
    /// or <see langword="null"/> if the result is an error.
    /// </summary>
    /// <remarks>
    /// This method differs from <see cref="Result{T}.GetValueOrDefault()"/> in that
    /// it specifically targets value types and returns <see langword="null"/> as opposed
    /// to the default value for the type in case the result is an error.
    /// Can also be understood as mapping the value to <c>T?</c> and calling
    /// <see cref="Result{T}.GetValueOrDefault()"/> on the returned result.
    /// </remarks>
    /// <typeparam name="T">The type of the ok value of the result.</typeparam>
    /// <param name="result">The result to get the ok value of.</param>
    /// <returns>The ok value of <paramref name="result"/>,
    /// or <see langword="null"/> if <paramref name="result"/> is an error.</returns>
    [Pure]
    public static T? GetValueOrNull<T>(this Result<T> result) where T : struct =>
        result.Map(x => (T?)x).GetValueOrDefault();

    /// <summary>
    /// Turns a sequence of results into a single result containing the ok values in the results
    /// only if all the results are ok.
    /// Can also been seen as turning an <c>IEnumerable&lt;Result&lt;T&gt;&gt;</c> "inside out".
    /// </summary>
    /// <param name="results">The results to turn into a single sequence.</param>
    /// <typeparam name="T">The type of the ok values in the results.</typeparam>
    /// <returns>A single result containing a sequence of all the ok values from the original sequence of results,
    /// or the first error encountered within the sequence.</returns>
    /// <remarks>
    /// This method completely enumerates the input sequence before returning and is not lazy.
    /// As a consequence of this, the sequence within the returned result is an <see cref="IReadOnlyList{T}"/>.
    /// </remarks>
    public static Result<IReadOnlyList<T>> Sequence<T>(this IEnumerable<Result<T>> results)
    {
        var list = new List<T>();

        foreach (var result in results)
        {
            if (!result.TryGetValue(out var x, out var e)) return new(e);

            list.Add(x);
        }

        return list;
    }

    /// <summary>
    /// Gets a result containing the value that is associated with the specified key in a dictionary,
    /// or an error if the key is not present.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dict">The dictionary to try locate the key in.</param>
    /// <param name="key">The key to locate.</param>
    /// <param name="error">The error to return if the key is not present.</param>
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
    /// <param name="error">The error to return if the index is out of range.</param>
    /// <returns>A result containing the value at <paramref name="index"/> in the list,
    /// or an error if the index is out of range of the list.</returns>
    public static Result<T> Index<T>(this IReadOnlyList<T> list, int index, Error? error = null) =>
        index < list.Count
            ? new(list[index])
            : new(error ?? new NotFoundError(
                   $"Index {index} is out of range for the list, " +
                   $"which has a count of {list.Count}."));

    /// <summary>
    /// Catches the cancellation of a task and wraps the returned value or thrown exception in a result.
    /// </summary>
    /// <param name="task">The task to catch.</param>
    /// <param name="error">A function which produces an error in case of a cancellation.</param>
    /// <typeparam name="T">The type returned by the task.</typeparam>
    /// <returns>A result which contains either the value returned by awaiting <paramref name="task"/>,
    /// or an error in case the task is cancelled.</returns>
    public static async Task<Result<T>> CatchCancellation<T>(this Task<T> task, Func<Error>? error = null)
    {
        try
        {
            var value = await task;
            return new(value);
        }
        catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
        {
            return new(error?.Invoke() ?? new CancellationError());
        }
    }

#if NETCOREAPP
    /// <inheritdoc cref="CatchCancellation{T}(System.Threading.Tasks.Task{T},System.Func{Rascal.Error}?)"/>
    public static async Task<Result<T>> CatchCancellation<T>(this ValueTask<T> task, Func<Error>? error = null)
    {
        try
        {
            var value = await task;
            return new(value);
        }
        catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
        {
            return new(error?.Invoke() ?? new CancellationError());
        }
    }
#endif
}
