using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryIdMapAnalyzer : BaseAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        Diagnostics.UnnecessaryIdMap);

    protected override void Handle(CompilationStartAnalysisContext ctx, WellKnownSymbols symbols) => ctx.RegisterOperationAction(operationCtx =>
    {
        var operation = (IInvocationOperation)operationCtx.Operation;

        // Check that it is Map being called.
        if (!operation.TargetMethod.OriginalDefinition.Equals(symbols.MapMethod, SymbolEqualityComparer.Default))
            return;
        
        // Check that the first argument is a lambda with a single parameter which immediately returns.
        if (operation.Arguments is not
        [
            {
                Value: IDelegateCreationOperation
                {
                    Target: IAnonymousFunctionOperation
                    {
                        Body.Operations:
                        [
                            IReturnOperation
                            {
                                ReturnedValue: IParameterReferenceOperation returnReference
                            }
                        ],
                        Symbol.Parameters: [var lambdaParameter]
                    }
                }
            }
        ]) return;

        // Check that the returned parameter is the same as the lambda parameter.
        if (!returnReference.Parameter.Equals(lambdaParameter, SymbolEqualityComparer.Default)) return;
        
        // Get the location of the method invocation.
        var location = operation.Syntax is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name.Span.Start: var start
            },
            Span.End: var end
        }
            ? Location.Create(operation.Syntax.SyntaxTree, TextSpan.FromBounds(start, end))
            : operation.Syntax.GetLocation();
        
        // Report the diagnostic.
        operationCtx.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.UnnecessaryIdMap,
            location,
            lambdaParameter.Name));
    }, OperationKind.Invocation);
}
