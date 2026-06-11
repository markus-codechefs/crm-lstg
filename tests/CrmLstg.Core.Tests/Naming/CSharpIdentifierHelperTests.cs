using CrmLstg.Core.Naming;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Tests.Naming;

public class CSharpIdentifierHelperTests
{
    [Fact]
    public void ToAttributeConstantName_PrimaryImage_ReturnsPrimaryImage()
    {
        // Arrange
        var attribute = new ImageAttributeMetadata
        {
            LogicalName = "entityimage",
            SchemaName = "EntityImage",
        };

        // Act
        var result = CSharpIdentifierHelper.ToAttributeConstantName(
            attribute,
            isPrimaryId: false,
            isPrimaryName: false,
            isPrimaryImage: true);

        // Assert
        Assert.Equal("PrimaryImage", result);
    }

    [Fact]
    public void ToAttributeConstantName_PrimaryId_ReturnsPrimaryKey()
    {
        // Arrange
        var attribute = new StringAttributeMetadata
        {
            LogicalName = "bkw_contactpointid",
            SchemaName = "BkwContactpointId",
        };

        // Act
        var result = CSharpIdentifierHelper.ToAttributeConstantName(attribute, isPrimaryId: true, isPrimaryName: false);

        // Assert
        Assert.Equal("PrimaryKey", result);
    }

    [Theory]
    [InlineData("account", "Account")]
    [InlineData("new_customfield", "NewCustomfield")]
    [InlineData("msdyn_workorder", "MsdynWorkorder")]
    public void ToPascalCaseIdentifier_ValidLogicalName_ReturnsPascalCase(string logicalName, string expected)
    {
        // Arrange
        // (parameters)

        // Act
        var result = CSharpIdentifierHelper.ToPascalCaseIdentifier(logicalName);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToPascalCaseIdentifier_ReservedKeyword_PrefixesWithAt()
    {
        // Arrange
        const string logicalName = "class";

        // Act
        var result = CSharpIdentifierHelper.ToPascalCaseIdentifier(logicalName);

        // Assert
        Assert.Equal("@Class", result);
    }

    [Fact]
    public void ToEnumMemberName_EmptyLabel_UsesValueSuffix()
    {
        // Arrange
        const int value = 100000001;

        // Act
        var result = CSharpIdentifierHelper.ToEnumMemberName(string.Empty, value);

        // Assert
        Assert.Equal("Value100000001", result);
    }
}
