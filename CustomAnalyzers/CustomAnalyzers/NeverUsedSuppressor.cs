using CustomAnalyzers.Interface;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NeverUsedSuppressor : FieldSuppressor
    {
        private static readonly SuppressionDescriptor neverUsedSuppressor = new SuppressionDescriptor(
            id: "CUS-CS0169",
            suppressedDiagnosticId: "CS0169", // Never used
            justification: "Field is used through reflection.");

        protected override SuppressionDescriptor GetSuppressionDescriptor() => neverUsedSuppressor;
        protected override string GetSuppressorAttributeName() => nameof(SuppressRemoveUnused);
    }
}