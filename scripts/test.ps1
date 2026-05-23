Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Running tests..."
dotnet test --configuration Release
