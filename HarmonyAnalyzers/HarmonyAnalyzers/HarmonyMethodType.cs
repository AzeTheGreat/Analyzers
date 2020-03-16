using HarmonyAnalyzers.Interface;
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
        // TODO: Make this...not ugly...
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
            
            new HarmonyMethodType("Prepare", (IMethodSymbol method) => method.ReturnType.SpecialType.Equals(SpecialType.System_Boolean), () => PredefinedType(Token(SyntaxKind.BoolKeyword))),
            
            new HarmonyMethodType("Prefix", (IMethodSymbol method) =>
            {
                var type = method.ReturnType.SpecialType;
                return method.ReturnsVoid || type.Equals(SpecialType.System_Boolean);
            },
                () => PredefinedType(Token(SyntaxKind.VoidKeyword))),
            
            new HarmonyMethodType("Postfix", (IMethodSymbol method) =>
            {
#pragma warning disable RS1024 // Compare symbols correctly - Not sure why this is triggering here, and the fix errors.
                return method.ReturnsVoid || method.ReturnType.Equals(method.Parameters.FirstOrDefault()?.Type);
#pragma warning restore RS1024 // Compare symbols correctly
            },
                () => PredefinedType(Token(SyntaxKind.VoidKeyword))),
            
            new HarmonyMethodType("Transpiler", (IMethodSymbol method) =>
            {
                var type = method.ReturnType as INamedTypeSymbol;
                return type.ConstructedFrom.SpecialType.Equals(SpecialType.System_Collections_Generic_IEnumerable_T) && type.TypeArguments.FirstOrDefault()?.Name == "CodeInstruction";
            },
                () => GenericName("IEnumerable").AddTypeArgumentListArguments(IdentifierName("CodeInstruction")))
        };

        public static bool IsHarmonyMethod(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.IsStatic && methodSymbol.IsInHarmonyClass() && methodSymbol.IsHarmonyMethodName();
        }

        public static bool IsInHarmonyClass(this IMethodSymbol methodSymbol) => methodSymbol.ContainingType.IsHarmonyClass();

        public static bool IsHarmonyClass(this INamedTypeSymbol namedTypeSymbol)
        {
            do
            {
                if (IsHarmonyClassAtLevel(namedTypeSymbol))
                    return true;
            } while ((namedTypeSymbol = namedTypeSymbol.BaseType) != null);
            return false;

            bool IsHarmonyClassAtLevel(INamedTypeSymbol typeSymbol)
            {
                foreach (var attribute in typeSymbol.GetAttributes())
                {
                    var name = attribute.AttributeClass.Name;                                                               // Consider it a Harmony class if:
                    if (name == "HarmonyPatch" ||                                                                               // That class has the patch attribute (normal usage)
                        name == nameof(HarmonyPatchMock) ||                                                                     // That class has the mock attribute (useful for manual patching)
                        attribute.AttributeClass.GetAttributes().Any(x => x.AttributeClass.Name == nameof(HarmonyPatchMock)))   // Any of the attributes have the mock attribute (useful for reflection)
                        return true;
                }
                return false;
            }
        }

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
