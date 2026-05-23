Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Restoring packages..."
dotnet restore

Write-Host "Building solution..."
dotnet build --configuration Release
