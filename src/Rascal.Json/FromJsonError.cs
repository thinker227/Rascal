namespace Rascal.Json;

/// <summary>
/// An error which 
/// </summary>
/// <param name="message"></param>
public sealed class FromJsonError(string message) : Error
{
    public override string Message => message;
}
