# Generate OpenAPI Specification and Frontend API Clients
# This script generates the OpenAPI spec from the compiled .NET API and then generates TypeScript API clients

param(
    [string]$Configuration = "Debug",
    [string]$OutputPath = "src/BindingChaos.Web/api-spec.json"
)

Write-Host "Generating OpenAPI specification and API clients..." -ForegroundColor Green

# Determine the project root directory (where this script is located)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptPath

Write-Host "Project root: $projectRoot" -ForegroundColor Yellow

# Resolve target framework from the project file
$csprojPath = Join-Path $projectRoot "src/BindingChaos.Web.Gateway/BindingChaos.Web.Gateway.csproj"
[xml]$proj = Get-Content $csprojPath
$tfm = $proj.Project.PropertyGroup.TargetFramework
if (-not $tfm -or [string]::IsNullOrWhiteSpace($tfm)) {
    $tfms = $proj.Project.PropertyGroup.TargetFrameworks
    if ($tfms) {
        # If TargetFrameworks is present, pick the first
        $tfm = ($tfms -split ';')[0]
    }
}
if ($tfm) {
    if ($tfm -is [System.Xml.XmlElement]) {
        $tfm = $tfm.InnerText
    }
    $tfm = [string]$tfm
    $tfm = $tfm.Trim()
}
if (-not $tfm) {
    # Fallback if not detected
    $tfm = "net10.0"
}

# Build the Web Gateway API project
Write-Host "Building Web Gateway API project (TFM=$tfm)..." -ForegroundColor Yellow
dotnet build "$csprojPath" --configuration $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create output directory if it doesn't exist
$outputDir = Split-Path "$projectRoot/$OutputPath" -Parent
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

# Ensure NSwag CLI is installed
Write-Host "Ensuring NSwag CLI..." -ForegroundColor Yellow
$null = (dotnet tool update --global NSwag.ConsoleCore 2>$null)
if ($LASTEXITCODE -ne 0) {
    dotnet tool install --global NSwag.ConsoleCore
}

# Generate OpenAPI spec using NSwag CLI (no need for Startup)
Write-Host "Generating OpenAPI spec via NSwag..." -ForegroundColor Yellow
nswag aspnetcore2openapi /project:"$csprojPath" /output:"$projectRoot/$OutputPath" /configuration:$Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to generate OpenAPI spec!" -ForegroundColor Red
    exit 1
}

Write-Host "OpenAPI spec generated successfully at: $projectRoot/$OutputPath" -ForegroundColor Green

# Install dependencies if needed
$frontendPath = "$projectRoot/src/BindingChaos.Web"
if (-not (Test-Path "$frontendPath/node_modules")) {
    Write-Host "Installing frontend dependencies..." -ForegroundColor Yellow
    npm --prefix "$frontendPath" install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to install dependencies!" -ForegroundColor Red
        exit 1
    }
}

# Generate TypeScript types from OpenAPI spec
Write-Host "Generating TypeScript API clients..." -ForegroundColor Yellow
npm --prefix "$frontendPath" run generate-api-from-spec

if ($LASTEXITCODE -eq 0) {
    Write-Host "API clients generated successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to generate API clients!" -ForegroundColor Red
    exit 1
}

Write-Host "OpenAPI specification and API clients generation completed successfully!" -ForegroundColor Green
