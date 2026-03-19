
function Invoke-ResetAppData {
    Write-Host "[Reset:app] Resetting application data schemas..."
    Ensure-Postgres

    $schemas = @(
        "bindingchaos_events",
        "signal_awareness",
        "ideation",
        "community_discourse",
        "tagging",
        "documents",
        "identity_profile",
        "societies"
    )

    foreach ($schema in $schemas) {
        Write-Host "[Reset:app] Dropping schema '$schema'..."
        Execute-PostgresSql -Database $PostgresDb -Sql "DROP SCHEMA IF EXISTS `"$schema`" CASCADE;"
    }

    # Drop and recreate the public schema to remove Wolverine envelope tables, stray Marten
    # tables, and leftover Paperless-ngx data. Wolverine and Marten recreate what they need on startup.
    Write-Host "[Reset:app] Dropping and recreating public schema..."
    Execute-PostgresSql -Database $PostgresDb -Sql "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public AUTHORIZATION pg_database_owner;"

    Write-Host "[Reset:app] Re-applying EF migrations for EF-backed contexts..."
    & dotnet ef database update --project src/BindingChaos.IdentityProfile --context IdentityProfileDbContext
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to apply IdentityProfile migration."
    }

    Write-Host "[Reset:app] Done. Marten schemas and development seed data will be recreated when CorePlatform API starts in Development."
}