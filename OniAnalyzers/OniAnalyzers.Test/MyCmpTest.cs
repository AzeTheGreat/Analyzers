using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace OniAnalyzers.Test
{
    [TestClass]
    public class MyCmpTest : CodeFixVerifier
    {
        [TestMethod]
        public void TestMain()
        {
            var test = @"[MyCmpAdd] private object Test;";
            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MyCmpSuppressor();
    }
}
