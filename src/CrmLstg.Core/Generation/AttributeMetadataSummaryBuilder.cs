using System.Text;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Generation;

public static class AttributeMetadataSummaryBuilder
{
    public static string Build(AttributeMetadata attribute)
    {
        var parts = new List<string>
        {
            "Type: " + GetTypeDescription(attribute),
        };

        if (attribute.RequiredLevel != null)
        {
            parts.Add("RequiredLevel: " + attribute.RequiredLevel.Value);
        }

        AppendTypeSpecificParts(attribute, parts);

        return string.Join(", ", parts);
    }

    private static string GetTypeDescription(AttributeMetadata attribute)
    {
        if (attribute is ImageAttributeMetadata)
        {
            return "Image";
        }

        var typeName = attribute.AttributeType?.ToString() ?? "Unknown";

        if (IsLogicalAttribute(attribute))
        {
            typeName += " (Logical)";
        }

        return typeName;
    }

    private static bool IsLogicalAttribute(AttributeMetadata attribute)
    {
        if (attribute.IsLogical == true)
        {
            return true;
        }

        return !string.IsNullOrEmpty(attribute.AttributeOf);
    }

    private static void AppendTypeSpecificParts(AttributeMetadata attribute, List<string> parts)
    {
        switch (attribute)
        {
            case StringAttributeMetadata stringAttribute:
                AppendStringParts(stringAttribute, parts);
                break;
            case MemoAttributeMetadata memoAttribute:
                parts.Add("MaxLength: " + memoAttribute.MaxLength);
                break;
            case LookupAttributeMetadata lookupAttribute:
                AppendLookupTargets(lookupAttribute.Targets, parts);
                break;
            case DateTimeAttributeMetadata dateTimeAttribute:
                if (dateTimeAttribute.Format != null)
                {
                    parts.Add("Format: " + dateTimeAttribute.Format);
                }

                if (dateTimeAttribute.DateTimeBehavior != null)
                {
                    parts.Add("DateTimeBehavior: " + dateTimeAttribute.DateTimeBehavior);
                }

                break;
            case IntegerAttributeMetadata integerAttribute:
                parts.Add("MinValue: " + integerAttribute.MinValue);
                parts.Add("MaxValue: " + integerAttribute.MaxValue);
                break;
            case BigIntAttributeMetadata bigIntAttribute:
                parts.Add("MinValue: " + bigIntAttribute.MinValue);
                parts.Add("MaxValue: " + bigIntAttribute.MaxValue);
                break;
            case DecimalAttributeMetadata decimalAttribute:
                parts.Add("MinValue: " + decimalAttribute.MinValue);
                parts.Add("MaxValue: " + decimalAttribute.MaxValue);
                break;
            case DoubleAttributeMetadata doubleAttribute:
                parts.Add("MinValue: " + doubleAttribute.MinValue);
                parts.Add("MaxValue: " + doubleAttribute.MaxValue);
                break;
            case MoneyAttributeMetadata moneyAttribute:
                parts.Add("MinValue: " + moneyAttribute.MinValue);
                parts.Add("MaxValue: " + moneyAttribute.MaxValue);
                break;
            case EnumAttributeMetadata enumAttribute:
                AppendOptionSetParts(enumAttribute, parts);
                break;
            case ImageAttributeMetadata imageAttribute:
                parts.Add("MaxSizeInKB: " + imageAttribute.MaxSizeInKB);
                break;
        }
    }

    private static void AppendStringParts(StringAttributeMetadata stringAttribute, List<string> parts)
    {
        if (stringAttribute.MaxLength.HasValue)
        {
            parts.Add("MaxLength: " + stringAttribute.MaxLength.Value);
        }

        if (stringAttribute.Format != null)
        {
            parts.Add("Format: " + stringAttribute.Format);
        }
    }

    private static void AppendLookupTargets(string[]? targets, List<string> parts)
    {
        if (targets == null || targets.Length == 0)
        {
            return;
        }

        parts.Add("Targets: " + string.Join(",", targets));
    }

    private static void AppendOptionSetParts(EnumAttributeMetadata enumAttribute, List<string> parts)
    {
        var displayName = enumAttribute.DisplayName?.UserLocalizedLabel?.Label;

        if (!string.IsNullOrEmpty(displayName))
        {
            parts.Add("DisplayName: " + displayName);
        }

        if (enumAttribute.OptionSet is OptionSetMetadata optionSet)
        {
            parts.Add("OptionSetType: " + optionSet.OptionSetType);
        }

        if (enumAttribute.DefaultFormValue.HasValue)
        {
            parts.Add("DefaultFormValue: " + enumAttribute.DefaultFormValue.Value);
        }
    }

    public static string EscapeXmlDoc(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return new StringBuilder(value)
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .ToString();
    }
}
