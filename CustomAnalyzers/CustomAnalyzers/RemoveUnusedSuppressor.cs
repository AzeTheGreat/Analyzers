using CustomAnalyzers.Interface;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RemoveUnusedSuppressor : FieldSuppressor
    {
        private static readonly SuppressionDescriptor unusedSuppresor = new SuppressionDescriptor(
            id: "CUS-IDE0051",
            suppressedDiagnosticId: "IDE0051", // Remove unused member
            justification: "Field is used through reflection.");

        protected override SuppressionDescriptor GetSuppressionDescriptor() => unusedSuppresor;
        protected override string GetSuppressorAttributeName() => nameof(SuppressRemoveUnused);
    }
}