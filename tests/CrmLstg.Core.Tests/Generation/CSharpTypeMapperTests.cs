using CrmLstg.Core.Generation;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Tests.Generation;

public class CSharpTypeMapperTests
{
    [Fact]
    public void MapAttributeType_StringAttribute_ReturnsNullableString()
    {
        // Arrange
        var mapper = new CSharpTypeMapper(useSdkTypes: false);
        var attribute = new StringAttributeMetadata
        {
            LogicalName = "name",
            SchemaName = "Name",
        };

        // Act
        var result = mapper.MapAttributeType(attribute);

        // Assert
        Assert.Equal("string?", result);
    }

    [Fact]
    public void MapAttributeType_LookupWithSdkTypes_ReturnsEntityReference()
    {
        // Arrange
        var mapper = new CSharpTypeMapper(useSdkTypes: true);
        var attribute = new LookupAttributeMetadata
        {
            LogicalName = "parentaccountid",
            SchemaName = "ParentAccountId",
        };

        // Act
        var result = mapper.MapAttributeType(attribute);

        // Assert
        Assert.Equal("Microsoft.Xrm.Sdk.EntityReference?", result);
    }

    [Fact]
    public void MapAttributeType_PicklistWithoutSdkTypes_ReturnsNullableInt()
    {
        // Arrange
        var mapper = new CSharpTypeMapper(useSdkTypes: false);
        var attribute = new PicklistAttributeMetadata
        {
            LogicalName = "statuscode",
            SchemaName = "StatusCode",
        };

        // Act
        var result = mapper.MapAttributeType(attribute);

        // Assert
        Assert.Equal("int?", result);
    }
}
