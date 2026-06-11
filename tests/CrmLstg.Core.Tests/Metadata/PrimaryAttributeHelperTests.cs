using CrmLstg.Core.Metadata;
using CrmLstg.Core.Tests.Helpers;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Tests.Metadata;

public class PrimaryAttributeHelperTests
{
    [Fact]
    public void IsPrimaryImage_ImageAttributeWithIsPrimaryImage_ReturnsTrue()
    {
        // Arrange
        var entity = new EntityMetadata
        {
            LogicalName = "account",
        };

        var imageAttribute = new ImageAttributeMetadata
        {
            LogicalName = "entityimage",
            SchemaName = "EntityImage",
        };

        MetadataReflection.SetProperty(imageAttribute, "IsPrimaryImage", true);

        // Act
        var result = PrimaryAttributeHelper.IsPrimaryImage(entity, imageAttribute);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPrimaryImage_EntityPrimaryImageAttributeMatches_ReturnsTrue()
    {
        // Arrange
        var entity = new EntityMetadata
        {
            LogicalName = "account",
        };

        MetadataReflection.SetProperty(entity, "PrimaryImageAttribute", "entityimage");

        var imageAttribute = new ImageAttributeMetadata
        {
            LogicalName = "entityimage",
            SchemaName = "EntityImage",
        };

        // Act
        var result = PrimaryAttributeHelper.IsPrimaryImage(entity, imageAttribute);

        // Assert
        Assert.True(result);
    }
}
