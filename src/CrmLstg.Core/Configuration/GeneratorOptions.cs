namespace CrmLstg.Core.Configuration;

public sealed class GeneratorOptions
{
    public string ConnectionString { get; init; } = string.Empty;

    public string SolutionUniqueName { get; init; } = string.Empty;

    public string OutputDirectory { get; init; } = "./Generated";

    public string Namespace { get; init; } = "Crm.Generated";

    public bool IncludeVirtualAttributes { get; init; } = true;

    public bool GenerateOptionSetEnums { get; init; } = true;
}
