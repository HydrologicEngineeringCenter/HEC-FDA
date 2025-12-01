# Copilot Instructions for HEC-FDA

## Repository Overview
HEC-FDA (Hydrologic Engineering Center - Flood Damage Reduction Analysis) is a large WPF desktop application for flood damage analysis built with .NET 9.0. The repository contains ~250+ files following MVVM architecture with a custom framework. Target platform is Windows only (WPF).

**Languages**: C# (primary), VB.NET (HEC.FDA.UserControls only)  
**Framework**: .NET 9.0, WPF  
**Size**: Large enterprise application with multiple layers  

## Build & Test Commands

### Prerequisites
- .NET 9.0 SDK required
- Windows OS required (WPF application)
- Visual Studio 2022+ recommended for WPF development

### Critical Build Steps
**ALWAYS clean and build in this order to avoid file lock issues:**
```bash
# 1. Always clean first if rebuilding
dotnet clean Fda.sln

# 2. Build entire solution (takes ~30-60 seconds)
dotnet build Fda.sln -c Release

# 3. If build fails with file locks, close Visual Studio and any running instances of HEC.FDA.View.exe
# Common error: "The file is locked by: Microsoft Visual Studio 2022, HEC.FDA.View"
```

### Testing
```bash
# Run CI tests only (fast, <30 seconds)
dotnet test --nologo --filter "RunsOn=Remote" -c Release

# Run ALL tests including local tests
dotnet test Fda.sln

# Run specific test project
dotnet test HEC.FDA.ModelTest/HEC.FDA.ModelTest.csproj
```

### Running the Application
```bash
dotnet run --project HEC.FDA.View/HEC.FDA.View.csproj
```

## Project Structure & Key Files

### Solution Architecture
```
Fda.sln                           # Main solution file
├── HEC.FDA.Model/               # Domain models, flood calculations
├── HEC.FDA.Statistics/          # Statistical distributions, Monte Carlo
├── HEC.FDA.ViewModel/           # Business logic, UI state (WPF ViewModels)
├── HEC.FDA.View/                # WPF Views, XAML, Main application entry
├── HEC.FDA.Importer/            # FDA1 legacy data import
├── HEC.MVVMFramework.*/         # Custom MVVM framework components
├── *.Test/                      # Test projects for each layer
└── ScratchSpace/                # Experimental code (ignore for production)
```

### Configuration Files
- `nuget.config`: Contains 6 package sources including private feeds requiring authentication
- `Directory.Build.props`: Sets global MSBuild properties (TargetFramework=net9.0)
- `.github/workflows/CI.yaml`: CI pipeline configuration
- `CLAUDE.md`: Project-specific instructions (if exists, read this first)

### Main Entry Point
`HEC.FDA.View/HEC.FDA.View.csproj` - The WPF application project

## Validation & CI Requirements

### Before Committing
1. **Run remote tests**: `dotnet test --filter "RunsOn=Remote" -c Release`
2. **Ensure no build warnings in core projects** (warnings in ScratchSpace can be ignored)
3. **Follow naming conventions**:
   - Elements: `*Element` suffix
   - ViewModels: `*VM` suffix  
   - Test classes: `*Should` suffix
   - Tests: Method names complete sentence started by class name

### CI Pipeline Checks (GitHub Actions)
- Triggers on PR to main branch
- Runs remote tests only (`RunsOn=Remote`)
- Requires NuGet authentication (handled in CI)
- Creates self-contained deployment package

## Common Issues & Workarounds

### File Lock Errors During Build
**Problem**: "The process cannot access the file because it is being used by another process"  
**Solution**: Close Visual Studio and any running HEC.FDA.View.exe processes, then rebuild

### NuGet Package Restore Failures
**Problem**: Authentication required for private feeds  
**Solution**: See GitHub Discussion #170 for setting up GitHub Personal Access Token

### Test Attribute Requirements
**Important**: All tests MUST have `RunsOn` attribute set to either `Local` or `Remote`
- Remote tests: Complete in <5 seconds
- Local tests: Longer running, not run in CI

## Key Patterns & Conventions

### Element Pattern
- `*Element` classes inherit from `BaseFdaElement`
- `*OwnerElement` classes manage collections
- Elements handle SQLite persistence

### Data Binding
- ViewModels implement `INotifyPropertyChanged` via framework
- Use `BaseFdaElement` for domain objects
- Follow MVVM strictly - no code-behind in Views

### Database
- SQLite for local persistence
- Connection strings in app.config
- Elements auto-persist through framework

## Quick Reference

**Build**: `dotnet build Fda.sln -c Release`  
**Test (CI)**: `dotnet test --filter "RunsOn=Remote" -c Release`  
**Run**: `dotnet run --project HEC.FDA.View/HEC.FDA.View.csproj`  
**Clean**: `dotnet clean Fda.sln`  
**Publish**: `dotnet publish HEC.FDA.View/HEC.FDA.View.csproj -c Release --self-contained true`

## Important Notes
- ALWAYS read existing code patterns before implementing new features
- NEVER assume libraries are available - check package.json/csproj first
- Follow existing MVVM patterns strictly
- Windows-only application (WPF)
- Close all instances before rebuilding to avoid file locks

**Trust these instructions.** Only search for additional information if these instructions are incomplete or found to be in error.