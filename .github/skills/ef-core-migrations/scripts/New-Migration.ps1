param(
    [Parameter(Mandatory = $true)]
    [string]$Name,

    [string]$ProjectPath,
    [string]$Context = "HabitTrackerDbContext"
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $scriptDirectory = if ($PSScriptRoot) { $PSScriptRoot } elseif ($PSCommandPath) { Split-Path -Parent $PSCommandPath } else { Get-Location }
    $ProjectPath = (Resolve-Path (Join-Path $scriptDirectory "..\..\..\..")).Path
}

Push-Location $ProjectPath
try {
    Write-Host "Creating EF Core migration '$Name' for context '$Context'..."

    & dotnet ef migrations add $Name `
        --project (Join-Path $ProjectPath "HabitTracker.csproj") `
        --startup-project (Join-Path $ProjectPath "HabitTracker.csproj") `
        --context $Context `
        --output-dir Migrations

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet ef migrations add failed with exit code $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}
