using System.Reflection;

namespace CrmLstg.Core.Tests.Helpers;

internal static class MetadataReflection
{
    public static void SetProperty(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        if (property == null)
        {
            throw new InvalidOperationException($"Property '{propertyName}' was not found on {target.GetType().Name}.");
        }

        property.SetValue(target, value);
    }
}
