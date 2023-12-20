namespace Rascal;

public readonly partial struct Result<T>
{
    /// <summary>
    /// Implicitly constructs a result from a value.
    /// </summary>
    /// <param name="value">The value to construct the result from.</param>
    public static implicit operator Result<T>(T value) =>
        new(value);

    /// <summary>
    /// Combines the result with another result.
    /// </summary>
    /// <typeparam name="TOther">The type of the value in the other result.</typeparam>
    /// <param name="other">The result to combine the current result with.</param>
    /// <returns>A result containing a tuple of the value
    /// of the current result and the value of <paramref name="other"/>.
    /// If either of the results do contain an error, returns a result containing
    /// that error, or both errors if both results contain errors.</returns>
    public Result<(T first, TOther second)> Combine<TOther>(Result<TOther> other) =>
        (hasValue, other.hasValue) switch
    {
        (true, true) => new((value!, other.value!)),
        (false, true) => new(Error),
        (true, false) => new(other.Error),
        (false, false) => new(new AggregateError([Error, other.Error])),
    };

    /// <summary>
    /// Tries to convert the result value to another type.
    /// </summary>
    /// <typeparam name="TDerived">The type derived from <typeparamref name="T"/>
    /// to which to try convert the value.</typeparam>
    /// <param name="error">The error to return </param>
    /// <returns>A result which contains the current result's value converted to
    /// <typeparamref name="TDerived"/>, </returns>
    public Result<TDerived> ToType<TDerived>(string? error = null)
        where TDerived : T =>
        hasValue
            ? value is TDerived derived
                ? new(derived)
                : new(new StringError(
                    error ?? $"Value was not of type '{typeof(TDerived)}'."))
            : new(Error);

    /// <summary>
    /// Checks whether the value in the result matches a predicate,
    /// and if not replaces the value with an error.
    /// </summary>
    /// <param name="predicate">The predicate to match the value against.</param>
    /// <param name="getError">A function to produce an error
    /// if the value doesn't match the predicate.</param>
    /// <returns>A result which contains the value of the original result
    /// if the value matches <paramref name="predicate"/>, otherwise an
    /// error produced by <paramref name="getError"/>.
    /// If the original result does not contain a value, that result will be returned.</returns>
    public Result<T> ErrorIf(Func<T, bool> predicate, Func<T, Error> getError) =>
        hasValue
            ? predicate(value!)
                ? new(value!)
                : new(getError(value!))
            : new(Error);

    /// <summary>
    /// Creates a <see cref="IEnumerable{T}"/> from the result.
    /// </summary>
    /// <returns>A <see cref="IEnumerable{T}"/>
    /// containing either only the value of the result,
    /// or no values at all if the result does not contain a value</returns>
    public IEnumerable<T> ToEnumerable() =>
        hasValue
            ? [value!]
            : [];
}
