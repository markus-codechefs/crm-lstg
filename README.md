# crm-lstg

CRM Dynamics **late-bound strong types** generator. Console app that connects to Microsoft Dataverse / Dynamics 365, reads entity metadata for all tables in a solution, and emits C# classes for compile-time safe attribute names and typed record models.

## Features

- Connects with a standard Dataverse connection string (`Microsoft.PowerPlatform.Dataverse.Client`)
- Filters entities by **solution unique name** (`solutioncomponent` where `componenttype = Entity`)
- Generates per-entity static classes in **EntityConstants** style:
  - Class summary with display name, ownership type, introduced version
  - `EntityName` and `EntityCollectionName` constants
  - `#region Attributes` with XML doc summaries and schema-based constant names (`PrimaryKey`, `PrimaryName`, …)
  - `#region Relationships` with `RelM1_` / `Rel1M_` / `RelMM_` relationship schema constants
  - `#region OptionSets` with `{Attribute}_OptionSet` enums
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
| `--exclude-virtual-attributes` | | Skip virtual attributes (included by default) |
| `--option-set-enums` | | Emit option set enums (default: true) |

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
/// <summary>DisplayName: Account, OwnershipType: UserOwned, IntroducedVersion: 1.0</summary>
public static class Account
{
    public const string EntityName = "account";
    public const string EntityCollectionName = "accounts";

    #region Attributes

    /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
    public const string PrimaryKey = "accountid";
    /// <summary>Type: String, RequiredLevel: None, MaxLength: 160, Format: Text</summary>
    public const string PrimaryName = "name";

    #endregion Attributes

    #region OptionSets
    // ...
    #endregion OptionSets
}
```

Use attribute constants with late-bound `Entity` / `ColumnSet` / `QueryExpression` APIs to avoid string typos.

## Tests

```bash
dotnet test CrmLstg.sln
```

## Related tools

For full **early-bound** types (inherit `Microsoft.Xrm.Sdk.Entity`, `OrganizationServiceContext`), use [PAC CLI modelbuilder](https://learn.microsoft.com/en-us/power-platform/developer/cli/reference/modelbuilder) or `CrmSvcUtil.exe`. This project targets **late-bound** workflows with strong naming and typed POCOs instead.
