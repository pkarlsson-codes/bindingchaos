function Configure-KeycloakAdminCredentials {
    & docker exec -i $KeycloakContainer /opt/keycloak/bin/kcadm.sh config credentials --server http://localhost:8080 --realm master --user $KeycloakAdminUser --password $KeycloakAdminPassword | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to authenticate kcadm against Keycloak."
    }
}

function Reimport-KeycloakRealm {
    Write-Host "[Reset:keycloak] Re-importing realm '$KeycloakRealm' without container restart..."

    & docker exec -i $KeycloakContainer /opt/keycloak/bin/kcadm.sh delete "realms/$KeycloakRealm" | Out-Null
    & docker exec -i $KeycloakContainer /opt/keycloak/bin/kcadm.sh create realms -f "/opt/keycloak/data/import/realm-bindingchaos-dev.json" | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to import Keycloak realm JSON."
    }

    Write-Host "[Reset:keycloak] Done."
}

function Restart-KeycloakContainer {
    Write-Host "[Reset:keycloak] Restarting Keycloak container to apply schema reset..."
    & docker restart $KeycloakContainer | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to restart Keycloak container '$KeycloakContainer'."
    }
}

function Wait-KeycloakReady {
    $maxAttempts = 180
    for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
        try {
            $resp = Invoke-WebRequest -Uri "http://localhost:7080/health/ready" -Method Get -UseBasicParsing -TimeoutSec 5
            if ($resp.StatusCode -eq 200) {
                Write-Host "[Reset:keycloak] Keycloak is ready."
                return
            }
        }
        catch {
            # Keep retrying until timeout
        }

        Start-Sleep -Seconds 1
    }

    throw "Keycloak did not become ready within $maxAttempts seconds."
}

function Invoke-ResetKeycloak {
    Write-Host "[Reset:keycloak] Resetting Keycloak schema and realm import state..."
    Ensure-Postgres
    Ensure-ServiceRunning -Service "keycloak" -ContainerName $KeycloakContainer

    Execute-PostgresSql -Database $PostgresDb -Sql 'DROP SCHEMA IF EXISTS "keycloak" CASCADE;'

    $keycloakMigrationScript = Join-Path $PSScriptRoot "..\keycloak\db-migrate-keycloak.ps1"
    & powershell -ExecutionPolicy Bypass -File $keycloakMigrationScript
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to ensure keycloak schema."
    }

    Restart-KeycloakContainer
    Wait-KeycloakReady
    Configure-KeycloakAdminCredentials
    Reimport-KeycloakRealm
}