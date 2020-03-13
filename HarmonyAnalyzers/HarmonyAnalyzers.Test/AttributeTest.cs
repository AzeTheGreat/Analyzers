using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Harmony.Test
{
    [TestClass]
    public class AttributeTest : CodeFixVerifier
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
using System;

class Test
{
    static void Postfix() { }
}

class HarmonyPatch : Attribute { }";
            var expected = new DiagnosticResult
            {
                Id = AttributeAnalyzer.rule.Id,
                Message = string.Format(AttributeAnalyzer.rule.MessageFormat.ToString(), "Test"),
                Severity = AttributeAnalyzer.rule.DefaultSeverity,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 7) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System;

[HarmonyPatch]
class Test
{
    static void Postfix() { }
}

class HarmonyPatch : Attribute { }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new AttributeAnalyzer();
        protected override CodeFixProvider GetCSharpCodeFixProvider() => new AttributeFix();
    }
}
