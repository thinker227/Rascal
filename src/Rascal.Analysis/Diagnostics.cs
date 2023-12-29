using Microsoft.CodeAnalysis;

namespace Rascal.Analysis;

public static class Diagnostics
{
    public static DiagnosticDescriptor UseMap { get; } = new(
        "RASCAL0001",
        "Use Map instead of Then and Ok",
        "Use Map instead of calling Ok directly inside Then",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling Ok directly inside a Then call is equivalent to calling Map. " +
        "Use Map instead for clarity and performance.");
    
    public static DiagnosticDescriptor DoNotThrow { get; } = new(
        "RASCAL0002",
        "Do not throw inside a result method or property",
        "Do not throw exceptions inside method or property '{0}' which returns '{1}'",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Methods or properties which return results should not throw exceptions unless absolutely necessary. " +
            "Instead consider returning `Err<T>()` with a description of the error.");
}
