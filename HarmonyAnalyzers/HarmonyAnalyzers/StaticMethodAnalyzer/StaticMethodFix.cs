// add this "using static" to easily create syntax nodes
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace HarmonyAnalyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class StaticMethodFix : CodeFixProvider
    {
        // The name as it will appear in the light bulb menu
        private const string title = "Make method static";

        // The list of rules the code fix can handle
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StaticMethodAnalyzer.rule.Id);
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => FixAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> FixAsync(Document document, MethodDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var newModifiers = declaration.Modifiers.Add(Token(SyntaxKind.StaticKeyword));
            var newNode = declaration.WithModifiers(newModifiers);

            // Replace the old local declaration with the new local declaration.
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(declaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}