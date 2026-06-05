using System.Text;
using CrmLstg.Core.Configuration;
using CrmLstg.Core.Naming;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Generation;

public sealed class CSharpCodeGenerator
{
    private readonly GeneratorOptions _options;
    private readonly CSharpTypeMapper _typeMapper;

    public CSharpCodeGenerator(GeneratorOptions options)
    {
        _options = options;
        _typeMapper = new CSharpTypeMapper(useSdkTypes: false);
    }

    public string GenerateEntityFile(EntityMetadata entity)
    {
        var className = CSharpIdentifierHelper.ToEntityClassName(entity);
        var builder = new StringBuilder();

        AppendFileHeader(builder);
        builder.AppendLine($"namespace {_options.Namespace}");
        builder.AppendLine("{");
        builder.AppendLine($"    /// <summary>{AttributeMetadataSummaryBuilder.EscapeXmlDoc(BuildEntitySummary(entity))}</summary>");
        builder.AppendLine($"    public static class {className}");
        builder.AppendLine("    {");
        builder.AppendLine($"        public const string EntityName = \"{entity.LogicalName}\";");
        builder.AppendLine($"        public const string EntityCollectionName = \"{entity.EntitySetName}\";");
        builder.AppendLine();
        AppendAttributesRegion(builder, entity);
        RelationshipConstantsGenerator.AppendRelationshipsRegion(builder, entity);

        if (_options.GenerateOptionSetEnums)
        {
            AppendOptionSetsRegion(builder, entity);
        }

        builder.AppendLine("    }");
        builder.AppendLine("}");

        return builder.ToString();
    }

    private static string BuildEntitySummary(EntityMetadata entity)
    {
        var parts = new List<string>();

        var displayName = entity.DisplayName?.UserLocalizedLabel?.Label;

        if (!string.IsNullOrEmpty(displayName))
        {
            parts.Add("DisplayName: " + displayName);
        }

        if (entity.OwnershipType.HasValue)
        {
            parts.Add("OwnershipType: " + entity.OwnershipType.Value);
        }

        if (!string.IsNullOrEmpty(entity.IntroducedVersion))
        {
            parts.Add("IntroducedVersion: " + entity.IntroducedVersion);
        }

        return string.Join(", ", parts);
    }

    private void AppendAttributesRegion(StringBuilder builder, EntityMetadata entity)
    {
        builder.AppendLine("        #region Attributes");
        builder.AppendLine();

        foreach (var attribute in GetOrderedAttributes(entity))
        {
            var constantName = CSharpIdentifierHelper.ToAttributeConstantName(
                attribute,
                attribute.IsPrimaryId == true,
                attribute.IsPrimaryName == true);

            var summary = AttributeMetadataSummaryBuilder.Build(attribute);

            builder.AppendLine($"        /// <summary>{AttributeMetadataSummaryBuilder.EscapeXmlDoc(summary)}</summary>");
            builder.AppendLine($"        public const string {constantName} = \"{attribute.LogicalName}\";");
        }

        builder.AppendLine();
        builder.AppendLine("        #endregion Attributes");
    }

    private void AppendOptionSetsRegion(StringBuilder builder, EntityMetadata entity)
    {
        var attributes = GetOrderedAttributes(entity)
            .Where(_typeMapper.IsOptionSet)
            .ToList();

        if (attributes.Count == 0)
        {
            return;
        }

        builder.AppendLine();
        builder.AppendLine("        #region OptionSets");
        builder.AppendLine();

        foreach (var attribute in attributes)
        {
            if (attribute is not EnumAttributeMetadata enumAttribute)
            {
                continue;
            }

            var enumName = CSharpIdentifierHelper.ToOptionSetEnumName(attribute.SchemaName ?? attribute.LogicalName);

            builder.AppendLine($"        public enum {enumName}");
            builder.AppendLine("        {");

            if (enumAttribute.OptionSet is OptionSetMetadata optionSet && optionSet.Options != null)
            {
                foreach (var option in optionSet.Options.OrderBy(o => o.Value))
                {
                    var memberName = CSharpIdentifierHelper.ToEnumMemberName(
                        option.Label?.UserLocalizedLabel?.Label ?? string.Empty,
                        option.Value ?? 0);

                    builder.AppendLine($"            {memberName} = {option.Value},");
                }
            }

            builder.AppendLine("        }");
        }

        builder.AppendLine();
        builder.AppendLine("        #endregion OptionSets");
    }

    private IEnumerable<AttributeMetadata> GetOrderedAttributes(EntityMetadata entity)
    {
        var attributes = entity.Attributes?
            .Where(ShouldIncludeAttribute)
            .ToList()
            ?? new List<AttributeMetadata>();

        return attributes
            .OrderBy(attribute => GetAttributeSortKey(attribute))
            .ThenBy(attribute => CSharpIdentifierHelper.ToAttributeConstantName(
                attribute,
                attribute.IsPrimaryId == true,
                attribute.IsPrimaryName == true),
                StringComparer.Ordinal);
    }

    private bool ShouldIncludeAttribute(AttributeMetadata attribute)
    {
        if (attribute.AttributeType == AttributeTypeCode.Virtual && !_options.IncludeVirtualAttributes)
        {
            return false;
        }

        return attribute.IsValidForCreate == true
            || attribute.IsValidForUpdate == true
            || attribute.IsValidForRead == true
            || attribute.IsPrimaryId == true
            || attribute.IsPrimaryName == true
            || attribute.IsLogical == true;
    }

    private static int GetAttributeSortKey(AttributeMetadata attribute)
    {
        if (attribute.IsPrimaryId == true)
        {
            return 0;
        }

        if (attribute.IsPrimaryName == true)
        {
            return 1;
        }

        return 2;
    }

    private static void AppendFileHeader(StringBuilder builder)
    {
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("// Generated by CrmLstg (CRM Dynamics late-bound strong types generator).");
        builder.AppendLine($"// Generated at {DateTime.UtcNow:O}");
        builder.AppendLine();
    }
}
