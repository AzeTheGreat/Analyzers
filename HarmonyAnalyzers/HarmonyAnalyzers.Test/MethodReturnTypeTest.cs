using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace HarmonyAnalyzers.Test
{
    [TestClass]
    public class MethodReturnTypeTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void TestAlreadyGood()
        {
            var test = @"
    [HarmonyPatch]
    class Test
    {
        static void Postfix() { }
    }";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestPredefinedType()
        {
            var test = @"
    [HarmonyPatch]
    class Test
    {
        static bool Postfix() { }
    }";
            var expected = new DiagnosticResult
            {
                Id = MethodReturnTypeAnalyzer.rule.Id,
                Message = string.Format(MethodReturnTypeAnalyzer.rule.MessageFormat.ToString(), "Postfix"),
                Severity = MethodReturnTypeAnalyzer.rule.DefaultSeverity,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 16) }
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

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestGenericName()
        {
            var test = @"
    [HarmonyPatch]
    class Test
    {
        static bool TargetMethods() { }
    }";
            var expected = new DiagnosticResult
            {
                Id = MethodReturnTypeAnalyzer.rule.Id,
                Message = string.Format(MethodReturnTypeAnalyzer.rule.MessageFormat.ToString(), "TargetMethods"),
                Severity = MethodReturnTypeAnalyzer.rule.DefaultSeverity,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 16) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    [HarmonyPatch]
    class Test
    {
        static IEnumerable<MethodBase> TargetMethods() { }
    }";
            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MethodReturnTypeAnalyzer();
        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MethodReturnTypeFix();
    }
}
