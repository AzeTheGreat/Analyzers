using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CustomAnalyzers
{
    public abstract class FieldSuppressor : DiagnosticSuppressor
    {
        protected abstract SuppressionDescriptor GetSuppressionDescriptor();
        protected abstract string GetSuppressorAttributeName();

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(GetSuppressionDescriptor());

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                if (!(GetSymbolForDiagnostic(diagnostic, context) is IFieldSymbol symbol))
                    return;

                foreach (var attribute in symbol.GetAttributes())
                {
                    if (IsRightAttribute(attribute) || attribute.AttributeClass.GetAttributes().Any(x => IsRightAttribute(x)))
                        context.ReportSuppression(Suppression.Create(GetSuppressionDescriptor(), diagnostic));
                }

                bool IsRightAttribute(AttributeData attribute) => attribute.AttributeClass.Name == GetSuppressorAttributeName();
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