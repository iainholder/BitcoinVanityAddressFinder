# .NET 8 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 8 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8 upgrade.
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
| CommunityToolkit.Mvvm               | N/A             | 8.4.0       | Replacement for deprecated MvvmLight and MvvmLightLibs |
| Microsoft.Bcl.AsyncInterfaces       | 10.0.0         | 8.0.0       | Recommended for .NET 8              |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.0 | 8.0.2 | Recommended for .NET 8 |
| Microsoft.Extensions.Logging.Abstractions | 10.0.0 | 8.0.3 | Recommended for .NET 8 |
| MvvmLight                           | 5.4.1.1        | Remove       | Deprecated, replaced by CommunityToolkit.Mvvm |
| MvvmLightLibs                       | 5.4.1.1        | Remove       | Deprecated, replaced by CommunityToolkit.Mvvm |
| System.Buffers                      | 4.6.1          | Remove       | Package functionality included with new framework reference |
| System.Collections                  | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Collections.Concurrent       | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Diagnostics.Debug            | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Diagnostics.DiagnosticSource | 10.0.0         | 8.0.1        | Recommended for .NET 8              |
| System.Globalization                | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.IO                           | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Linq                         | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Memory                       | 4.6.3          | Remove       | Package functionality included with new framework reference |
| System.Net.Http                     | 4.3.4          | Remove       | Package functionality included with new framework reference |
| System.Net.Requests                 | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Numerics.Vectors             | 4.6.1          | Remove       | Package functionality included with new framework reference |
| System.Reflection                   | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Resources.ResourceManager    | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Runtime                      | 4.3.1          | Remove       | Package functionality included with new framework reference |
| System.Runtime.Extensions           | 4.3.1          | Remove       | Package functionality included with new framework reference |
| System.Runtime.InteropServices      | 4.3.0          | Remove       | Package functionality included with new framework reference |
| System.Security.Cryptography.Algorithms | 4.3.1     | Remove       | Package functionality included with new framework reference |
| System.Security.Cryptography.Encoding | 4.3.0      | Remove       | Package functionality included with new framework reference |
| System.Security.Cryptography.Primitives | 4.3.0   | Remove       | Package functionality included with new framework reference |
| System.Security.Cryptography.X509Certificates | 4.3.2 | Remove       | Package functionality included with new framework reference |
| System.Threading.Tasks.Extensions   | 4.6.3          | Remove       | Package functionality included with new framework reference |
| System.ValueTuple                   | 4.6.1          | Remove       | Package functionality included with new framework reference |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### BitcoinVanityAddressFinder\BitcoinVanityAddressFinder.csproj modifications

Project properties changes:
  - Target framework should be changed from .NETFramework,Version=v4.7.2 to net8.0-windows

NuGet packages changes:
  - CommunityToolkit.Mvvm should be added at version 8.4.0 (replacement for deprecated MvvmLight and MvvmLightLibs)
  - Microsoft.Bcl.AsyncInterfaces should be updated from 10.0.0 to 8.0.0 (recommended for .NET 8)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from 10.0.0 to 8.0.2 (recommended for .NET 8)
  - Microsoft.Extensions.Logging.Abstractions should be updated from 10.0.0 to 8.0.3 (recommended for .NET 8)
  - MvvmLight should be removed (deprecated)
  - MvvmLightLibs should be removed (deprecated)
  - System.Buffers should be removed (package functionality included with new framework reference)
  - System.Collections should be removed (package functionality included with new framework reference)
  - System.Collections.Concurrent should be removed (package functionality included with new framework reference)
  - System.Diagnostics.Debug should be removed (package functionality included with new framework reference)
  - System.Diagnostics.DiagnosticSource should be updated from 10.0.0 to 8.0.1 (recommended for .NET 8)
  - System.Globalization should be removed (package functionality included with new framework reference)
  - System.IO should be removed (package functionality included with new framework reference)
  - System.Linq should be removed (package functionality included with new framework reference)
  - System.Memory should be removed (package functionality included with new framework reference)
  - System.Net.Http should be removed (package functionality included with new framework reference)
  - System.Net.Requests should be removed (package functionality included with new framework reference)
  - System.Numerics.Vectors should be removed (package functionality included with new framework reference)
  - System.Reflection should be removed (package functionality included with new framework reference)
  - System.Resources.ResourceManager should be removed (package functionality included with new framework reference)
  - System.Runtime should be removed (package functionality included with new framework reference)
  - System.Runtime.Extensions should be removed (package functionality included with new framework reference)
  - System.Runtime.InteropServices should be removed (package functionality included with new framework reference)
  - System.Security.Cryptography.Algorithms should be removed (package functionality included with new framework reference)
  - System.Security.Cryptography.Encoding should be removed (package functionality included with new framework reference)
  - System.Security.Cryptography.Primitives should be removed (package functionality included with new framework reference)
  - System.Security.Cryptography.X509Certificates should be removed (package functionality included with new framework reference)
  - System.Threading.Tasks.Extensions should be removed (package functionality included with new framework reference)
  - System.ValueTuple should be removed (package functionality included with new framework reference)

Feature upgrades:
  - Convert project to SDK-style project

Other changes: