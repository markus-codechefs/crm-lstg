using CrmLstg.Console.Configuration;
using CrmLstg.Core.Configuration;
using CrmLstg.Core.Generation;
using CrmLstg.Core.Metadata;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.CommandLine;

namespace CrmLstg.Console;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var configuration = AppConfiguration.Build();

        var connectionOption = new Option<string>(
            aliases: new[] { "--connection-string", "-c" },
            description: "Dataverse connection string (AuthType=ClientSecret or OAuth). Overrides configuration.")
        {
            IsRequired = false,
        };

        var solutionOption = new Option<string>(
            aliases: new[] { "--solution", "-s" },
            description: "Unique name of the Dynamics solution to export entities from. Overrides configuration.");

        var outputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            getDefaultValue: () => "./Generated",
            description: "Output directory for generated C# files.");

        var namespaceOption = new Option<string>(
            aliases: new[] { "--namespace", "-n" },
            getDefaultValue: () => "Crm.Generated",
            description: "Root namespace for generated types.");

        var excludeVirtualAttributesOption = new Option<bool>(
            aliases: new[] { "--exclude-virtual-attributes" },
            description: "Exclude virtual attributes from generated entity constants.");

        var optionSetEnumsOption = new Option<bool>(
            aliases: new[] { "--option-set-enums" },
            getDefaultValue: () => true,
            description: "Generate nested option set enums per entity.");

        var rootCommand = new RootCommand(
            "Connects to Dynamics CRM / Dataverse and generates late-bound strong C# types for entities in a solution.")
        {
            connectionOption,
            solutionOption,
            outputOption,
            namespaceOption,
            excludeVirtualAttributesOption,
            optionSetEnumsOption,
        };

        rootCommand.SetHandler(async (context) =>
        {
            var connectionString = ConnectionStringResolver.Resolve(
                context.ParseResult.GetValueForOption(connectionOption),
                configuration["ConnectionStrings:Dataverse"]);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string is required. Provide --connection-string, set ConnectionStrings:Dataverse in appsettings.json, " +
                    "use dotnet user-secrets set \"ConnectionStrings:Dataverse\" \"<connection-string>\", " +
                    "or set the ConnectionStrings__Dataverse environment variable.");
            }

            var solutionUniqueName = SolutionUniqueNameResolver.Resolve(
                context.ParseResult.GetValueForOption(solutionOption),
                configuration["CrmLstg:Solution"]);

            if (string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                using var serviceClient = new ServiceClient(connectionString);

                if (!serviceClient.IsReady)
                {
                    throw new InvalidOperationException($"Failed to connect to Dataverse: {serviceClient.LastError}");
                }

                var solutionResolver = new SolutionEntityResolver(serviceClient);
                var availableSolutions = solutionResolver.GetSolutionsByUniqueNamePrefix(
                    AvailableSolutionsFormatter.DefaultSolutionUniqueNamePrefix);

                throw new InvalidOperationException(
                    AvailableSolutionsFormatter.FormatMissingSolutionMessage(
                        availableSolutions,
                        AvailableSolutionsFormatter.DefaultSolutionUniqueNamePrefix));
            }

            var options = new GeneratorOptions
            {
                ConnectionString = connectionString,
                SolutionUniqueName = solutionUniqueName,
                OutputDirectory = Path.GetFullPath(context.ParseResult.GetValueForOption(outputOption)!),
                Namespace = context.ParseResult.GetValueForOption(namespaceOption)!,
                IncludeVirtualAttributes = !context.ParseResult.GetValueForOption(excludeVirtualAttributesOption),
                GenerateOptionSetEnums = context.ParseResult.GetValueForOption(optionSetEnumsOption),
            };

            var service = new TypeGenerationService(options);
            var result = await service.GenerateAsync(context.GetCancellationToken());

            await System.Console.Out.WriteLineAsync(
                $"Generated {result.EntityCount} entity type file(s) in '{result.OutputDirectory}'.");
        });

        return await rootCommand.InvokeAsync(args);
    }
}
