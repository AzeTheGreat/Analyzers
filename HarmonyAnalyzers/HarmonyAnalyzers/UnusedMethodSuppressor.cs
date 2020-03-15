using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace HarmonyAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UnusedMethodSuppressor : DiagnosticSuppressor
    {
        private static readonly SuppressionDescriptor suppressionDescriptor = new SuppressionDescriptor(
            id: "HAR-IDE0051",
            suppressedDiagnosticId: "IDE0051", // Remove unused member
            justification: "Member is used from the Harmony library.");

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(suppressionDescriptor);

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                var symbol = GetSymbolForDiagnostic(diagnostic, context) as IMethodSymbol;

                // If method is a Harmony method, suppress the warning.
                if (symbol.IsHarmonyMethod())
                    context.ReportSuppression(Suppression.Create(suppressionDescriptor, diagnostic));
            }
        }

        private ISymbol GetSymbolForDiagnostic(Diagnostic diagnostic, SuppressionAnalysisContext context)
        {
            var syntaxTree = diagnostic.Location.SourceTree;
            var nodeWithWarning = syntaxTree.GetRoot().FindNode(diagnostic.Location.SourceSpan);
            var semanticModel = context.GetSemanticModel(syntaxTree);
            return semanticModel.GetDeclaredSymbol(nodeWithWarning);
        }
    }
}