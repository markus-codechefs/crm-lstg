using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Metadata;

public static class PrimaryAttributeHelper
{
    public static bool IsPrimaryImage(EntityMetadata entity, AttributeMetadata attribute)
    {
        if (attribute is ImageAttributeMetadata imageAttribute && imageAttribute.IsPrimaryImage == true)
        {
            return true;
        }

        if (string.IsNullOrEmpty(entity.PrimaryImageAttribute))
        {
            return false;
        }

        return string.Equals(
            attribute.LogicalName,
            entity.PrimaryImageAttribute,
            StringComparison.OrdinalIgnoreCase);
    }
}
