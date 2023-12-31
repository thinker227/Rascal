namespace Rascal;

public readonly partial struct Result<T> :
    IEquatable<T>,
    IEquatable<Result<T>>
#if NET7_0_OR_GREATER
    ,
    System.Numerics.IEqualityOperators<Result<T>, Result<T>, bool>,
    System.Numerics.IEqualityOperators<Result<T>, T, bool>
#endif
{
    /// <summary>
    /// Checks whether the result is ok and the ok value is equal to another value.
    /// </summary>
    /// <param name="other">The value to check for equality with the ok value of the result.</param>
    [Pure]
    public bool Equals(T? other) =>
        EqualityCore.Equals(this, other, EqualityComparer<T>.Default);

    /// <summary>
    /// Checks whether the result is ok and the ok value is equal to another value
    /// using a specified equality comparer.
    /// </summary>
    /// <param name="other">The value to check for equality with the ok value of the result.</param>
    /// <param name="valueComparer">The equality comparer to use for comparing values.</param>
    [Pure]
    public bool Equals(T? other, IEqualityComparer<T> valueComparer) =>
        EqualityCore.Equals(this, other, valueComparer);

    /// <summary>
    /// Checks whether the result is equal to another result.
    /// Results are equal if both results are ok and the ok values are equal,
    /// or if both results are errors.
    /// </summary>
    /// <param name="other">The result to check for equality with the current result.</param>
    [Pure]
    public bool Equals(Result<T> other) =>
        EqualityCore.Equals(this, other, EqualityComparer<T>.Default);

    /// <summary>
    /// Checks whether the result is equal to another result using a specified equality comparer.
    /// Results are equal if both results are ok and the ok values are equal,
    /// or if both results are errors.
    /// </summary>
    /// <param name="other">The result to check for equality with the current result.</param>
    /// <param name="valueComparer">The equality comparer to use for comparing values.</param>
    [Pure]
    public bool Equals(Result<T> other, IEqualityComparer<T> valueComparer) =>
        EqualityCore.Equals(this, other, valueComparer);

    /// <inheritdoc/>
    [Pure]
    public override bool Equals(object? other) =>
        other is T x && Equals(x) ||
        other is Result<T> r && Equals(r);

    /// <inheritdoc/>
    [Pure]
    public override int GetHashCode() =>
        EqualityCore.GetHashCode(this, EqualityComparer<T>.Default);

    /// <summary>
    /// Checks whether two results are equal.
    /// Results are equal if both results are ok and the ok values are equal,
    /// or if both results are errors.
    /// </summary>
    /// <param name="a">The first result to compare.</param>
    /// <param name="b">The second result to compare.</param>
    [Pure]
    public static bool operator ==(Result<T> a, Result<T> b) =>
        a.Equals(b);
    
    /// <summary>
    /// Checks whether two results are not equal.
    /// Results are equal if both results are ok and the ok values are equal,
    /// or if both results are errors.
    /// </summary>
    /// <param name="a">The first result to compare.</param>
    /// <param name="b">The second result to compare.</param>
    [Pure]
    public static bool operator !=(Result<T> a, Result<T> b) =>
        !a.Equals(b);

    /// <summary>
    /// Checks whether a result is ok and the ok value is equal to another value.
    /// </summary>
    /// <param name="a">The result to compare.</param>
    /// <param name="b">The value to check for equality with the ok value in the result.</param>
    [Pure]
    public static bool operator ==(Result<T> a, T? b) =>
        a.Equals(b);
    
    /// <summary>
    /// Checks whether a result either does not have a value, or the value is not equal to another value.
    /// </summary>
    /// <param name="a">The result to compare.</param>
    /// <param name="b">The value to check for inequality with the ok value in the result.</param>
    [Pure]
    public static bool operator !=(Result<T> a, T? b) =>
        !a.Equals(b);
}
