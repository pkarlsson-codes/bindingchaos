Param(
    [string]$ContainerName = "bindingchaos_postgres",
    [string]$Database = "bindingchaos",
    [string]$User = "postgres",
    [string]$Schema = "keycloak"
)

Write-Host "[Keycloak DB] Ensuring schema '$Schema' exists in database '$Database' on container '$ContainerName'..."

# Ensure postgres container is up
$container = & docker ps -a --format "{{.Names}}" | Where-Object { $_ -eq $ContainerName }
if (-not $container) {
    Write-Host "[Keycloak DB] Postgres container '$ContainerName' not found. Starting 'postgres' service via docker-compose..."
    & docker-compose up -d postgres | Out-Null
}

$isRunning = & docker inspect -f '{{.State.Running}}' $ContainerName 2>$null
if ($LASTEXITCODE -ne 0 -or $isRunning -ne 'true') {
    Write-Host "[Keycloak DB] Starting container '$ContainerName'..."
    & docker start $ContainerName | Out-Null
}

# Create schema if not exists (quote identifier safely)
$sql = 'CREATE SCHEMA IF NOT EXISTS "{0}" AUTHORIZATION {1};' -f $Schema, $User
Write-Host "[Keycloak DB] Executing: $sql"
& docker exec -i $ContainerName psql -U $User -d $Database -c $sql

if ($LASTEXITCODE -eq 0) {
    Write-Host "[Keycloak DB] Schema '$Schema' ensured."
    exit 0
} else {
    Write-Host "[Keycloak DB] Failed to ensure schema '$Schema'."
    exit 1
}


