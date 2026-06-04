using CrmLstg.Core.Configuration;
using CrmLstg.Core.Generation;
using System.CommandLine;

namespace CrmLstg.Console;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var connectionOption = new Option<string>(
            aliases: new[] { "--connection-string", "-c" },
            description: "Dataverse connection string (AuthType=ClientSecret or OAuth).")
        {
            IsRequired = true,
        };

        var solutionOption = new Option<string>(
            aliases: new[] { "--solution", "-s" },
            description: "Unique name of the Dynamics solution to export entities from.")
        {
            IsRequired = true,
        };

        var outputOption = new Option<string>(
            aliases: new[] { "--output", "-o" },
            getDefaultValue: () => "./Generated",
            description: "Output directory for generated C# files.");

        var namespaceOption = new Option<string>(
            aliases: new[] { "--namespace", "-n" },
            getDefaultValue: () => "Crm.Generated",
            description: "Root namespace for generated types.");

        var virtualAttributesOption = new Option<bool>(
            aliases: new[] { "--include-virtual-attributes" },
            description: "Include virtual attributes in generated types.");

        var sdkTypesOption = new Option<bool>(
            aliases: new[] { "--use-sdk-types" },
            getDefaultValue: () => true,
            description: "Use Microsoft.Xrm.Sdk types (EntityReference, Money, OptionSetValue) in generated properties.");

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
            virtualAttributesOption,
            sdkTypesOption,
            optionSetEnumsOption,
        };

        rootCommand.SetHandler(async (context) =>
        {
            var options = new GeneratorOptions
            {
                ConnectionString = context.ParseResult.GetValueForOption(connectionOption)!,
                SolutionUniqueName = context.ParseResult.GetValueForOption(solutionOption)!,
                OutputDirectory = Path.GetFullPath(context.ParseResult.GetValueForOption(outputOption)!),
                Namespace = context.ParseResult.GetValueForOption(namespaceOption)!,
                IncludeVirtualAttributes = context.ParseResult.GetValueForOption(virtualAttributesOption),
                UseSdkTypes = context.ParseResult.GetValueForOption(sdkTypesOption),
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
