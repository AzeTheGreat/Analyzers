using System;

namespace CustomAnalyzers.Interface
{
    /// <summary>
    /// Used to suppress IDE0051 and CS0169.
    /// Can be applied to a field directly, or to an attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SuppressRemoveUnused : Attribute { }

    /// <summary>
    /// Used to suppress CS0649.
    /// Can be applied to a field directly, or to an attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SuppressNotAssigned : Attribute { }
}
