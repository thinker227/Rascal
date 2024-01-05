using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseGetValueOrForIdMatchAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.UseGetValueOrForIdMatch);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;
            
            var resultMembers = resultType.GetMembers();
            
            var matchMethod = (IMethodSymbol)resultMembers.First(x => x.Name == "Match");
            
            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IInvocationOperation)operationCtx.Operation;

                if (!operation.TargetMethod.OriginalDefinition.Equals(matchMethod, SymbolEqualityComparer.Default))
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
                    },
                    ..
                ]) return;
                
                // Check that the returned parameter is the same as the lambda parameter.
                if (!returnReference.Parameter.Equals(lambdaParameter, SymbolEqualityComparer.Default)) return;
                
                // Get the location of the method invocation.
                var location = operation.Syntax is InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax memberAccessExpression
                }
                    ? memberAccessExpression.Name.GetLocation()
                    : operation.Syntax.GetLocation();

                // Report the diagnostic.
                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UseGetValueOrForIdMatch,
                    location,
                    lambdaParameter.Name));
            }, OperationKind.Invocation);
        });
    }
}
