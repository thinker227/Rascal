using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseMapAnalyzer : BaseAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.UseMap);

    protected override void Handle(CompilationStartAnalysisContext ctx, WellKnownSymbols symbols) => ctx.RegisterOperationAction(operationCtx =>
    {
        var operation = (IInvocationOperation)operationCtx.Operation;

        // Check that it is Then being called.
        if (!operation.TargetMethod.OriginalDefinition.Equals(symbols.ThenMethod, SymbolEqualityComparer.Default)) return;

        // Check that the operation should be targeted.
        if (!IsTargetOperation(operation, symbols.OkMethod, symbols.OkCtor, symbols.OkConversion)) return;

        // Get the location of the method name.
        var location = operation.Syntax is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax memberAccessExpression
        }
            ? memberAccessExpression.Name.GetLocation()
            : operation.Syntax.GetLocation();
                
        // Report the diagnostic.
        operationCtx.ReportDiagnostic(Diagnostic.Create(
            Diagnostics.UseMap,
            location));
    }, OperationKind.Invocation);

    private static bool IsTargetOperation(
        IInvocationOperation operation,
        IMethodSymbol okMethod,
        IMethodSymbol okCtor,
        IMethodSymbol okConversion)
    {
        // Check that the first argument is a lambda with an immediate return.
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
                                    ReturnedValue: var returnValue
                                }
                            ]
                        }
                    }
                }
            ]) return false;

        // Check whether the return operation is an invocation to Ok.
        if (returnValue is IInvocationOperation invocation &&
            invocation.TargetMethod.OriginalDefinition.Equals(okMethod, SymbolEqualityComparer.Default)) return true;

        // Check whether the return operation is an constructor invocation to new(T).
        if (returnValue is IObjectCreationOperation objectCreation &&
            (objectCreation.Constructor?.OriginalDefinition.Equals(okCtor, SymbolEqualityComparer.Default)
             ?? false)) return true;

        // Check whether the return operation is either a target-type new or implicit/explicit conversion.
        // The conversion is implicit if it's a target-type new expression.
        if (returnValue is IConversionOperation conversion &&
            (conversion.IsImplicit ||
             (conversion.OperatorMethod?.OriginalDefinition.Equals(okConversion, SymbolEqualityComparer.Default) ?? false)))
            return true;
        
        return false;
    }
}
