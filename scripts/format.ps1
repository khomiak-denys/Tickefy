Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Running format check..."
dotnet format --verify-no-changes
