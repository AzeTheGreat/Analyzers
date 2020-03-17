# Suppression Attributes

Roslyn analyzers that allow you to suppress issues in your code caused by reflection.

**Installation:**  
Grab the most recent [NuGet package](https://www.nuget.org/packages/SuppressionAttributes).

**Usage:**  
`CustomAnalyzers.Interface` provides `[Suppress...]` attributes that can be used to suppress warnings.  These can be placed directly on the field, or on the attribute that is applied to the field.

This is useful if an attribute sets a field through reflection, which will normally result in [CS0649](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0649).  
`[MyIntGet] protected IActivationRangeTarget activationTarget;`  
To prevent the warning from showing, the `[SuppressNotAssigned]` attribute can be added to the attribute class:
```   
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
[SuppressNotAssigned]
public sealed class MyIntGetAttribute : Attribute { }
```
