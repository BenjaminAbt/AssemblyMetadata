#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Initial setup script for developers to configure their development environment
.DESCRIPTION
    This script helps new developers set up their local development environment. It includes Git configuration, hook setup,
    and other necessary development tools.

    IMPORTANT: This script must be run from the docs\dev directory!
.EXAMPLE
    cd docs\dev
    .\init.ps1
#>

param(
    [switch]$Force,
    [switch]$SkipConfirmation
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to prompt user for confirmation
function Confirm-Step {
    param(
        [string]$Message,
        [string]$StepName
    )

    if ($SkipConfirmation) {
        Write-Host "Auto-confirming: $StepName" -ForegroundColor Green
        return $true
    }

    do {
        $response = Read-Host "$Message (y/n/s) [y=Yes, n=No, s=Skip]"
        $response = $response.ToLower()

        switch ($response) {
            'y' { return $true }
            'yes' { return $true }
            'n' {
                Write-Host "Setup cancelled by user." -ForegroundColor Yellow
                exit 0
            }
            'no' {
                Write-Host "Setup cancelled by user." -ForegroundColor Yellow
                exit 0
            }
            's' { return $false }
            'skip' { return $false }
            default {
                Write-Host "Please enter 'y' for Yes, 'n' for No, or 's' to Skip." -ForegroundColor Yellow
            }
        }
    } while ($true)
}

# Function to check if running in correct directory and navigate to repository root
function Set-RepositoryRoot {
    $currentPath = Get-Location
    Write-Host "Current directory: $currentPath" -ForegroundColor Gray

    # Check if we're in docs\dev directory
    $currentDir = Split-Path -Leaf $currentPath
    $parentDir = Split-Path -Leaf (Split-Path -Parent $currentPath)

    if ($currentDir -eq "dev" -and $parentDir -eq "docs") {
        Write-Host "✓ Script is running from docs\dev directory" -ForegroundColor Green

        # Navigate to repository root (two levels up)
        $repoRoot = Split-Path -Parent (Split-Path -Parent $currentPath)
        Set-Location $repoRoot
        Write-Host "✓ Changed to repository root: $repoRoot" -ForegroundColor Green

        # Verify .git directory exists
        if (-not (Test-Path ".git")) {
            Write-Host "✗ .git directory not found in repository root" -ForegroundColor Red
            Write-Host "Please ensure this script is run from the correct docs\dev directory." -ForegroundColor Red
            exit 1
        }

        Write-Host "✓ Repository root validated" -ForegroundColor Green
        return $true
    }
    else {
        Write-Host "✗ Error: This script must be run from the docs\dev directory." -ForegroundColor Red
        Write-Host "Current location: $currentPath" -ForegroundColor Red
        Write-Host "Please navigate to docs\dev and run the script again:" -ForegroundColor Yellow
        Write-Host "  cd docs\dev" -ForegroundColor Gray
        Write-Host "  .\init.ps1" -ForegroundColor Gray
        exit 1
    }
}

# Function to display welcome message
function Show-WelcomeMessage {
    Clear-Host
    Write-Host ("=" * 75) -ForegroundColor Cyan
    Write-Host "  Welcome to Environment Setup." -ForegroundColor Cyan
    Write-Host ("=" * 75) -ForegroundColor Cyan
    Write-Host ""
    Write-Host "This script will help you configure your development environment" -ForegroundColor White
    Write-Host ""
    Write-Host "IMPORTANT: This script must be run from the docs\dev directory!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "The following steps will be performed:" -ForegroundColor White
    Write-Host "  1. Welcome and repository validation" -ForegroundColor Gray
    Write-Host "  2. Git configuration and hooks setup" -ForegroundColor Gray
    Write-Host ""
    Write-Host "You can skip any step if it's already configured or not needed." -ForegroundColor Yellow
    Write-Host ""
}

# Function to setup Git configuration and hooks
function Set-GitConfiguration {
    Write-Host ("=" * 50) -ForegroundColor Green
    Write-Host "  Git Configuration and Hooks Setup" -ForegroundColor Green
    Write-Host ("=" * 50) -ForegroundColor Green
    Write-Host ""

    # Check if Git is available
    try {
        $gitVersion = git --version
        Write-Host "✓ Git is available: $gitVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Git is not available or not in PATH" -ForegroundColor Red
        Write-Host "Please install Git and ensure it's in your PATH before continuing." -ForegroundColor Red
        return $false
    }

    # Check and confirm Git email configuration
    Write-Host ""
    Write-Host "Checking Git user configuration..." -ForegroundColor White

    try {
        $gitEmail = git config user.email
        $gitName = git config user.name

        if ($gitEmail) {
            Write-Host "Current Git email: $gitEmail" -ForegroundColor Yellow
            if ($gitName) {
                Write-Host "Current Git name: $gitName" -ForegroundColor Yellow
            }
            Write-Host ""

            do {
                $emailConfirm = Read-Host "Is this the correct email address you want to use for commits? (y/n)"
                $emailConfirm = $emailConfirm.ToLower()

                if ($emailConfirm -eq 'y' -or $emailConfirm -eq 'yes') {
                    Write-Host "✓ Git email configuration confirmed" -ForegroundColor Green
                    break
                }
                elseif ($emailConfirm -eq 'n' -or $emailConfirm -eq 'no') {
                    Write-Host ""
                    Write-Host "Please configure your Git email and name before continuing:" -ForegroundColor Yellow
                    Write-Host "  git config --global user.email 'your.email@example.com'" -ForegroundColor Gray
                    Write-Host "  git config --global user.name 'Your Name'" -ForegroundColor Gray
                    Write-Host ""
                    Write-Host "After configuration, please run this setup script again." -ForegroundColor Yellow
                    return $false
                }
                else {
                    Write-Host "Please enter 'y' for Yes or 'n' for No." -ForegroundColor Yellow
                }
            } while ($true)
        }
        else {
            Write-Host "✗ No Git email configured" -ForegroundColor Red
            Write-Host ""
            Write-Host "Please configure your Git email and name before continuing:" -ForegroundColor Yellow
            Write-Host "  git config --global user.email 'your.email@example.com'" -ForegroundColor Gray
            Write-Host "  git config --global user.name 'Your Name'" -ForegroundColor Gray
            Write-Host ""
            Write-Host "After configuration, please run this setup script again." -ForegroundColor Yellow
            return $false
        }
    }
    catch {
        Write-Host "✗ Failed to check Git configuration: $_" -ForegroundColor Red
        return $false
    }

    # Check current Git hooks configuration
    $currentHooksPath = ""
    try {
        $currentHooksPath = git config core.hooksPath
        if ($currentHooksPath) {
            Write-Host "Current Git hooks path: $currentHooksPath" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "No custom hooks path currently configured." -ForegroundColor Gray
    }

    # Configure Git hooks path
    Write-Host ""
    Write-Host "Setting up Git hooks to prevent direct pushes to main branch..." -ForegroundColor White

    if (-not (Test-Path ".githooks")) {
        Write-Host "✗ .githooks directory not found" -ForegroundColor Red
        Write-Host "Please ensure you're running this script from the repository root." -ForegroundColor Red
        return $false
    }

    if (-not (Test-Path ".githooks\pre-push")) {
        Write-Host "✗ pre-push hook not found in .githooks directory" -ForegroundColor Red
        return $false
    }

    try {
        git config core.hooksPath .githooks
        Write-Host "✓ Git hooks path configured successfully" -ForegroundColor Green

        # Verify configuration
        $verifyHooksPath = git config core.hooksPath
        if ($verifyHooksPath -eq ".githooks") {
            Write-Host "✓ Git hooks path verified: $verifyHooksPath" -ForegroundColor Green
        }
        else {
            Write-Host "⚠ Warning: Hooks path verification failed" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "✗ Failed to configure Git hooks path: $_" -ForegroundColor Red
        return $false
    }

    # Display information about the hooks
    Write-Host ""
    Write-Host "Git Hooks Information:" -ForegroundColor Cyan
    Write-Host "• pre-push hook: Prevents direct pushes to main branch" -ForegroundColor White
    Write-Host "• This enforces a pull request workflow for better code quality" -ForegroundColor White
    Write-Host ""
    Write-Host "To test the hook, try: git push origin main" -ForegroundColor Gray
    Write-Host "You should see an error preventing the push." -ForegroundColor Gray
    Write-Host ""

    return $true
}

# Main execution flow
function Main {
    # Step 1: Welcome Message and Repository Validation
    Show-WelcomeMessage

    if (-not (Confirm-Step "Do you want to proceed with the development environment setup?" "Welcome and Setup")) {
        Write-Host "Setup skipped. You can run this script again anytime." -ForegroundColor Yellow
        exit 0
    }

    # Validate repository
    Set-RepositoryRoot
    Write-Host ""

    # Step 2: Git Setup
    if (Confirm-Step "Do you want to configure Git settings and activate Git hooks?" "Git Configuration") {
        if (Set-GitConfiguration) {
            Write-Host "✓ Git configuration completed successfully" -ForegroundColor Green
        }
        else {
            Write-Host "✗ Git configuration failed" -ForegroundColor Red
            exit 1
        }
    }
    else {
        Write-Host "Git configuration skipped." -ForegroundColor Yellow
    }

    # Setup completion
    Write-Host ""
    Write-Host ("=" * 50) -ForegroundColor Green
    Write-Host "  Setup Complete!" -ForegroundColor Green
    Write-Host ("=" * 50) -ForegroundColor Green
    Write-Host ""
    Write-Host "Your development environment is now configured." -ForegroundColor White
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "• Create a feature branch: git checkout -b feature/your-feature-name" -ForegroundColor White
    Write-Host "• Make your changes and commit them" -ForegroundColor White
    Write-Host "• Push to your feature branch: git push origin feature/your-feature-name" -ForegroundColor White
    Write-Host "• Create a pull request for code review" -ForegroundColor White
    Write-Host ""
    Write-Host "For more information, check the documentation in the docs/ directory." -ForegroundColor Gray
    Write-Host ""
}

# Script entry point
try {
    Main
}
catch {
    Write-Host ""
    Write-Host "An error occurred during setup:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check the error and try again, or contact the development team for assistance." -ForegroundColor Yellow
    exit 1
}