using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseMapAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.UseMap);
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;

            var preludeType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Prelude");
            if (preludeType is null) return;

            var resultMembers = resultType.GetMembers();
            var preludeMembers = preludeType.GetMembers();

            var thenMethod = (IMethodSymbol)resultMembers.First(x => x.Name == "Then");
            var okMethod = (IMethodSymbol)preludeMembers.First(x => x.Name == "Ok");
            var okCtor = resultType.InstanceConstructors
                .First(x => x.Parameters is [{ Type: ITypeParameterSymbol }]);
            var okConversion = (IMethodSymbol)resultMembers
                .OfType<IMethodSymbol>()
                .First(x => x is { Name: "op_Implicit", Parameters: [{ Type: ITypeParameterSymbol }] });
            
            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IInvocationOperation)operationCtx.Operation;

                // Check that it is Then being called.
                if (!operation.TargetMethod.OriginalDefinition.Equals(thenMethod, SymbolEqualityComparer.Default)) return;

                // Check that the operation should be targeted.
                if (!IsTargetOperation(operation, okMethod, okCtor, okConversion)) return;

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
        });
    }

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
