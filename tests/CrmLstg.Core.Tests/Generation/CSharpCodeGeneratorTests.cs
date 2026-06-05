using System.Text;
using CrmLstg.Core.Configuration;
using CrmLstg.Core.Generation;
using CrmLstg.Core.Tests.Helpers;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Tests.Generation;

public class CSharpCodeGeneratorTests
{
    [Fact]
    public void GenerateEntityFile_EntityConstantsFormat_ContainsExpectedStructure()
    {
        // Arrange
        var options = new GeneratorOptions
        {
            Namespace = "Bkw.Crm.Indiv.Core.Dynamics.EntityConstants",
            GenerateOptionSetEnums = true,
            IncludeVirtualAttributes = true,
        };

        var entity = new EntityMetadata
        {
            LogicalName = "bkw_contactpoint",
            SchemaName = "BkwContactPoint",
            EntitySetName = "bkw_contactpoints",
        };

        MetadataReflection.SetProperty(entity, "OwnershipType", OwnershipTypes.UserOwned);
        MetadataReflection.SetProperty(entity, "IntroducedVersion", "1.0");

        var primaryId = new StringAttributeMetadata
        {
            LogicalName = "bkw_contactpointid",
            SchemaName = "BkwContactpointId",
        };

        MetadataReflection.SetProperty(primaryId, "IsPrimaryId", true);
        MetadataReflection.SetProperty(
            primaryId,
            "RequiredLevel",
            new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.SystemRequired));

        var primaryName = new StringAttributeMetadata
        {
            LogicalName = "bkw_name",
            SchemaName = "BkwName",
        };

        MetadataReflection.SetProperty(primaryName, "IsPrimaryName", true);
        MetadataReflection.SetProperty(primaryName, "IsValidForRead", true);

        var lookup = new LookupAttributeMetadata
        {
            LogicalName = "bkw_accountid",
            SchemaName = "BkwAccountId",
        };

        MetadataReflection.SetProperty(lookup, "IsValidForRead", true);

        var picklist = new PicklistAttributeMetadata
        {
            LogicalName = "bkw_portal",
            SchemaName = "BkwPortal",
        };

        var optionSet = new OptionSetMetadata
        {
            OptionSetType = OptionSetType.Picklist,
        };

        MetadataReflection.SetProperty(
            optionSet,
            "Options",
            new OptionMetadataCollection(
                new List<OptionMetadata>
                {
                    new(new Microsoft.Xrm.Sdk.Label("EMSYS Portal WET", 1033), 484170000),
                }));

        MetadataReflection.SetProperty(picklist, "OptionSet", optionSet);
        MetadataReflection.SetProperty(picklist, "IsValidForRead", true);

        MetadataReflection.SetProperty(
            entity,
            "Attributes",
            new AttributeMetadata[] { primaryId, primaryName, lookup, picklist });

        var relationship = new OneToManyRelationshipMetadata
        {
            SchemaName = "bkw_contactpoint_accountid_account",
            ReferencingEntity = "bkw_contactpoint",
            ReferencedEntity = "account",
            ReferencingAttribute = "bkw_accountid",
        };

        MetadataReflection.SetProperty(entity, "OneToManyRelationships", new[] { relationship });

        var generator = new CSharpCodeGenerator(options);

        // Act
        var source = generator.GenerateEntityFile(entity);

        // Assert
        Assert.Contains("namespace Bkw.Crm.Indiv.Core.Dynamics.EntityConstants", source);
        Assert.Contains("public static class BkwContactPoint", source);
        Assert.Contains("OwnershipType: UserOwned", source);
        Assert.Contains("public const string EntityName = \"bkw_contactpoint\";", source);
        Assert.Contains("public const string EntityCollectionName = \"bkw_contactpoints\";", source);
        Assert.Contains("#region Attributes", source);
        Assert.Contains("public const string PrimaryKey = \"bkw_contactpointid\";", source);
        Assert.Contains("public const string PrimaryName = \"bkw_name\";", source);
        Assert.Contains("#region Relationships", source);
        Assert.Contains("RelM1_BkwContactpointAccountidAccount", source);
        Assert.Contains("#region OptionSets", source);
        Assert.Contains("public enum BkwPortal_OptionSet", source);
        Assert.DoesNotContain("class Record", source);
        Assert.DoesNotContain("class Fields", source);
    }

    [Fact]
    public void AppendRelationshipsRegion_OneToMany_WritesConstant()
    {
        // Arrange
        var entity = new EntityMetadata
        {
            LogicalName = "bkw_contactpoint",
            SchemaName = "BkwContactPoint",
        };

        var relationship = new OneToManyRelationshipMetadata
        {
            SchemaName = "bkw_contactpoint_accountid_account",
            ReferencingEntity = "bkw_contactpoint",
            ReferencedEntity = "account",
            ReferencingAttribute = "bkw_accountid",
        };

        MetadataReflection.SetProperty(
            entity,
            "Attributes",
            new AttributeMetadata[]
            {
                new LookupAttributeMetadata
                {
                    LogicalName = "bkw_accountid",
                    SchemaName = "BkwAccountId",
                },
            });

        MetadataReflection.SetProperty(entity, "OneToManyRelationships", new[] { relationship });
        var builder = new StringBuilder();

        // Act
        RelationshipConstantsGenerator.AppendRelationshipsRegion(builder, entity);
        var output = builder.ToString();

        // Assert
        Assert.Contains("Parent: \"Account\" Child: \"BkwContactPoint\" Lookup: \"BkwAccountId\"", output);
        Assert.Contains("RelM1_BkwContactpointAccountidAccount", output);
    }
}
