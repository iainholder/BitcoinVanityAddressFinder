# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10 upgrade.
3. Upgrade BitcoinVanityAddressFinder.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                         |
|:------------------------------------|:---------------:|:-----------:|:------------------------------------|

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### BitcoinVanityAddressFinder\BitcoinVanityAddressFinder.csproj modifications

Project properties changes:
  - Target framework should be changed from net8.0-windows to net10.0-windows

NuGet packages changes:

Feature upgrades:
  - Convert project to SDK-style project

Other changes: