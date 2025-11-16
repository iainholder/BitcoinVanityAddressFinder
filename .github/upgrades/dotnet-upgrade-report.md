# .NET 8 Upgrade Report

## Project target framework modifications

| Project name                                   | Old Target Framework    | New Target Framework         | Commits                   |
|:-----------------------------------------------|:-----------------------:|:----------------------------:|---------------------------|
| BitcoinVanityAddressFinder                     |   net472                | net8.0-windows               | 596d12cb, c655b7c1        |

## NuGet Packages

| Package Name                        | Old Version | New Version | Commit Id                                 |
|:------------------------------------|:-----------:|:-----------:|-------------------------------------------|
| CommunityToolkit.Mvvm               | N/A         | 8.4.0       | c655b7c1                                  |
| Microsoft.Bcl.AsyncInterfaces       | N/A         | 8.0.0       | c655b7c1                                  |
| Microsoft.Extensions.DependencyInjection.Abstractions | N/A | 8.0.2 | c655b7c1 |
| Microsoft.Extensions.Logging.Abstractions | N/A | 8.0.3 | c655b7c1 |
| MvvmLight                           | N/A         | Removed      | c655b7c1                                  |
| MvvmLightLibs                       | N/A         | Removed      | c655b7c1                                  |
| System.Buffers                      | N/A         | Removed      | c655b7c1                                  |
| System.Collections                  | N/A         | Removed      | c655b7c1                                  |
| System.Collections.Concurrent       | N/A         | Removed      | c655b7c1                                  |
| System.Diagnostics.Debug            | N/A         | Removed      | c655b7c1                                  |
| System.Diagnostics.DiagnosticSource | N/A         | 8.0.1        | c655b7c1                                  |
| System.Globalization                | N/A         | Removed      | c655b7c1                                  |
| System.IO                           | N/A         | Removed      | c655b7c1                                  |
| System.Linq                         | N/A         | Removed      | c655b7c1                                  |
| System.Memory                       | N/A         | Removed      | c655b7c1                                  |
| System.Net.Http                     | N/A         | Removed      | c655b7c1                                  |
| System.Net.Requests                 | N/A         | Removed      | c655b7c1                                  |
| System.Numerics.Vectors             | N/A         | Removed      | c655b7c1                                  |
| System.Reflection                   | N/A         | Removed      | c655b7c1                                  |
| System.Resources.ResourceManager    | N/A         | Removed      | c655b7c1                                  |
| System.Runtime                      | N/A         | Removed      | c655b7c1                                  |
| System.Runtime.Extensions           | N/A         | Removed      | c655b7c1                                  |
| System.Runtime.InteropServices      | N/A         | Removed      | c655b7c1                                  |
| System.Security.Cryptography.Algorithms | N/A     | Removed      | c655b7c1                                  |
| System.Security.Cryptography.Encoding | N/A      | Removed      | c655b7c1                                  |
| System.Security.Cryptography.Primitives | N/A   | Removed      | c655b7c1                                  |
| System.Security.Cryptography.X509Certificates | N/A | Removed      | c655b7c1                                  |
| System.Threading.Tasks.Extensions   | N/A         | Removed      | c655b7c1                                  |
| System.ValueTuple                   | N/A         | Removed      | c655b7c1                                  |

## All commits

| Commit ID              | Description                                |
|:-----------------------|:-------------------------------------------|
| 8412521d               | Commit upgrade plan                        |
| 0b265fa9               | Added using directive for Microsoft.Extensions.DependencyInjection.Extensions |
| 596d12cb               | Modernize project: migrate to SDK-style .csproj |
| 8100f113               | Replaced MvvmLight messaging with CommunityToolkit.Mvvm.Messaging |
| 5e0e1d21               | Replaced RaisePropertyChanged with OnPropertyChanged |
| b6bf7398               | Migrated ViewModel to CommunityToolkit.Mvvm |
| c0f603d8               | Updated namespaces and messaging API |
| 9fb71363               | Commit changes before fixing errors        |
| ac5c4e1f               | Fixed messaging to use string for int values |
| 1ab2917d               | Migrated IoC from MvvmLight to CommunityToolkit |
| e19a3a92               | Fixed messaging types in ViewModel |
| 9c5f970d               | Replaced RaiseCanExecuteChanged with NotifyCanExecuteChanged |
| 2537fc41               | Added using for DependencyInjection |
| aefc0531               | Removed unnecessary using |
| 6c2e3c00               | Added using for extensions |
| 8d247751               | Store final changes for step 'Upgrade BitcoinVanityAddressFinder.csproj' |
| c655b7c1               | Update BitcoinVanityAddressFinder.csproj dependencies |

## Project feature upgrades

Contains summary of modifications made to the project assets during different upgrade stages.

### BitcoinVanityAddressFinder

Here is what changed for the project during upgrade:

- Converted project to SDK-style .csproj targeting .NET 8.0 Windows.
- Updated NuGet packages to .NET 8 compatible versions.
- Removed deprecated MvvmLight packages and replaced with CommunityToolkit.Mvvm.
- Migrated MVVM code from MvvmLight to CommunityToolkit.Mvvm, updating namespaces, base classes, property change notifications, commands, and messaging.
- Added Microsoft.Extensions.DependencyInjection package to resolve BuildServiceProvider extension method.

## Next steps

- Test the application to ensure all functionality works correctly with .NET 8 and CommunityToolkit.Mvvm.
- Consider updating any remaining dependencies if needed.