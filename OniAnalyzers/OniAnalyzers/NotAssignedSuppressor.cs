using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace OniAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NotAssignedSuppressor : FieldSuppressor
    {
        private static readonly SuppressionDescriptor notAssignedSuppressor = new SuppressionDescriptor(
            id: "ONI-CS0649",
            suppressedDiagnosticId: "CS0649", // Field not assigned, always null
            justification: "Field is assigned through reflection.");

        protected override SuppressionDescriptor GetSuppressionDescriptor() => notAssignedSuppressor;
        protected override ImmutableArray<string> GetSuppressorAttributeName() => ImmutableArray.Create("MyCmpAdd", "MyCmpReq", "MyCmpGet");
    }
}