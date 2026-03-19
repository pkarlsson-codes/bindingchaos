param (
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$ContextProjectPath,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$DbContextName,

    [Parameter(Mandatory = $true, Position = 2)]
    [string]$MigrationName
)

Write-Host "Adding EF Core migration '$MigrationName'..."
Write-Host "Context project: $ContextProjectPath"
Write-Host "DbContext: $DbContextName"

dotnet ef migrations add $MigrationName `
    --project $ContextProjectPath `
    --context $DbContextName `
    --output-dir Infrastructure/Persistence/Migrations
