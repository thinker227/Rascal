using Microsoft.CodeAnalysis;

namespace Rascal.Analysis;

public static class Diagnostics
{
    public static DiagnosticDescriptor UseMap { get; } = new(
        "RASCAL0001",
        "Use Map instead of Then(x => Ok(...))",
        "Use Map instead of calling Ok directly inside Then",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling Ok directly inside a Then call is equivalent to calling Map. " +
        "Use Map instead for clarity and performance.");

    public static DiagnosticDescriptor UseThen { get; } = new(
        "RASCAL0002",
        "Use Then instead of Map(...).Unnest()",
        "Use Then instead of calling Unnest directly after Map",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling Unnest directly after a Map call is equivalent to calling Then. " +
        "Use Then instead for clarity and performance.");
}
