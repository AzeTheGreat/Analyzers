using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace OniAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MyCmpSuppressor : DiagnosticSuppressor
    {
        private static readonly SuppressionDescriptor unusedSuppresor = new SuppressionDescriptor(
            id: "ONI-IDE0051",
            suppressedDiagnosticId: "IDE0051", // Remove unused member
            justification: "Field is used through reflection.");

        private static readonly SuppressionDescriptor alwaysNullSuppressor = new SuppressionDescriptor(
            id: "ONI-CS0649",
            suppressedDiagnosticId: "CS0649", // Never assigned to, always null
            justification: "Field is set through reflection.");

        private static readonly SuppressionDescriptor neverUsedSuppressor = new SuppressionDescriptor(
            id: "ONI-CS0169",
            suppressedDiagnosticId: "CS0169",
            justification: "Field is used through reflection.");

        private static readonly List<string> attributesSuppressUnused = new List<string> { "MyCmpAdd", "MyCmpReq" };
        private static readonly List<string> attributesSuppressAlwaysNull = new List<string> { "MyCmpAdd", "MyCmpReq", "MyCmpGet" };

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(unusedSuppresor, alwaysNullSuppressor, neverUsedSuppressor);

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                if (!(GetSymbolForDiagnostic(diagnostic, context) is IFieldSymbol symbol))
                    return;

                var attributes = symbol.GetAttributes().Select(x => x.AttributeClass.Name);

                if (diagnostic.Id == unusedSuppresor.SuppressedDiagnosticId && attributes.Intersect(attributesSuppressUnused).Any())
                    context.ReportSuppression(Suppression.Create(unusedSuppresor, diagnostic));
                else if (diagnostic.Id == alwaysNullSuppressor.SuppressedDiagnosticId && attributes.Intersect(attributesSuppressAlwaysNull).Any())
                    context.ReportSuppression(Suppression.Create(alwaysNullSuppressor, diagnostic));
                else if (diagnostic.Id == neverUsedSuppressor.SuppressedDiagnosticId && attributes.Intersect(attributesSuppressUnused).Any())
                    context.ReportSuppression(Suppression.Create(neverUsedSuppressor, diagnostic));
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