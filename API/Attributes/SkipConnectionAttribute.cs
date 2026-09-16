namespace API.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SkipConnectionAttribute : Attribute
    {
        // This is a marker attribute, so it doesn't need logic inside.
        // It simply acts as a "Flag" for the Filter to read.
    }
}
