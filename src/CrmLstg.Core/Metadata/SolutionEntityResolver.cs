using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CrmLstg.Core.Metadata;

public sealed class SolutionEntityResolver
{
    private const int EntityComponentType = 1;

    private readonly IOrganizationService _organizationService;

    public SolutionEntityResolver(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public IReadOnlyList<SolutionSummary> GetSolutionsByUniqueNamePrefix(string uniqueNamePrefix)
    {
        var query = new QueryExpression("solution")
        {
            ColumnSet = new ColumnSet("uniquename", "friendlyname", "version"),
            Criteria =
            {
                Conditions =
                {
                    new ConditionExpression("uniquename", ConditionOperator.BeginsWith, uniqueNamePrefix),
                },
            },
            Orders =
            {
                new OrderExpression("uniquename", OrderType.Ascending),
            },
        };

        var results = _organizationService.RetrieveMultiple(query);
        var solutions = new List<SolutionSummary>(results.Entities.Count);

        foreach (var solution in results.Entities)
        {
            var uniqueName = solution.GetAttributeValue<string>("uniquename") ?? string.Empty;
            var friendlyName = solution.GetAttributeValue<string>("friendlyname") ?? string.Empty;
            var version = solution.GetAttributeValue<string>("version") ?? string.Empty;

            solutions.Add(new SolutionSummary(uniqueName, friendlyName, version));
        }

        return solutions;
    }

    public Guid ResolveSolutionId(string solutionUniqueName)
    {
        var query = new QueryExpression("solution")
        {
            ColumnSet = new ColumnSet("solutionid"),
            Criteria =
            {
                Conditions =
                {
                    new ConditionExpression("uniquename", ConditionOperator.Equal, solutionUniqueName),
                },
            },
            TopCount = 2,
        };

        var results = _organizationService.RetrieveMultiple(query);

        if (results.Entities.Count == 0)
        {
            throw new InvalidOperationException($"Solution '{solutionUniqueName}' was not found.");
        }

        if (results.Entities.Count > 1)
        {
            throw new InvalidOperationException($"Multiple solutions matched unique name '{solutionUniqueName}'.");
        }

        return results.Entities[0].Id;
    }

    public IReadOnlyList<Guid> GetEntityMetadataIds(Guid solutionId)
    {
        var query = new QueryExpression("solutioncomponent")
        {
            ColumnSet = new ColumnSet("objectid"),
            Criteria =
            {
                FilterOperator = LogicalOperator.And,
                Conditions =
                {
                    new ConditionExpression("solutionid", ConditionOperator.Equal, solutionId),
                    new ConditionExpression("componenttype", ConditionOperator.Equal, EntityComponentType),
                },
            },
        };

        var results = _organizationService.RetrieveMultiple(query);
        var metadataIds = new List<Guid>(results.Entities.Count);

        foreach (var component in results.Entities)
        {
            if (component.TryGetAttributeValue<Guid>("objectid", out var objectId) && objectId != Guid.Empty)
            {
                metadataIds.Add(objectId);
            }
        }

        return metadataIds;
    }
}
