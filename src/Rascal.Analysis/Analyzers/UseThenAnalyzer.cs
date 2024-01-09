using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseThenAnalyzer : BaseAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.UseThen);

    protected override void Handle(CompilationStartAnalysisContext ctx, WellKnownSymbols symbols) => ctx.RegisterOperationAction(operationCtx =>
    {
        var operation = (IInvocationOperation)operationCtx.Operation;

        // Check that it is Unnest being called.
        if (!operation.TargetMethod.OriginalDefinition.Equals(symbols.UnnestMethod, SymbolEqualityComparer.Default))
            return;

        // Check that the first argument is an invocation.
        if (operation.Arguments is not
            [
                {
                    Value: IInvocationOperation argumentInvocation
                }
            ]) return;

        // Check that the invoked method is Map.
        if (!argumentInvocation.TargetMethod.OriginalDefinition
                .Equals(symbols.MapMethod, SymbolEqualityComparer.Default))
            return;

        // Get the location of the method name.
        var location = argumentInvocation.Syntax is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax memberAccessExpression
        }
            ? memberAccessExpression.Name.GetLocation()
            : argumentInvocation.Syntax.GetLocation();

        // Report the diagnostic.
        operationCtx.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.UseThen,
            location));
    }, OperationKind.Invocation);
}
