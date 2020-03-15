using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
            }),
            new HarmonyMethodType("TargetMethods", (IMethodSymbol method) =>
            {
                var name = method.ReturnType.Name;
                return name == nameof(IEnumerable<MethodInfo>) || name == nameof(IEnumerable<MethodBase>);
            }),
            new HarmonyMethodType("Prepare", (IMethodSymbol method) => method.ReturnType.Equals(SpecialType.System_Boolean)),
            new HarmonyMethodType("Prefix", (IMethodSymbol method) =>
            {
                var type = method.ReturnType.SpecialType;
                return type.Equals(SpecialType.System_Void) || type.Equals(SpecialType.System_Boolean);
            }),
            new HarmonyMethodType("Postfix", (IMethodSymbol method) =>
            {
                // TODO: Add Passthrough support
                return method.ReturnType.SpecialType.Equals(SpecialType.System_Void);
            }),
            new HarmonyMethodType("Transpiler", (IMethodSymbol method) =>
            {
                return method.ReturnType.Name == "";
            })
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
            public HarmonyMethodType(string name, Func<IMethodSymbol, bool> isReturnTypeValid)
            {
                this.name = name;
                attribute = "Harmony" + name;
                this.isReturnTypeValid = isReturnTypeValid;
            }

            public string name;
            public string attribute;
            //public List<string> returnTypes;
            public Func<IMethodSymbol, bool> isReturnTypeValid;
        }
    }
}
