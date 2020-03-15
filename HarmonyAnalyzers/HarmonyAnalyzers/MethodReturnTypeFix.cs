using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HarmonyAnalyzers
{
    // TODO: Add code fixes for each possible return type not just a default (?)
    // TODO: Add correct usings for return types if neded

    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class MethodReturnTypeFix : CodeFixProvider
    {
        // The name as it will appear in the light bulb menu
        private const string title = "Change return type";

        // The list of rules the code fix can handle
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MethodReturnTypeAnalyzer.rule.Id);
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => FixAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> FixAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var model = await document.GetSemanticModelAsync();
            if (!model.GetDeclaredSymbol(declaration).TryGetHarmonyMethodType(out var harmonyMethodType))
                return document;

            var newNode = declaration.WithReturnType(harmonyMethodType.getReturnTypeSyntax());

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(declaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}