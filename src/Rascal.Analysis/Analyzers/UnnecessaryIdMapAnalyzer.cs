using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryIdMapAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        Diagnostics.UnnecessaryIdMap);
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;
            
            var resultMembers = resultType.GetMembers();
            
            var mapMethod = (IMethodSymbol)resultMembers.First(x => x.Name == "Map");
            
            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IInvocationOperation)operationCtx.Operation;

                // Check that it is Map being called.
                if (!operation.TargetMethod.OriginalDefinition.Equals(mapMethod, SymbolEqualityComparer.Default))
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
                if (operation.Syntax is not InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Name.Span.Start: var start
                    },
                    Span.End: var end
                }) return;
                var span = TextSpan.FromBounds(start, end);
                var location = Location.Create(operation.Syntax.SyntaxTree, span);
                
                // Report the diagnostic.
                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnnecessaryIdMap,
                    location,
                    lambdaParameter.Name));
            }, OperationKind.Invocation);
        });
    }
}
