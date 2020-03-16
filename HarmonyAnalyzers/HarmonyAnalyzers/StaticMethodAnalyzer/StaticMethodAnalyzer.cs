using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace HarmonyAnalyzers
{
    // Harmony methods must be static.
    // Checks if a method is in a harmony class, has a harmony name, and is not yet static.

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class StaticMethodAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor rule = new DiagnosticDescriptor(
            id: "HAR0001",
            title: "Harmony methods must be static",
            messageFormat: "Make method `{0}` static to be used by Harmony.",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as IMethodSymbol;

            if (symbol.IsInHarmonyClass() && symbol.IsHarmonyMethodName() && !symbol.IsStatic)
                context.ReportDiagnostic(Diagnostic.Create(
                    rule,
                    context.Node.DescendantTokens().First(x => x.IsKind(SyntaxKind.IdentifierToken)).GetLocation(),
                    symbol.Name));
        }
    }
}
