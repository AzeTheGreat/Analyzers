using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace HarmonyAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MethodReturnTypeAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor rule = new DiagnosticDescriptor(
            id: "HAR0003",
            title: "Harmony method requires specific return type",
            messageFormat: "Change return type of `{0}`",
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
            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as IMethodSymbol;

            if (methodSymbol.TryGetHarmonyMethodType(out var harmonyMethodType))
            {
                if (!harmonyMethodType.isReturnTypeValid(methodSymbol))
                    context.ReportDiagnostic(Diagnostic.Create(
                       rule,
                       context.Node.ChildNodes().First(x => x.IsKind(SyntaxKind.PredefinedType)).GetLocation(),
                       methodSymbol.Name));
            }
        }
    }
}
