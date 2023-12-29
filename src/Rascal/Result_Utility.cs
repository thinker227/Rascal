using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Rascal;

public readonly partial struct Result<T>
{
    /// <summary>
    /// Implicitly constructs a result from an ok value.
    /// </summary>
    /// <param name="value">The value to construct the result from.</param>
    [Pure]
    public static implicit operator Result<T>(T value) =>
        new(value);

    /// <summary>
    /// Combines the result with another result.
    /// </summary>
    /// <typeparam name="TOther">The type of the ok value in the other result.</typeparam>
    /// <param name="other">The result to combine the current result with.</param>
    /// <returns>A result containing a tuple of the ok value of the current result
    /// and the ok value of <paramref name="other"/>.
    /// If either of the results are errors, returns a result containing
    /// that error, or both errors if both results are errors.</returns>
    [Pure]
    public Result<(T first, TOther second)> Combine<TOther>(Result<TOther> other) =>
        (IsOk, other.IsOk) switch
    {
        (true, true) => new((value!, other.value!)),
        (false, true) => new(Error),
        (true, false) => new(other.Error),
        (false, false) => new(new AggregateError([Error, other.Error])),
    };

    /// <summary>
    /// Tries to convert the ok value of the result to another type.
    /// </summary>
    /// <typeparam name="TDerived">The type derived from <typeparamref name="T"/>
    /// to which to try convert the ok value.</typeparam>
    /// <param name="error">The error to return
    /// if the ok value cannot be converted to <typeparamref name="TDerived"/>.</param>
    /// <returns>A result which contains the ok value converted to <typeparamref name="TDerived"/>,
    /// <paramref name="error"/> if the ok value cannot be converted to <typeparamref name="TDerived"/>,
    /// or the error of the result if it is an error.</returns>
    [Pure]
    public Result<TDerived> ToType<TDerived>(string? error = null)
        where TDerived : T =>
        IsOk
            ? value is TDerived derived
                ? new(derived)
                : new(new ValidationError(
                    error ?? $"Value was not of type '{typeof(TDerived)}'.",
                    value))
            : new(Error);

#if NETCOREAPP
    /// <summary>
    /// Checks whether the ok value of the result matches a predicate,
    /// and if not replaces the value with an error.
    /// </summary>
    /// <param name="predicate">The predicate to match the ok value against.</param>
    /// <param name="getError">A function to produce an error
    /// if the ok value doesn't match the predicate.</param>
    /// <param name="expr">The caller argument expression for <paramref name="predicate"/>.
    /// Should not be specified explicitly, instead letting the compiler fill it in.</param>
    /// <returns>A result which contains the ok value of the original result
    /// if said value matches <paramref name="predicate"/>, otherwise an
    /// error produced by <paramref name="getError"/>.
    /// If the original result is an error, that error will be returned.</returns>
    [Pure]
    public Result<T> Validate(
        Func<T, bool> predicate,
        Func<T, Error>? getError = null,
        [CallerArgumentExpression(nameof(predicate))] string expr = "") =>
#else
    /// <summary>
    /// Checks whether the ok value in the result matches a predicate,
    /// and if not replaces the value with an error.
    /// </summary>
    /// <param name="predicate">The predicate to match the ok value against.</param>
    /// <param name="getError">A function to produce an error
    /// if the ok value doesn't match the predicate.</param>
    /// <returns>A result which contains the ok value of the original result
    /// if said value matches <paramref name="predicate"/>, otherwise an
    /// error produced by <paramref name="getError"/>.
    /// If the original result is an error, that error will be returned.</returns>
    [Pure]
    public Result<T> Validate(
        Func<T, bool> predicate,
        Func<T, Error>? getError = null) =>
#endif
        IsOk
            ? predicate(value!)
                ? this
                : new(getError?.Invoke(value!)
#if NETCOREAPP
                    ?? new ValidationError($"Value did not match predicate '{expr}'.", value))
#else
                    ?? new ValidationError("Value did not match predicate.", value))
#endif
            : new(Error);

#if NETCOREAPP
    /// <summary>
    /// Checks whether the ok value of the result matches a predicate,
    /// and if not replaces the value with an error.
    /// </summary>
    /// <param name="predicate">The predicate to match the ok value against.</param>
    /// <param name="expr">The caller argument expression for <paramref name="predicate"/>.
    /// Should not be specified explicitly, instead letting the compiler fill it in.</param>
    /// <returns>A result which contains the ok value of the original result
    /// if said value matches <paramref name="predicate"/>, otherwise an error.
    /// If the original result is an error, that error will be returned.</returns>
    [Pure]
    public Result<T> Where(
        Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string expr = "") =>
        Validate(predicate, null, expr);
#else
    /// <summary>
    /// Checks whether the ok value of the result matches a predicate,
    /// and if not replaces the value with an error.
    /// </summary>
    /// <param name="predicate">The predicate to match the ok value against.</param>
    /// <returns>A result which contains the ok value of the original result
    /// if said value matches <paramref name="predicate"/>, otherwise an error.
    /// If the original result is an error, that error will be returned.</returns>
    [Pure]
    public Result<T> Where(
        Func<T, bool> predicate) =>
        Validate(predicate);
#endif

    /// <summary>
    /// Creates a <see cref="IEnumerable{T}"/> from the result.
    /// </summary>
    /// <returns>A <see cref="IEnumerable{T}"/> containing either only the ok value of the result,
    /// or no values at all if the result is an error.</returns>
    [Pure]
    public IEnumerable<T> ToEnumerable() =>
        IsOk
            ? [value!]
            : [];

    /// <summary>
    /// Gets an immutable reference to the ok value of the result.
    /// </summary>
    /// <returns>An immutable reference to the ok value.
    /// The value of the reference might be <see langword="default"/> if the result is an error.</returns>
    [UnscopedRef]
    [Pure]
    public ref readonly T? AsRef() =>
        ref value;
}
