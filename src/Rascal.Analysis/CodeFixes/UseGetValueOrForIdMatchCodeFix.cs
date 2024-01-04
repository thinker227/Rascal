using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class UseGetValueOrForIdMatchCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.UseGetValueOrForIdMatch.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        var document = ctx.Document;
        var semanticModel = await document.GetSemanticModelAsync(ctx.CancellationToken);
        if (semanticModel is null) return;
        
        var root = await document.GetSyntaxRootAsync(ctx.CancellationToken);
        if (root is null) return;
        
        var invocationSyntax = root.FindNode(ctx.Span).FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocationSyntax is not { Expression: MemberAccessExpressionSyntax memberAccessSyntax }) return;
        
        if (semanticModel.GetOperation(invocationSyntax, ctx.CancellationToken) is not IInvocationOperation operation) return;
        if (operation.Arguments is not
        [
            _,
            {
                Value: IDelegateCreationOperation
                {
                    Target: IAnonymousFunctionOperation
                    {
                        Body: var body,
                        Symbol.Parameters: [var param]
                    } lambda
                }
            }
        ]) return;

        if (lambda.Syntax is not LambdaExpressionSyntax lambdaSyntax) return;

        var codeAction = CodeAction.Create(
            "Use GetValueOr",
            _ => Task.FromResult(ExecuteFix(
                document,
                root,
                invocationSyntax,
                memberAccessSyntax,
                lambdaSyntax,
                body,
                param)),
            nameof(UseGetValueOrForIdMatchCodeFix));
        
        ctx.RegisterCodeFix(codeAction, ctx.Diagnostics);
    }

    private static Document ExecuteFix(
        Document document,
        SyntaxNode root,
        InvocationExpressionSyntax invocationExpression,
        MemberAccessExpressionSyntax memberAccessSyntax,
        LambdaExpressionSyntax lambdaSyntax,
        IBlockOperation body,
        IParameterSymbol param)
    {
        var discard = !body
            .Descendants()
            .OfType<IParameterReferenceOperation>()
            .Any(p => p.Parameter.Equals(param, SymbolEqualityComparer.Default));
        
        var newInvocationExpression = discard
            ? body.Operations is [IReturnOperation { ReturnedValue.ConstantValue.HasValue: true } ret]
                ? SyntaxInator.GetValueOrConstant(
                    memberAccessSyntax.Expression,
                    (ExpressionSyntax)ret.ReturnedValue.Syntax)
                : SyntaxInator.GetValueOrDiscardLambda(memberAccessSyntax.Expression, lambdaSyntax)
            : SyntaxInator.GetValueOrParameterizedLambda(memberAccessSyntax.Expression, lambdaSyntax);

        var newRoot = root.ReplaceNode(invocationExpression, newInvocationExpression);

        return document.WithSyntaxRoot(newRoot);
    }
}
