using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmLstg.Core.Metadata;

public sealed class EntityMetadataProvider
{
    private readonly IOrganizationService _organizationService;

    public EntityMetadataProvider(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    public async Task<IReadOnlyList<EntityMetadata>> RetrieveEntitiesAsync(
        IEnumerable<Guid> metadataIds,
        CancellationToken cancellationToken)
    {
        var entities = new List<EntityMetadata>();
        var idList = metadataIds.Distinct().ToList();

        foreach (var metadataId in idList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new RetrieveEntityRequest
            {
                MetadataId = metadataId,
                EntityFilters = EntityFilters.Attributes | EntityFilters.Relationships,
                RetrieveAsIfPublished = true,
            };

            var response = (RetrieveEntityResponse)await Task.Run(
                () => _organizationService.Execute(request),
                cancellationToken);

            entities.Add(response.EntityMetadata);
        }

        return entities
            .OrderBy(entity => entity.LogicalName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
