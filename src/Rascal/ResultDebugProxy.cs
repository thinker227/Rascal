namespace Rascal;

internal sealed class ResultDebugProxy<T>(Result<T> result)
{
    public bool IsOk { get; } = result.IsOk;

    public object? Value { get; } = result.IsOk
        ? result.value!
        : result.Error;
}
