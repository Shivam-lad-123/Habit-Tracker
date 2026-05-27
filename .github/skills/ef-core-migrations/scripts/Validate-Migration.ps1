param(
    [string]$ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..\..")).Path,
    [string]$Context = "HabitTrackerDbContext"
)

$ErrorActionPreference = "Stop"

Push-Location $ProjectPath
try {
    Write-Host "Building Habit Tracker..."
    & dotnet build (Join-Path $ProjectPath "HabitTracker.csproj")
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE."
    }

    Write-Host "Listing migrations for context '$Context'..."
    & dotnet ef migrations list `
        --project (Join-Path $ProjectPath "HabitTracker.csproj") `
        --startup-project (Join-Path $ProjectPath "HabitTracker.csproj") `
        --context $Context

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet ef migrations list failed with exit code $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}
