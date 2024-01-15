namespace Rascal;

/// <summary>
/// A <see cref="IEqualityComparer{T}"/> which compares equality between <see cref="Result{T}"/> values.
/// </summary>
/// <param name="valueComparer">The equality comparer used to compare ok values of results.</param>
/// <typeparam name="T">The type of an ok value of a result.</typeparam>
public sealed class ResultEqualityComparer<T>(IEqualityComparer<T> valueComparer) : IEqualityComparer<Result<T>>
{
    /// <summary>
    /// A <see cref="ResultEqualityComparer{T}"/> which uses
    /// <see cref="EqualityComparer{T}.Default"/> to compare ok values.
    /// </summary>
    public static ResultEqualityComparer<T> Default { get; } = new(EqualityComparer<T>.Default);

    /// <inheritdoc/>
    public bool Equals(Result<T> x, Result<T> y) =>
        EqualityCore.Equals(x, y, valueComparer);

    /// <inheritdoc/>
    public int GetHashCode(Result<T> obj) =>
        EqualityCore.GetHashCode(obj, valueComparer);
}
