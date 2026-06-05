using System.Globalization;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Naming;

public static class CSharpIdentifierHelper
{
    private static readonly HashSet<string> ReservedKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this",
        "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
        "using", "virtual", "void", "volatile", "while",
    };

    public static string ToEntityClassName(EntityMetadata entity)
    {
        if (!string.IsNullOrEmpty(entity.SchemaName))
        {
            return SanitizeIdentifier(entity.SchemaName);
        }

        return ToPascalCaseIdentifier(entity.LogicalName);
    }

    public static string ToAttributeConstantName(AttributeMetadata attribute, bool isPrimaryId, bool isPrimaryName)
    {
        if (isPrimaryId)
        {
            return "PrimaryKey";
        }

        if (isPrimaryName)
        {
            return "PrimaryName";
        }

        if (!string.IsNullOrEmpty(attribute.SchemaName))
        {
            return SanitizeIdentifier(attribute.SchemaName);
        }

        return ToAttributeConstantName(attribute.LogicalName, isPrimaryId, isPrimaryName);
    }

    public static string ToAttributeConstantName(string logicalName, bool isPrimaryId, bool isPrimaryName)
    {
        if (isPrimaryId)
        {
            return "PrimaryKey";
        }

        if (isPrimaryName)
        {
            return "PrimaryName";
        }

        return ToPascalCaseIdentifier(logicalName);
    }

    public static string ToOptionSetEnumName(string schemaOrLogicalName)
    {
        var baseName = schemaOrLogicalName.Contains('_', StringComparison.Ordinal)
            ? ToPascalCaseIdentifier(schemaOrLogicalName)
            : SanitizeIdentifier(schemaOrLogicalName);

        return baseName + "_OptionSet";
    }

    public static string ToPascalCaseIdentifier(string logicalName)
    {
        if (string.IsNullOrWhiteSpace(logicalName))
        {
            return "Unknown";
        }

        if (!logicalName.Contains('_', StringComparison.Ordinal) && char.IsUpper(logicalName[0]))
        {
            return SanitizeIdentifier(logicalName);
        }

        var segments = logicalName.Split('_', StringSplitOptions.RemoveEmptyEntries);
        var builder = new StringBuilder();

        foreach (var segment in segments)
        {
            if (segment.Length == 0)
            {
                continue;
            }

            builder.Append(char.ToUpperInvariant(segment[0]));

            if (segment.Length > 1)
            {
                builder.Append(segment.AsSpan(1).ToString().ToLowerInvariant());
            }
        }

        var identifier = builder.Length == 0 ? "Unknown" : builder.ToString();

        return SanitizeIdentifier(identifier);
    }

    public static string ToEnumMemberName(string label, int value)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return "Value" + value.ToString(CultureInfo.InvariantCulture);
        }

        var cleaned = new StringBuilder();

        foreach (var ch in label)
        {
            if (char.IsLetterOrDigit(ch))
            {
                cleaned.Append(ch);
            }
            else if (char.IsWhiteSpace(ch) || ch is '-' or '/' or '.')
            {
                cleaned.Append('_');
            }
        }

        var member = ToPascalCaseIdentifier(cleaned.ToString().Trim('_'));

        if (string.IsNullOrEmpty(member))
        {
            member = "Value" + value.ToString(CultureInfo.InvariantCulture);
        }

        if (char.IsDigit(member[0]))
        {
            member = "_" + member;
        }

        return member;
    }

    private static string SanitizeIdentifier(string identifier)
    {
        if (char.IsDigit(identifier[0]))
        {
            identifier = "_" + identifier;
        }

        if (ReservedKeywords.Contains(identifier) || ReservedKeywords.Contains(identifier.ToLowerInvariant()))
        {
            identifier = "@" + identifier;
        }

        return identifier;
    }
}
