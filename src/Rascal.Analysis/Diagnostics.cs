using Microsoft.CodeAnalysis;

namespace Rascal.Analysis;

public static class Diagnostics
{
    public static DiagnosticDescriptor DoNotThrow { get; } = new(
        "RASCAL0001",
        "Do not throw inside a result method",
        "Do not throw exceptions inside method '{0}' which returns '{1}'",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Methods which return results should not throw exceptions unless absolutely necessary. " +
            "Instead consider returning `Err<T>()` with a description of the error.");
}
