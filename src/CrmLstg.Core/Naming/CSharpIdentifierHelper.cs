using System.Globalization;
using System.Text;

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

    public static string ToPascalCaseIdentifier(string logicalName)
    {
        if (string.IsNullOrWhiteSpace(logicalName))
        {
            return "Unknown";
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

    public static string ToClassName(string entityLogicalName)
    {
        return ToPascalCaseIdentifier(entityLogicalName);
    }

    public static string ToFieldConstantName(string attributeLogicalName)
    {
        return ToPascalCaseIdentifier(attributeLogicalName);
    }

    public static string ToEnumName(string optionSetName, string? prefix = null)
    {
        var name = ToPascalCaseIdentifier(optionSetName);

        if (!string.IsNullOrEmpty(prefix) && !name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            name = prefix + name;
        }

        if (!name.EndsWith("Option", StringComparison.Ordinal))
        {
            name += "Option";
        }

        return name;
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
            else if (char.IsWhiteSpace(ch) || ch is '-' or '/')
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
}
