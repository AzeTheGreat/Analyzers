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

namespace Harmony
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class AttributeFix : CodeFixProvider
    {
        // The name as it will appear in the light bulb menu
        private const string title = "Add [HarmonyPatch]";

        // The list of rules the code fix can handle
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AttributeAnalyzer.rule.Id);
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => FixAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> FixAsync(Document document, ClassDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var newAttributes = declaration.AttributeLists.Add(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("HarmonyPatch")))));
            var newNode = declaration.WithAttributeLists(newAttributes);

            // Replace the old local declaration with the new local declaration.
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(declaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}