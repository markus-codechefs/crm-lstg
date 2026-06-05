using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Generation;

public sealed class CSharpTypeMapper
{
    private readonly bool _useSdkTypes;

    public CSharpTypeMapper(bool useSdkTypes)
    {
        _useSdkTypes = useSdkTypes;
    }

    public string MapAttributeType(AttributeMetadata attribute)
    {
        return attribute.AttributeType switch
        {
            AttributeTypeCode.BigInt => "long?",
            AttributeTypeCode.Boolean => "bool?",
            AttributeTypeCode.Customer => GetEntityReferenceType(),
            AttributeTypeCode.DateTime => "DateTime?",
            AttributeTypeCode.Decimal => "decimal?",
            AttributeTypeCode.Double => "double?",
            AttributeTypeCode.Integer => "int?",
            AttributeTypeCode.Lookup => GetEntityReferenceType(),
            AttributeTypeCode.Memo => "string?",
            AttributeTypeCode.Money => GetMoneyType(),
            AttributeTypeCode.Owner => GetEntityReferenceType(),
            AttributeTypeCode.PartyList => GetEntityCollectionType(),
            AttributeTypeCode.Picklist => GetOptionSetType(),
            AttributeTypeCode.State => GetOptionSetType(),
            AttributeTypeCode.Status => GetOptionSetType(),
            AttributeTypeCode.String => "string?",
            AttributeTypeCode.Uniqueidentifier => "Guid?",
            AttributeTypeCode.CalendarRules => "IEnumerable<object>?",
            AttributeTypeCode.Virtual => "object?",
            AttributeTypeCode.ManagedProperty => "bool?",
            AttributeTypeCode.EntityName => "string?",
            _ => "object?",
        };
    }

    public bool IsOptionSet(AttributeMetadata attribute)
    {
        return attribute.AttributeType is AttributeTypeCode.Picklist
            or AttributeTypeCode.State
            or AttributeTypeCode.Status;
    }

    private string GetEntityReferenceType()
    {
        return _useSdkTypes ? "Microsoft.Xrm.Sdk.EntityReference?" : "Guid?";
    }

    private string GetMoneyType()
    {
        return _useSdkTypes ? "Microsoft.Xrm.Sdk.Money?" : "decimal?";
    }

    private string GetOptionSetType()
    {
        return _useSdkTypes ? "Microsoft.Xrm.Sdk.OptionSetValue?" : "int?";
    }

    private string GetEntityCollectionType()
    {
        return _useSdkTypes ? "Microsoft.Xrm.Sdk.EntityCollection?" : "object?";
    }

}
