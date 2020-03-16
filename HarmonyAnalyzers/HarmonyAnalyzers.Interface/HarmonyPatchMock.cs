using System;

namespace HarmonyAnalyzers.Interface
{
    /// <summary>
    /// Used to make HarmonyAnalyzers think a class will be used for patching, without Harmony patching it.
    /// Useful for manual and reflection patching.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class HarmonyPatchMock : Attribute { }
}
