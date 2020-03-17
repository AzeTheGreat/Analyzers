using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace OniAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NeverUsedSuppressor : FieldSuppressor
    {
        private static readonly SuppressionDescriptor neverUsedSuppressor = new SuppressionDescriptor(
            id: "ONI-CS0169",
            suppressedDiagnosticId: "CS0169", // Never used
            justification: "Field is used through reflection.");

        protected override SuppressionDescriptor GetSuppressionDescriptor() => neverUsedSuppressor;
        protected override ImmutableArray<string> GetSuppressorAttributeName() => ImmutableArray.Create("MyCmpAdd", "MyCmpReq");
    }
}