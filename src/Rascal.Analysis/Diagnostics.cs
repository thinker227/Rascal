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
}
