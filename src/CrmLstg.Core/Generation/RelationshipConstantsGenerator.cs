using System.Text;
using CrmLstg.Core.Naming;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Generation;

public static class RelationshipConstantsGenerator
{
    public static void AppendRelationshipsRegion(StringBuilder builder, EntityMetadata entity)
    {
        var relationships = GetRelationshipsForEntity(entity).ToList();

        if (relationships.Count == 0)
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine("        #region Relationships");
        builder.AppendLine();

        foreach (var relationship in relationships)
        {
            AppendRelationship(builder, entity, relationship);
        }

        builder.AppendLine("        #endregion Relationships");
    }

    private static IEnumerable<RelationshipMetadataBase> GetRelationshipsForEntity(EntityMetadata entity)
    {
        if (entity.OneToManyRelationships != null)
        {
            foreach (var relationship in entity.OneToManyRelationships)
            {
                yield return relationship;
            }
        }

        if (entity.ManyToManyRelationships != null)
        {
            foreach (var relationship in entity.ManyToManyRelationships)
            {
                yield return relationship;
            }
        }
    }

    private static void AppendRelationship(
        StringBuilder builder,
        EntityMetadata entity,
        RelationshipMetadataBase relationship)
    {
        if (relationship is OneToManyRelationshipMetadata oneToMany)
        {
            AppendOneToManyRelationship(builder, entity, oneToMany);
            return;
        }

        if (relationship is ManyToManyRelationshipMetadata manyToMany)
        {
            AppendManyToManyRelationship(builder, entity, manyToMany);
        }
    }

    private static void AppendOneToManyRelationship(
        StringBuilder builder,
        EntityMetadata entity,
        OneToManyRelationshipMetadata relationship)
    {
        var isManySide = string.Equals(
            relationship.ReferencingEntity,
            entity.LogicalName,
            StringComparison.OrdinalIgnoreCase);

        var prefix = isManySide ? "RelM1_" : "Rel1M_";
        var constantName = prefix + ToRelationshipConstantSuffix(relationship);
        var schemaName = relationship.SchemaName ?? string.Empty;
        var parentDisplay = ToEntityDisplayName(entity, relationship.ReferencedEntity);
        var childDisplay = ToEntityDisplayName(entity, relationship.ReferencingEntity);
        var lookupDisplay = isManySide
            ? GetLookupDisplayName(entity, relationship.ReferencingAttribute)
            : string.Empty;

        var summary = $"Parent: \"{parentDisplay}\" Child: \"{childDisplay}\" Lookup: \"{lookupDisplay}\"";

        builder.AppendLine($"        /// <summary>{AttributeMetadataSummaryBuilder.EscapeXmlDoc(summary)}</summary>");
        builder.AppendLine($"        public const string {constantName} = \"{schemaName}\";");
    }

    private static void AppendManyToManyRelationship(
        StringBuilder builder,
        EntityMetadata entity,
        ManyToManyRelationshipMetadata relationship)
    {
        var entity1 = relationship.Entity1LogicalName ?? string.Empty;
        var entity2 = relationship.Entity2LogicalName ?? string.Empty;
        var isEntity1 = string.Equals(entity1, entity.LogicalName, StringComparison.OrdinalIgnoreCase);
        var otherEntity = isEntity1 ? entity2 : entity1;
        var prefix = "RelMM_";
        var constantName = prefix + ToRelationshipConstantSuffix(relationship);
        var schemaName = relationship.SchemaName ?? string.Empty;
        var summary = $"Entity1: \"{entity1}\" Entity2: \"{entity2}\" Intersect: \"{otherEntity}\"";

        builder.AppendLine($"        /// <summary>{AttributeMetadataSummaryBuilder.EscapeXmlDoc(summary)}</summary>");
        builder.AppendLine($"        public const string {constantName} = \"{schemaName}\";");
    }

    private static string GetLookupDisplayName(EntityMetadata entity, string? referencingAttributeLogicalName)
    {
        if (string.IsNullOrEmpty(referencingAttributeLogicalName))
        {
            return string.Empty;
        }

        var attribute = entity.Attributes?.FirstOrDefault(candidate =>
            string.Equals(candidate.LogicalName, referencingAttributeLogicalName, StringComparison.OrdinalIgnoreCase));

        if (attribute != null)
        {
            return CSharpIdentifierHelper.ToAttributeConstantName(
                attribute,
                attribute.IsPrimaryId == true,
                attribute.IsPrimaryName == true);
        }

        return CSharpIdentifierHelper.ToAttributeConstantName(
            referencingAttributeLogicalName,
            isPrimaryId: false,
            isPrimaryName: false);
    }

    private static string ToEntityDisplayName(EntityMetadata entity, string? logicalName)
    {
        if (string.IsNullOrEmpty(logicalName))
        {
            return string.Empty;
        }

        if (string.Equals(entity.LogicalName, logicalName, StringComparison.OrdinalIgnoreCase))
        {
            return CSharpIdentifierHelper.ToEntityClassName(entity);
        }

        return CSharpIdentifierHelper.ToPascalCaseIdentifier(logicalName);
    }

    private static string ToRelationshipConstantSuffix(RelationshipMetadataBase relationship)
    {
        if (!string.IsNullOrEmpty(relationship.SchemaName))
        {
            return CSharpIdentifierHelper.ToPascalCaseIdentifier(relationship.SchemaName);
        }

        return "UnknownRelationship";
    }
}
