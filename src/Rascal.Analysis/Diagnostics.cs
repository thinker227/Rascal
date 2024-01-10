namespace Rascal.Analysis;

public static class Diagnostics
{
    private const string CorrectnessCategory = "Correctness";
    private const string AnalysisCategory = "Analysis";

    public static DiagnosticDescriptor UseMap { get; } = new(
        "RASCAL001",
        "Use 'Map' instead of 'Then(x => Ok(...))'",
        "Use 'Map' instead of calling 'Ok' directly inside 'Then'.",
        CorrectnessCategory,
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Ok' directly inside a 'Then' call is equivalent to calling 'Map'. " +
        "Use 'Map' instead for clarity and performance.");

    public static DiagnosticDescriptor UseThen { get; } = new(
        "RASCAL002",
        "Use 'Then' instead of 'Map(...).Unnest()'",
        "Use 'Then' instead of calling 'Unnest' directly after 'Map'.",
        CorrectnessCategory,
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Unnest' directly after a 'Map' call is equivalent to calling 'Then'. " +
        "Use 'Then' instead for clarity and performance.");

    public static DiagnosticDescriptor UnnecessaryIdMap { get; } = new(
        "RASCAL003",
        "Unnecessary 'Map' call with identity function",
        "This call maps {0} to itself. " +
        "The call can be safely removed because it doesn't do anything.",
        CorrectnessCategory,
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Map' with an identity function returns the same result as the input. " +
        "Remove this call to 'Map'.");

    public static DiagnosticDescriptor ToSameType { get; } = new(
        "RASCAL004",
        "'To' called with same type as result",
        "This call converts '{0}' to itself and will always succeed. " +
        "Remove this call to 'To' as it doesn't do anything.",
        CorrectnessCategory,
        DiagnosticSeverity.Warning,
        true,
        "Calling 'To' with the same type as that of the result will always succeed. " +
        "Remove the call as it doesn't do anything.");

    public static DiagnosticDescriptor ToImpossibleType { get; } = new(
        "RASCAL005",
        "'To' called with impossible type",
        "This call tries to convert '{0}' to '{1}', but no value of type '{0}' can be of type '{1}'. " +
        "The conversion will always fail.",
        CorrectnessCategory,
        DiagnosticSeverity.Warning,
        true,
        "Calling 'To' with a type which no value of the type of the result permits will always fail.");

    public static DiagnosticDescriptor UseGetValueOrForIdMatch { get; } = new(
        "RASCAL006",
        "Use 'GetValueOr' instead of 'Match(x => x, ...)'",
        "This call matches {0} using an identity function. " +
        "Use 'GetValueOr' instead to reduce allocations.",
        CorrectnessCategory,
        DiagnosticSeverity.Warning,
        true,
        "Calling 'Match' with an identity function for the 'ifOk' parameter is equivalent to calling 'GetValueOr'. " +
        "Replace this call with 'GetValueOr'.");

    public static DiagnosticDescriptor MissingSymbol { get; } = new(
        "RASCAL007",
        "Missing symbol required for analysis",
        "Cannot find type or member '{0}' which is required for analysis. " +
        "No analysis will be performed. " +
        "Verify that the library is referenced and that the version of the analyzer assembly matches that of the library. " +
        "Alternatively, this may be a bug and should be reported as such.",
        AnalysisCategory,
        DiagnosticSeverity.Warning,
        true);
}
