using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rascal.Analysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class RemoveMapIdCallCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.UnnecessaryIdMap.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        var document = ctx.Document;
        
        var root = await document.GetSyntaxRootAsync();
        if (root is null) return;

        var invocation = (InvocationExpressionSyntax)root.FindNode(ctx.Span);

        var codeAction = CodeAction.Create(
            "Remove Map call",
            _ => Task.FromResult(ExecuteFix(document, root, invocation)));
        
        ctx.RegisterCodeFix(codeAction, ctx.Diagnostics);
    }

    private static Document ExecuteFix(
        Document document,
        SyntaxNode root,
        InvocationExpressionSyntax invocation)
    {
        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
        var expression = memberAccess.Expression;
        var newRoot = root.ReplaceNode(invocation, expression);
        return document.WithSyntaxRoot(newRoot);
    }
}
