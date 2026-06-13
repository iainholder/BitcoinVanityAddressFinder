# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WPF desktop application that finds vanity Bitcoin addresses (addresses containing human-readable strings or dictionary words) using NBitcoin. Targets .NET 10.0 on Windows.

## Build and Test Commands

```bash
# Build everything (app + tests)
dotnet build BitcoinVanityAddressFinder/BitcoinVanityAddressFinder.sln

# Run the app
dotnet run --project BitcoinVanityAddressFinder/BitcoinVanityAddressFinder.csproj

# Run all tests (NUnit, in the separate BitcoinVanityAddressFinder.Tests project)
dotnet test BitcoinVanityAddressFinder.Tests/BitcoinVanityAddressFinder.Tests.csproj

# Run a single test by name
dotnet test BitcoinVanityAddressFinder.Tests/BitcoinVanityAddressFinder.Tests.csproj --filter "FullyQualifiedName~InputStringVerifierTests.IsVanityAddress_WhenContains_Match"
```

## Architecture

**Two projects** in the solution:
- `BitcoinVanityAddressFinder/` — the WPF app (`WinExe`).
- `BitcoinVanityAddressFinder.Tests/` — the NUnit test project (references the app). Tests run via `dotnet test`; the test dependencies (`Microsoft.NET.Test.Sdk`, `NUnit3TestAdapter`) are kept out of the shipped app.

### MVVM Pattern with ViewModelLocator

- `App.xaml` registers `ViewModelLocator` as a static resource (`{StaticResource Locator}`)
- `ViewModelLocator` configures DI via `Microsoft.Extensions.DependencyInjection` and `CommunityToolkit.Mvvm.DependencyInjection.Ioc`
- `MainWindow.xaml` binds its `DataContext` to `VanityAddressViewModel` through the locator
- Uses `CommunityToolkit.Mvvm` (`ObservableObject`, `RelayCommand`, `WeakReferenceMessenger`) — migrated from the older MVVMLight library
- Property changes via `SetProperty(ref field, value)` using semi-auto properties
- Validation through `IDataErrorInfo` interface

### Search Flow

1. `VanityAddressViewModel.Search()` creates a `CancellationTokenSource` and `await`s `VanityAddressService.Search()`
2. `VanityAddressService` spawns N parallel tasks (one per selected core), each generating random `NBitcoin.Key` instances in a loop
3. Each task checks generated addresses against either `InputStringVerifierService` (string mode) or `DictionaryWordVerifierService` (dictionary mode). Each worker gets its own verifier instance, so the verifiers run lock-free on the hot path.
4. `await Task.WhenAny` returns the first match; the others are cancelled via a linked token, then the winning task is awaited (so cancellation surfaces as `OperationCanceledException`)
5. A `DispatcherTimer` (500ms interval) sends attempt counts back to the ViewModel via `WeakReferenceMessenger` using a GUID-based channel token. It is always stopped in a `finally`, even on cancel/error.

### Threading

- `CancellationToken` (linked source) for graceful cancellation and stopping losing workers
- `Interlocked.Increment()` for the thread-safe attempt counter
- `DispatcherTimer` for marshalling UI updates from background threads

## Security Considerations

**CRITICAL**: This application handles Bitcoin private keys.

- **No Logging**: Logging is intentionally disabled to prevent private keys from being written to disk. `Logging.cs` exists but is empty by design.
- **No Persistence**: Search results are not saved. Private keys exist only in UI memory during the session.
- **Do not add**: file logging, result history/persistence, crash reporting that might capture key data, or any feature that writes private key data to disk.

## Key Design Details

- **"Starts with" skips first character**: Bitcoin addresses have a meaningful first character (network prefix), so starts-with matching strips it with `address[1..]` (computed once, not per word)
- **Base58 validation**: vanity text is rejected if it contains characters that can never appear in a Base58 address (`0`, `O`, `I`, `l`), which would otherwise launch an unwinnable, never-ending search. The check accounts for case sensitivity (e.g. when case-insensitive, `O` is allowed because `o` is valid)
- **Dictionary is an embedded resource**: `Services/Dictionary.txt` is compiled as an `EmbeddedResource` and loaded via `Assembly.GetManifestResourceStream`
- **Max 7 characters**: vanity text of 8+ characters is rejected (`Length >= 8`) because longer searches would take impractically long
- **Copy buttons**: the Address and Private Key fields have Copy buttons (`CopyAddressCommand` / `CopyPrivateKeyCommand`) that use the WPF clipboard. This is in-memory only and does not violate the no-persistence rule.
- **CPU core warning**: Using all CPU cores triggers a warning dialog. The default is `Environment.ProcessorCount - 1`
- **Modern C# features in use**: `field` keyword in property setters, collection expressions (`[...]`), pattern matching
- Migrated from .NET 8 to .NET 10 and from MVVMLight to CommunityToolkit.Mvvm
