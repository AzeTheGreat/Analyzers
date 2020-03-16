using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace HarmonyAnalyzers
{
    // Harmony methods require a [HarmonyPatch] attribute on their class

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AttributeAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor rule = new DiagnosticDescriptor(
            id: "HAR0002",
            title: "Class needs Harmony attribute",
            messageFormat: "Add [HarmonyPatch] to `{0}`",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var model = context.SemanticModel;

            // Return if class is already a Harmony class
            var classSymbol = model.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;

            if (classSymbol.IsHarmonyClass())
                return;

            // Iterate each method in the class and report if any are Harmony named
            var methods = context.Node.ChildNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var methodSymbol = model.GetDeclaredSymbol(method) as IMethodSymbol;
                if (methodSymbol.IsHarmonyMethodName())
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        rule,
                        context.Node.DescendantTokens().First(x => x.IsKind(SyntaxKind.IdentifierToken)).GetLocation(),
                        classSymbol.Name));
                    break;
                }
            }
        }
    }
}
