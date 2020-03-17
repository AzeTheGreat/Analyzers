using CustomAnalyzers.Interface;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NotAssignedSuppressor : FieldSuppressor
    {
        private static readonly SuppressionDescriptor notAssignedSuppressor = new SuppressionDescriptor(
            id: "CUS-CS0649",
            suppressedDiagnosticId: "CS0649", // Field not assigned, always null
            justification: "Field is assigned through reflection.");

        protected override SuppressionDescriptor GetSuppressionDescriptor() => notAssignedSuppressor;
        protected override string GetSuppressorAttributeName() => nameof(SuppressNotAssigned);
    }
}