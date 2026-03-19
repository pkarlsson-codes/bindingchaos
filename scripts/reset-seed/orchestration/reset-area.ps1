Param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("app", "keycloak", "redis", "rabbitmq", "all")]
    [string]$Area,

    [string]$PostgresContainer = "bindingchaos_postgres",
    [string]$PostgresDb = "bindingchaos",
    [string]$PostgresUser = "postgres",
    [string]$PostgresPassword = "postgres",
    [string]$KeycloakContainer = "bindingchaos_keycloak",
    [string]$KeycloakAdminUser = "admin",
    [string]$KeycloakAdminPassword = "admin",
    [string]$KeycloakRealm = "bindingchaos-dev"
)

$ErrorActionPreference = "Stop"

$moduleRoot = Join-Path $PSScriptRoot ".."

. (Join-Path $moduleRoot "shared\reset-shared.ps1")
. (Join-Path $moduleRoot "app\reset-app.ps1")
. (Join-Path $moduleRoot "keycloak\reset-keycloak.ps1")
. (Join-Path $moduleRoot "redis\reset-redis.ps1")
. (Join-Path $moduleRoot "rabbitmq\reset-rabbitmq.ps1")

switch ($Area) {
    "app" {
        Invoke-ResetAppData
    }
    "keycloak" {
        Invoke-ResetKeycloak
    }
    "redis" {
        Invoke-ResetRedis
    }
    "rabbitmq" {
        Invoke-ResetRabbitMq
    }
    "all" {
        Invoke-ResetAppData
        Invoke-ResetKeycloak
        Invoke-ResetRedis
        Invoke-ResetRabbitMq
    }
}

Write-Host "[Reset] Completed area '$Area'."
