namespace Rascal;

internal sealed class ResultDebugProxy<T>(Result<T> result)
{
    public bool HasValue { get; } = result.HasValue;

    public object? Value { get; } = result.HasValue
        ? result.value!
        : result.Error;
}
