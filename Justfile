# Justfile .NET - BEN ABT 2025 - https://benjamin-abt.com

set shell := ["pwsh", "-c"]

# ===== Configurable defaults =====
CONFIG  := "Debug"
TFM     := "net10.0"

# ===== Default / Help =====
default: help

help:
    # Overview:
    just --list
    # Usage:
    #   just build
    #   just test
    #   just pack

# ===== Basic .NET Workflows =====
restore:
    dotnet restore

build *ARGS:
    dotnet build --configuration "{{CONFIG}}" --nologo --verbosity minimal {{ARGS}}

rebuild *ARGS:
    dotnet build --configuration "{{CONFIG}}" --nologo --verbosity minimal --no-incremental {{ARGS}}

clean:
    dotnet clean --configuration "{{CONFIG}}" --nologo

# ===== Quality / Tests =====
format:
    dotnet format --verbosity minimal

format-check:
    dotnet format --verify-no-changes --verbosity minimal

# xunit.v3 uses Microsoft Testing Platform (MTP) - run tests via dotnet run (not dotnet test)
test *ARGS:
    dotnet run --project "tests/AssemblyMetadata.UnitTests/AssemblyMetadata.UnitTests.csproj" --configuration "{{CONFIG}}" --framework "{{TFM}}" {{ARGS}}

test-cov:
    dotnet test --configuration "{{CONFIG}}" --nologo -- --coverage --coverage-output "./TestResults/coverage/coverage.xml" --coverage-output-format xml

test-filter QUERY:
    dotnet run --project "tests/AssemblyMetadata.UnitTests/AssemblyMetadata.UnitTests.csproj" --configuration "{{CONFIG}}" --framework "{{TFM}}" -- --filter "{{QUERY}}"

# ===== Packaging / Release =====
pack *ARGS:
    dotnet pack --configuration "{{CONFIG}}" --nologo --verbosity minimal -o "./artifacts/packages" {{ARGS}}

# ===== Housekeeping =====
clean-artifacts:
    if (Test-Path "./artifacts") { Remove-Item "./artifacts" -Recurse -Force }

clean-all:
    just clean
    just clean-artifacts

# ===== Combined Flows =====
fmt-build:
    just format
    just build

ci:
    just clean
    just restore
    just format-check
    just build
    just test
