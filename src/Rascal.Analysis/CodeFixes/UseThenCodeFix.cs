using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class UseThenCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.UseThen.Id);
    
    public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        var document = ctx.Document;
        var semanticModel = await document.GetSemanticModelAsync(ctx.CancellationToken);
        if (semanticModel is null) return;
        
        var root = await document.GetSyntaxRootAsync(ctx.CancellationToken);
        if (root is null) return;
        
        var invocation = root.FindNode(ctx.Span).FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocation is null) return;
        
        // This operation structure will be the same regardless of
        // whether Unnest is called as an extension or regular method.
        if (semanticModel.GetOperation(invocation) is not IInvocationOperation
            {
                Parent: IArgumentOperation
                {
                    Parent: IInvocationOperation unnestOperation
                }
            } mapOperation) return;

        var mapSyntax = (InvocationExpressionSyntax)mapOperation.Syntax;
        var unnestSyntax = (InvocationExpressionSyntax)unnestOperation.Syntax;

        if (mapSyntax is not
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Name: var nameSyntax
                }
            }) return;

        var codeAction = CodeAction.Create(
            "Use Then",
            _ => Task.FromResult(ExecuteFix(
                document,
                root,
                unnestSyntax,
                mapSyntax,
                nameSyntax)),
            nameof(UseThenCodeFix));
        
        ctx.RegisterCodeFix(codeAction, ctx.Diagnostics);
    }

    private static Document ExecuteFix(
        Document document,
        SyntaxNode root,
        ExpressionSyntax containingInvocation,
        InvocationExpressionSyntax invocation,
        NameSyntax name)
    {
        var newInvocation = invocation.ReplaceNode(name, SyntaxInator.ThenName());
        var newRoot = root.ReplaceNode(containingInvocation, newInvocation);
        return document.WithSyntaxRoot(newRoot);
    }
}
