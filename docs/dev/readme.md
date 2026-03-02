# Development Environment Setup

This directory contains tools and scripts to help developers set up their local development environment.

## Quick Start

To set up your development environment, run the initialization script:

```powershell
cd docs\dev
.\init.ps1
```

## Scripts

### `init.ps1` - Development Environment Initialization

The `init.ps1` script is an interactive PowerShell script that helps new developers configure their local development environment.

#### Prerequisites

- PowerShell (Windows PowerShell or PowerShell Core)
- Git installed and available in PATH
- Repository cloned locally

#### Usage

1. Navigate to the `docs\dev` directory:
   ```powershell
   cd docs\dev
   ```

2. Run the initialization script:
   ```powershell
   .\init.ps1
   ```

3. Follow the interactive prompts to configure your environment

#### Command Line Options

- **`-SkipConfirmation`**: Automatically confirms all steps without user interaction
- **`-Force`**: Forces configuration even if already set up

Examples:

```powershell
# Interactive mode (default)
.\init.ps1

# Automatic mode (no prompts)
.\init.ps1 -SkipConfirmation

# Force reconfiguration
.\init.ps1 -Force
```

