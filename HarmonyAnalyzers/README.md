# Harmony Library Analyzers

Roslyn analyzers designed to work with the [Harmony Library](https://github.com/pardeike/Harmony).  Provides assistance when writing patch classes.

**Installation:**  
Grab the most recent [NuGet package](https://www.nuget.org/packages/HarmonyAnalyzers).

**Usage:**
The analyzers will automatically suppress issues that are irrelevant to Harmony classes, and will find common errors in patches.

When writing a classes that are used in manual patches, using the `[HarmonyPatchMock]` attribute in `HarmonyAnalyzers.Interface` will allow the analyzers to function without Harmony trying to patch with that class itself.
