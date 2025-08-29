# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

HEC-FDA (Hydrologic Engineering Center - Flood Damage Reduction Analysis) is a WPF application built with .NET 9.0 for flood damage analysis. It follows an MVVM architecture pattern with a clear separation of concerns across multiple layers.

## Architecture

### Core Components

**Model Layer** (`HEC.FDA.Model/`)
- Contains domain models for flood damage calculations
- Includes spatial analysis, hydraulics, structures, scenarios, and alternatives
- Key concepts: ImpactAreaScenarioSimulation, Structure, Inventory, StageDamage

**Statistics Layer** (`HEC.FDA.Statistics/`)
- Provides statistical distributions (Normal, LogNormal, Triangular, etc.)
- Handles convergence criteria and histograms
- Core for uncertainty analysis in flood damage computations

**ViewModel Layer** (`HEC.FDA.ViewModel/`)
- Implements business logic and UI state management
- Contains elements for alternatives, scenarios, hydraulics, inventory
- Manages computation workflows and results visualization

**View Layer** (`HEC.FDA.View/`)
- WPF XAML views and user controls
- Uses data binding to ViewModels
- Includes specialized editors for curves, stage damage, and alternatives

**Importer** (`HEC.FDA.Importer/`)
- Handles data import from FDA1 (legacy version)
- Manages DBF file reading and data migration

### MVVM Framework

The project includes a custom MVVM framework (`HEC.MVVMFramework.*`) that provides:
- Base classes for ViewModels with property change notification
- Message hub for component communication
- Validation framework with rules and error reporting
- Command pattern implementation

## Build Commands

```bash
# Build the entire solution
dotnet build Fda.sln

# Build in Release mode
dotnet build Fda.sln -c Release

# Clean build artifacts
dotnet clean Fda.sln
```

## Test Commands

```bash
# Run all tests
dotnet test Fda.sln

# Run tests with detailed output
dotnet test Fda.sln --logger "console;verbosity=detailed"

# Run a specific test project
dotnet test HEC.FDA.ModelTest/HEC.FDA.ModelTest.csproj

# Run tests matching a filter
dotnet test --filter "FullyQualifiedName~HEC.FDA.ModelTest"

# Run a single test method
dotnet test --filter "FullyQualifiedName=HEC.FDA.ModelTest.unittests.StageDamageShould.TestMethodName"
```

## Development Setup

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or later (for WPF development)
- Windows OS (WPF application)

### NuGet Configuration
The project requires multiple NuGet sources configured in `nuget.config`:
1. **HEC Nexus**: https://www.hec.usace.army.mil/nexus/repository/fda-nuget/
2. **GitHub Packages**: Requires Personal Access Token - See [GitHub Discussion #170](https://github.com/HydrologicEngineeringCenter/HEC-FDA/discussions/170)
3. **DSS NuGet**: https://www.hec.usace.army.mil/nexus/repository/dss/
4. **RAS Public/Private**: Requires DevNet credentials for private repository

### Publishing
```bash
# Publish the main application
dotnet publish HEC.FDA.View/HEC.FDA.View.csproj -c Release --self-contained true
```

## Key Test Projects

- **HEC.FDA.ModelTest**: Unit and integration tests for the Model layer
- **HEC.FDA.StatisticsTest**: Tests for statistical distributions and calculations
- **HEC.FDA.ViewModelTest**: Tests for ViewModel logic
- **HEC.MVVMFramework.BaseTest**: Framework component tests

## Important Patterns

### Element Pattern
Most domain objects follow an "Element" pattern where:
- `*Element` classes inherit from `BaseFdaElement` 
- `*OwnerElement` classes manage collections of elements
- Elements handle persistence through SQLite

### Computation Flow
1. User creates Scenarios with ImpactAreas
2. Scenarios reference hydraulics, inventory, and stage-damage functions
3. Computations run Monte Carlo simulations using the Statistics library
4. Results stored as metrics (damages, performance, consequences)

### Data Import
The Importer namespace handles migration from FDA1 format using DBF file readers and data transformation classes.

## Common Development Tasks

### Running the Application
```bash
# Run the main WPF application
dotnet run --project HEC.FDA.View/HEC.FDA.View.csproj
```

### Working with Tests
When modifying domain models or computation logic, ensure tests pass:
- Model layer changes: Run `HEC.FDA.ModelTest`
- Statistics changes: Run `HEC.FDA.StatisticsTest`
- ViewModel changes: Run `HEC.FDA.ViewModelTest`

### Database
The application uses SQLite for local persistence of FDA elements (scenarios, alternatives, hydraulics, etc.)

## Code Conventions

### Naming
- Elements: `*Element` suffix for domain objects
- ViewModels: `*VM` suffix
- Owner elements: `*OwnerElement` for collection managers
- Test classes: `*Should` or `*Test` suffix

### Testing Patterns
- Unit tests use descriptive method names explaining what is being tested
- Integration tests include realistic scenario data
- Tests for statistical computations verify convergence and accuracy