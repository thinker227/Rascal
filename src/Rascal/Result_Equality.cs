using System.Numerics;

namespace Rascal;

public readonly partial struct Result<T> :
    IEquatable<T>,
    IEquatable<Result<T>>,
    IEqualityOperators<Result<T>, Result<T>, bool>,
    IEqualityOperators<Result<T>, T, bool>
{
    public bool Equals(T? other) =>
        HasValue && (other?.Equals(value) ?? value is null);

    public bool Equals(Result<T> other) =>
        HasValue && other.HasValue && (other.value?.Equals(value) ?? value is null) ||
        !HasValue && !other.HasValue;

    public override bool Equals(object? other) =>
        other is T x && Equals(x) ||
        other is Result<T> r && Equals(r);

    public override int GetHashCode() =>
        HasValue
            ? value?.GetHashCode() ?? 0
            : 0;

    public static bool operator ==(Result<T> a, Result<T> b) =>
        a.Equals(b);
    
    public static bool operator !=(Result<T> a, Result<T> b) =>
        !a.Equals(b);

    public static bool operator ==(Result<T> a, T? b) =>
        a.Equals(b);

    public static bool operator !=(Result<T> a, T? b) =>
        !a.Equals(b);
}
