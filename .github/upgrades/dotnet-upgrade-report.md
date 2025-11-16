# .NET 10 Upgrade Report

## Project target framework modifications

| Project name                                   | Old Target Framework    | New Target Framework         | Commits                   |
|:-----------------------------------------------|:-----------------------:|:----------------------------:|---------------------------|
| BitcoinVanityAddressFinder                     |   net8.0-windows        | net10.0-windows              | 8eb3f321                  |

## NuGet Packages

| Package Name                        | Old Version | New Version | Commit Id                                 |
|:------------------------------------|:-----------:|:-----------:|-------------------------------------------|
| System.Diagnostics.DiagnosticSource | 8.0.1       | Removed      | 3471f3ab                                  |
| System.Runtime.CompilerServices.Unsafe | 6.1.2   | Removed      | 3471f3ab                                  |

## All commits

| Commit ID              | Description                                |
|:-----------------------|:-------------------------------------------|
| 19879339               | Commit upgrade plan                        |
| 8eb3f321               | Update target framework in BitcoinVanityAddressFinder.csproj |
| 3471f3ab               | Remove unused package references from csproj file |

## Project feature upgrades

Contains summary of modifications made to the project assets during different upgrade stages.

### BitcoinVanityAddressFinder

Here is what changed for the project during upgrade:

- Updated target framework from .NET 8.0 to .NET 10.0 Windows.
- Removed unused package references for System.Diagnostics.DiagnosticSource and System.Runtime.CompilerServices.Unsafe.

## Next steps

- Test the application to ensure all functionality works correctly with .NET 10.
- Monitor for any new .NET 10 specific features or improvements that could be utilized.