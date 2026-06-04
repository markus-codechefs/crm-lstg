# crm-lstg

CRM Dynamics **late-bound strong types** generator. Console app that connects to Microsoft Dataverse / Dynamics 365, reads entity metadata for all tables in a solution, and emits C# classes for compile-time safe attribute names and typed record models.

## Features

- Connects with a standard Dataverse connection string (`Microsoft.PowerPlatform.Dataverse.Client`)
- Filters entities by **solution unique name** (`solutioncomponent` where `componenttype = Entity`)
- Generates per-entity static classes with:
  - `EntityLogicalName`, `EntitySetName`, `EntityTypeCode`
  - `Fields` — string constants for attribute logical names
  - `OptionSets` — nested enums for picklist/state/status attributes (optional)
  - `Record` — POCO with typed properties for late-bound / SDK usage
- Output files use **UTF-8 with BOM**

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- A Dataverse environment and an app registration or user account with permission to read solution metadata
- A published or unmanaged solution whose entities you want to export

## Build

```bash
dotnet build CrmLstg.sln
```

## Usage

```bash
dotnet run --project src/CrmLstg.Console/CrmLstg.Console.csproj -- \
  --connection-string "AuthType=ClientSecret;Url=https://org.crm.dynamics.com;ClientId=...;ClientSecret=...;" \
  --solution "MySolutionUniqueName" \
  --output "./Generated" \
  --namespace "MyCompany.Crm.Generated"
```

### Options

| Option | Alias | Description |
|--------|-------|-------------|
| `--connection-string` | `-c` | Dataverse connection string (required) |
| `--solution` | `-s` | Solution unique name (required) |
| `--output` | `-o` | Output folder (default: `./Generated`) |
| `--namespace` | `-n` | C# namespace (default: `Crm.Generated`) |
| `--include-virtual-attributes` | | Include virtual attributes |
| `--use-sdk-types` | | Use `EntityReference`, `Money`, `OptionSetValue` (default: true) |
| `--option-set-enums` | | Emit nested option set enums (default: true) |

### Connection string examples

**Client secret (recommended for automation):**

```
AuthType=ClientSecret;Url=https://contoso.crm.dynamics.com;ClientId=<app-id>;ClientSecret=<secret>;
```

**Interactive / Office 365:**

```
AuthType=OAuth;Url=https://contoso.crm.dynamics.com;Username=user@contoso.onmicrosoft.com;Password=...;
```

## Generated layout

```
Generated/
  GeneratedAssemblyInfo.cs
  Entities/
    Account.cs
    Contact.cs
    ...
```

Example excerpt:

```csharp
public static partial class Account
{
    public const string EntityLogicalName = "account";

    public static class Fields
    {
        public const string Name = "name";
    }

    public sealed class Record
    {
        public string? Name { get; set; }
    }
}
```

Use `Fields` constants with late-bound `Entity` / `ColumnSet` APIs to avoid string typos, and `Record` for strongly typed in-memory shapes.

## Tests

```bash
dotnet test CrmLstg.sln
```

## Related tools

For full **early-bound** types (inherit `Microsoft.Xrm.Sdk.Entity`, `OrganizationServiceContext`), use [PAC CLI modelbuilder](https://learn.microsoft.com/en-us/power-platform/developer/cli/reference/modelbuilder) or `CrmSvcUtil.exe`. This project targets **late-bound** workflows with strong naming and typed POCOs instead.
