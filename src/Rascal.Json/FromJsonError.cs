namespace Rascal.Json;

/// <summary>
/// An error which has been deserialized from JSON. 
/// </summary>
/// <param name="message">The message describing the error.</param>
public sealed class FromJsonError(string message) : Error
{
    public override string Message => message;
}
