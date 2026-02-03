# CLAUDE.md - AI Assistant Guide

This document provides essential context for AI assistants working with the Bitcoin Vanity Address Finder codebase.

## Project Overview

**Bitcoin Vanity Address Finder** is a Windows WPF desktop application that generates Bitcoin addresses containing human-readable patterns (vanity addresses). For example, finding an address like `1LoveBPzzD72PUXLzCkYAtGFYmK5vYNR33` that contains "Love".

- **Framework**: .NET 10.0-windows
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Primary Library**: NBitcoin for Bitcoin cryptographic operations

## Directory Structure

```
BitcoinVanityAddressFinder/
├── BitcoinVanityAddressFinder/     # Main project directory
│   ├── Services/                   # Business logic
│   │   ├── VanityAddressService.cs      # Multi-threaded address generation
│   │   ├── InputStringVerifierService.cs # Pattern matching for user strings
│   │   ├── DictionaryWordVerifierService.cs # Dictionary word matching
│   │   ├── ServiceFactory.cs            # Factory for creating services
│   │   └── Dictionary.txt               # 58k+ word dictionary (embedded)
│   ├── ViewModel/                  # MVVM ViewModels
│   │   ├── VanityAddressViewModel.cs    # Main application logic
│   │   └── ViewModelLocator.cs          # DI configuration
│   ├── Converters/                 # WPF value converters
│   ├── Tests/                      # NUnit tests
│   ├── MainWindow.xaml/.xaml.cs    # Main UI
│   ├── App.xaml/.xaml.cs           # Application entry point
│   └── BitcoinVanityAddressFinder.csproj
├── README.md
├── TODO.txt                        # Feature tracking
└── .github/upgrades/               # Framework upgrade docs
```

## Build and Run

```bash
# Build the project
dotnet build BitcoinVanityAddressFinder/BitcoinVanityAddressFinder.csproj

# Run the application
dotnet run --project BitcoinVanityAddressFinder/BitcoinVanityAddressFinder.csproj

# Run tests
dotnet test BitcoinVanityAddressFinder/BitcoinVanityAddressFinder.csproj
```

Alternatively, open `BitcoinVanityAddressFinder.sln` in Visual Studio.

## Key Dependencies

| Package | Purpose |
|---------|---------|
| NBitcoin 9.0.3 | Bitcoin address generation and cryptography |
| CommunityToolkit.Mvvm 8.4.0 | MVVM implementation (ObservableObject, RelayCommand) |
| Microsoft.Extensions.DependencyInjection 10.0.0 | Dependency injection |
| NUnit 4.4.0 | Unit testing framework |

## Code Conventions

### Naming
- **Properties/Methods**: PascalCase
- **Private fields**: `_camelCase` with underscore prefix
- **Enums**: PascalCase (e.g., `SearchMode.String`, `SearchMode.Dictionary`)

### MVVM Patterns
- ViewModels inherit from `ObservableObject` (CommunityToolkit.Mvvm)
- Commands use `RelayCommand`
- Property changes via `SetProperty(ref field, value)`
- Cross-component communication via `WeakReferenceMessenger`
- Validation through `IDataErrorInfo` interface

### Modern C# Features Used
- Collection expressions: `[item1, item2]` syntax
- Semi-auto properties: `set => SetProperty(ref field, value);`
- Pattern matching in validation logic

### Threading
- `SemaphoreSlim` for managing concurrent tasks
- `CancellationToken` for graceful cancellation
- `Interlocked.Increment()` for thread-safe counters
- `DispatcherTimer` for UI updates from background threads

## Security Considerations

**CRITICAL**: This application handles Bitcoin private keys.

1. **No Logging**: Logging is intentionally disabled to prevent private keys from being written to disk. The `Logging.cs` file exists but is empty by design.

2. **No Persistence**: Search results are not saved. Private keys exist only in UI memory during the session.

3. **Do not add**:
   - File logging of any kind
   - Result history/persistence
   - Crash reporting that might capture key data
   - Any feature that writes private key data to disk

## Testing

Tests are located in `BitcoinVanityAddressFinder/Tests/`:

- `InputStringVerifierTests.cs` - Tests for string pattern matching
- `DictionaryWordVerifierTests.cs` - Tests for dictionary word matching

Tests use NUnit with `[TestCase]` attributes for parametrized testing.

```bash
# Run all tests
dotnet test BitcoinVanityAddressFinder/BitcoinVanityAddressFinder.csproj
```

## Key Files for Common Tasks

| Task | File(s) |
|------|---------|
| Add search options | `VanityAddressViewModel.cs`, `MainWindow.xaml` |
| Modify pattern matching | `InputStringVerifierService.cs` |
| Change address generation | `VanityAddressService.cs` |
| Add UI elements | `MainWindow.xaml`, create converters in `Converters/` |
| Add tests | `Tests/` directory, follow existing patterns |

## Application Features

- Search for patterns up to 8 characters (longer = exponentially harder)
- Dictionary search mode (58k+ English words)
- Case-sensitive/insensitive matching
- "Starts with" / "Ends with" position matching
- Multi-threaded search (configurable cores, defaults to N-1)
- Multiple Bitcoin networks: Main, TestNet, RegTest
- Audio notification on completion

## Common Gotchas

1. **First character constraint**: Bitcoin addresses start with specific characters based on the network (e.g., '1' for mainnet). The "starts with" search applies after this first character.

2. **Max 8 characters**: The UI enforces a maximum of 8 characters for vanity text because longer searches would take impractically long.

3. **CPU core warning**: Using all CPU cores triggers a warning dialog. The default is `Environment.ProcessorCount - 1`.

4. **Dictionary is embedded**: `Services/Dictionary.txt` is an embedded resource, not read from disk at runtime.

## Development Notes

- The project recently migrated from .NET 8 to .NET 10
- Migrated from MVVMLight to CommunityToolkit.Mvvm
- Uses SDK-style project file (modern .csproj format)
- ReSharper annotations in `Properties/Annotations.cs` for code analysis
