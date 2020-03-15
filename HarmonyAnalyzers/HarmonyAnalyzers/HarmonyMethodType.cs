using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace HarmonyAnalyzers
{
    public static class HarmonyDef
    {
        private static readonly List<HarmonyMethodType> harmonyMethodDefs = new List<HarmonyMethodType>()
        {
            new HarmonyMethodType("TargetMethod", (IMethodSymbol method) =>
            {
                var name = method.ReturnType.Name;
                return name == nameof(MethodInfo) || name == nameof(MethodBase);
            },
                () => IdentifierName(nameof(MethodBase))),
            new HarmonyMethodType("TargetMethods", (IMethodSymbol method) =>
            {
                var name = method.ReturnType.Name;
                return name == nameof(IEnumerable<MethodInfo>) || name == nameof(IEnumerable<MethodBase>);
            },
                () => GenericName("IEnumerable").AddTypeArgumentListArguments(IdentifierName(nameof(MethodBase)))),
            new HarmonyMethodType("Prepare", (IMethodSymbol method) => method.ReturnType.Equals(SpecialType.System_Boolean), () => PredefinedType(Token(SyntaxKind.BoolKeyword))),
            new HarmonyMethodType("Prefix", (IMethodSymbol method) =>
            {
                var type = method.ReturnType.SpecialType;
                return method.ReturnsVoid || type.Equals(SpecialType.System_Boolean);
            },
                () => PredefinedType(Token(SyntaxKind.VoidKeyword))),
            new HarmonyMethodType("Postfix", (IMethodSymbol method) =>
            {
                // TODO: Add Passthrough support
                return method.ReturnsVoid;
            },
                () => PredefinedType(Token(SyntaxKind.VoidKeyword))),
            // TODO: Figure out these strings
            new HarmonyMethodType("Transpiler", (IMethodSymbol method) =>
            {
                return method.ReturnType.Name == "";
            },
                () => GenericName("IEnumerable").AddTypeArgumentListArguments(IdentifierName("CodeInstruction")))
        };

        public static bool IsHarmonyMethod(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.IsStatic && methodSymbol.IsInHarmonyClass() && methodSymbol.IsHarmonyMethodName();
        }

        public static bool IsInHarmonyClass(this IMethodSymbol methodSymbol) => methodSymbol.ContainingType.IsHarmonyClass();

        public static bool IsHarmonyClass(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.GetAttributes().Any(x => x.AttributeClass.Name == "HarmonyPatch");

        public static bool IsHarmonyMethodName(this IMethodSymbol methodSymbol)
        {
            var attributes = methodSymbol.GetAttributes();
            return attributes.Select(x => x.AttributeClass.Name).Intersect(harmonyMethodDefs.Select(x => x.attribute)).Any() || harmonyMethodDefs.Select(x => x.name).Contains(methodSymbol.Name);
        }

        public static bool TryGetHarmonyMethodType(this IMethodSymbol methodSymbol, out HarmonyMethodType harmonyMethodType)
        {
            var attributes = methodSymbol.GetAttributes().Select(x => x.AttributeClass.Name);

            harmonyMethodType = harmonyMethodDefs.Find(x => attributes.Contains(x.attribute) || methodSymbol.Name == x.name);

            if (harmonyMethodType == null)
                return false;
            else
                return true;
        }

        public class HarmonyMethodType
        {
            public HarmonyMethodType(string name, Func<IMethodSymbol, bool> isReturnTypeValid, Func<TypeSyntax> getReturnTypeSyntax)
            {
                this.name = name;
                attribute = "Harmony" + name;
                this.isReturnTypeValid = isReturnTypeValid;
                this.getReturnTypeSyntax = getReturnTypeSyntax;
            }

            public string name;
            public string attribute;
            public Func<IMethodSymbol, bool> isReturnTypeValid;
            public Func<TypeSyntax> getReturnTypeSyntax;
        }
    }
}
