using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace HarmonyAnalyzers.Test
{
    [TestClass]
    public class StaticMethodTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void TestBlank()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMain()
        {
            var test = @"
    [HarmonyPatch]
    class Test
    {
        void Postfix() { }
    }";
            var expected = new DiagnosticResult
            {
                Id = StaticMethodAnalyzer.rule.Id,
                Message = string.Format(StaticMethodAnalyzer.rule.MessageFormat.ToString(), "Postfix"),
                Severity = StaticMethodAnalyzer.rule.DefaultSeverity,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 14) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    [HarmonyPatch]
    class Test
    {
    static void Postfix() { }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new StaticMethodAnalyzer();
        protected override CodeFixProvider GetCSharpCodeFixProvider() => new StaticMethodFix();
    }
}
