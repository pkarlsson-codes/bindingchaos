function Invoke-Compose {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Args)

    if (Get-Command docker-compose -ErrorAction SilentlyContinue) {
        & docker-compose @Args
        return
    }

    & docker compose @Args
}

function Ensure-Postgres {
    Write-Host "[Reset] Ensuring postgres is running..."

    $exists = & docker ps -a --format "{{.Names}}" | Where-Object { $_ -eq $PostgresContainer }
    if (-not $exists) {
        Write-Host "[Reset] Postgres container '$PostgresContainer' not found. Creating via compose..."
        Invoke-Compose up -d postgres
    }
    else {
        & docker start $PostgresContainer | Out-Null
    }

    $maxAttempts = 30
    for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
        & docker exec -e PGPASSWORD=$PostgresPassword -i $PostgresContainer pg_isready -h localhost -U $PostgresUser -d $PostgresDb | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[Reset] Postgres is ready."
            return
        }

        Start-Sleep -Seconds 1
    }

    throw "Postgres did not become ready within $maxAttempts seconds."
}

function Ensure-ServiceRunning {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Service,
        [Parameter(Mandatory = $true)]
        [string]$ContainerName
    )

    $exists = & docker ps -a --format "{{.Names}}" | Where-Object { $_ -eq $ContainerName }
    if (-not $exists) {
        Write-Host "[Reset] Service '$Service' container '$ContainerName' not found. Creating via compose..."
        Invoke-Compose up -d $Service | Out-Null
        return
    }

    $isRunning = & docker inspect -f '{{.State.Running}}' $ContainerName 2>$null
    if ($LASTEXITCODE -ne 0 -or $isRunning -ne 'true') {
        Write-Host "[Reset] Starting container '$ContainerName'..."
        & docker start $ContainerName | Out-Null
    }
}

function Execute-PostgresSql {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Database,
        [Parameter(Mandatory = $true)]
        [string]$Sql
    )

    Write-Host "[Reset:sql] db=$Database sql=$Sql"
    & docker exec -e PGPASSWORD=$PostgresPassword -i $PostgresContainer psql -w -h localhost -v ON_ERROR_STOP=1 -U $PostgresUser -d $Database -c $Sql
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to execute SQL against database '$Database'."
    }
}