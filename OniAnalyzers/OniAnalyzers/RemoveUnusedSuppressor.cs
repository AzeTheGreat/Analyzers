using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace OniAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RemoveUnusedSuppressor : FieldSuppressor
    {
        private static readonly SuppressionDescriptor unusedSuppresor = new SuppressionDescriptor(
            id: "ONI-IDE0051",
            suppressedDiagnosticId: "IDE0051", // Remove unused member
            justification: "Field is used through reflection.");

        protected override SuppressionDescriptor GetSuppressionDescriptor() => unusedSuppresor;
        protected override ImmutableArray<string> GetSuppressorAttributeName() => ImmutableArray.Create("MyCmpAdd", "MyCmpReq");
    }
}