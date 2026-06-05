using CrmLstg.Core.Generation;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Tests.Generation;

public class AttributeMetadataSummaryBuilderTests
{
    [Fact]
    public void Build_PrimaryIdAttribute_IncludesTypeAndRequiredLevel()
    {
        // Arrange
        var attribute = new StringAttributeMetadata
        {
            LogicalName = "bkw_contactpointid",
            SchemaName = "BkwContactpointId",
            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.SystemRequired),
        };

        // Act
        var summary = AttributeMetadataSummaryBuilder.Build(attribute);

        // Assert
        Assert.Contains("Type: String", summary);
        Assert.Contains("RequiredLevel:", summary);
    }

    [Fact]
    public void Build_LookupAttribute_IncludesTargets()
    {
        // Arrange
        var attribute = new LookupAttributeMetadata
        {
            LogicalName = "bkw_contactid",
            SchemaName = "BkwContactId",
            Targets = ["contact"],
        };

        // Act
        var summary = AttributeMetadataSummaryBuilder.Build(attribute);

        // Assert
        Assert.Contains("Type: Lookup", summary);
        Assert.Contains("Targets: contact", summary);
    }
}
