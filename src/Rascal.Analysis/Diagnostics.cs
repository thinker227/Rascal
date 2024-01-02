namespace Rascal.Analysis;

public static class Diagnostics
{
    public static DiagnosticDescriptor UseMap { get; } = new(
        "RASCAL0001",
        "Use 'Map' instead of 'Then(x => Ok(...))'",
        "Use 'Map' instead of calling 'Ok' directly inside 'Then'",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Ok' directly inside a 'Then' call is equivalent to calling 'Map'. " +
        "Use 'Map' instead for clarity and performance.");

    public static DiagnosticDescriptor UseThen { get; } = new(
        "RASCAL0002",
        "Use Then instead of 'Map(...).Unnest()'",
        "Use 'Then' instead of calling 'Unnest' directly after 'Map'",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Unnest' directly after a 'Map' call is equivalent to calling 'Then'. " +
        "Use 'Then' instead for clarity and performance.");

    public static DiagnosticDescriptor UnnecessaryIdMap { get; } = new(
        "RASCAL0003",
        "Unnecessary 'Map' call with identity function",
        "This call maps {0} to itself. " +
        "The call can be safely removed because it doesn't do anything",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Map' with an identity function returns the same result as the input. " +
        "Remove this call to 'Map'.");

    public static DiagnosticDescriptor ToSameType { get; } = new(
        "RASCAL0004",
        "'To' called with same type as result",
        "This call converts '{0}' to itself and will always succeed. " +
        "Remove this call to 'To' as it doesn't do anything.",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling 'To' with the same type as that of the result will always succeed.");

    public static DiagnosticDescriptor ToImpossibleType { get; } = new(
        "RASCAL0005",
        "'To' called with impossible type",
        "This call tries to convert '{0}' to '{1}', but no value of type '{0}' can be of type '{1}'. " +
        "The conversion will always fail.",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling 'To' with a type which no value of the type of the result permits will always fail.");

    public static DiagnosticDescriptor UseDefaultOrForIdMatch { get; } = new(
        "RASCAL0006",
        "Use 'DefaultOr' instead of 'Match(x => x, ...)'",
        "This call matches {0} using an identity function. " +
        "Use 'DefaultOr' instead to reduce allocations.",
        "Correctness",
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Match' with an identity function for the 'ifOk' parameter is equivalent to 'DefaultOr'.");
}
