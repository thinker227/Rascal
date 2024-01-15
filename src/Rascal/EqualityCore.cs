namespace Rascal;

/// <summary>
/// Core equality implementations for <see cref="Result{T}"/>.
/// </summary>
internal static class EqualityCore
{
    internal static bool Equals<T>(Result<T> a, Result<T> b, IEqualityComparer<T> equalityComparer)
    {
        if (!a.IsOk || !b.IsOk) return !a.IsOk && !b.IsOk;
        if (a.value is null || b.value is null) return a.value is null && b.value is null;
        return equalityComparer.Equals(a.value, b.value);
    }

    internal static bool Equals<T>(Result<T> a, T? b, IEqualityComparer<T> equalityComparer)
    {
        if (!a.IsOk) return false;
        if (a.value is null || b is null) return a.value is null && b is null;
        return equalityComparer.Equals(a.value, b);
    }

    internal static int GetHashCode<T>(Result<T> result, IEqualityComparer<T> equalityComparer) =>
        result is { IsOk: true, value: not null }
            ? equalityComparer.GetHashCode(result.value)
            : 0;
}
