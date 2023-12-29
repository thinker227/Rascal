using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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
                
                // Check that the first argument is a lambda with a single parameter.
                if (operation.Arguments is not
                [
                    {
                        Value: IDelegateCreationOperation
                        {
                            Target: IAnonymousFunctionOperation
                            {
                                Body: var body,
                                Symbol.Parameters:
                                [
                                    var lambdaParameter
                                ]
                            }
                        }
                    }
                ]) return;
                
                // Check that the body is a single return operation.
                if (body.Operations is not [IReturnOperation returnOperation]) return;

                // Check that the returned expression is a parameter reference.
                if (returnOperation.ReturnedValue is not IParameterReferenceOperation returnReference) return;

                // Check that the returned parameter is the same as the lambda parameter.
                if (!returnReference.Parameter.Equals(lambdaParameter, SymbolEqualityComparer.Default)) return;
                
                // Get the syntax of the method invocation.
                var invocationExpression = (InvocationExpressionSyntax)operation.Syntax;
                var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;
                var start = memberAccess.Name.Span.Start;
                var end = invocationExpression.ArgumentList.Span.End;
                var span = TextSpan.FromBounds(start, end);
                var location = Location.Create(invocationExpression.SyntaxTree, span);
                
                // Report the diagnostic.
                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UnnecessaryIdMap,
                    location,
                    lambdaParameter.Name));
            }, OperationKind.Invocation);
        });
    }
}
