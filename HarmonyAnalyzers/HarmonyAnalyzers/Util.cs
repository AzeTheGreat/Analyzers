using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Harmony
{
    public static class Util
    {
        private static ImmutableArray<string> suppressedAttributes = ImmutableArray.Create(
            "HarmonyTargetMethod",
            "HarmonyTargetMethods",
            "HarmonyPrepare",
            "HarmonyPrefix",
            "HarmonyPostfix",
            "HarmonyTranspiler");

        private static ImmutableArray<string> suppressedMethods = ImmutableArray.Create(
            "TargetMethod",
            "TargetMethods",
            "Prepare",
            "Prefix",
            "Postfix",
            "Transpiler");

        public static bool IsHarmonyMethod(ISymbol symbol)
        {
            return IsInHarmonyClass(symbol) && IsHarmonyMethodName(symbol);
        }

        public static bool IsHarmonyMethodName(ISymbol symbol)
        {
            var attributes = symbol.GetAttributes();
            return attributes.Select(x => x.AttributeClass.Name).Intersect(suppressedAttributes).Any() || suppressedMethods.Contains(symbol.Name);
        }

        public static bool IsInHarmonyClass(ISymbol symbol)
        {
            ImmutableArray<AttributeData> attributes;
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                    attributes = symbol.ContainingType.GetAttributes();
                    break;
                case SymbolKind.NamedType:
                    attributes = symbol.GetAttributes();
                    break;
                default:
                    throw new Exception("Invalid symbol Kind in Util.IsInHarmonyClass");
            }

            return attributes.Select(x => x.AttributeClass.Name).Contains("HarmonyPatch");
        }
    }
}
