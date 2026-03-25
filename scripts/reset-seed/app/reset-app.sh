# Sourced by reset-area.sh — defines reset_app_data.
# Requires ensure_postgres and execute_postgres_sql from reset-shared.sh.

reset_app_data() {
    echo "[Reset:app] Resetting application data schemas..."
    ensure_postgres

    local schemas=(
        "bindingchaos_events"
        "signal_awareness"
        "ideation"
        "community_discourse"
        "tagging"
        "documents"
        "identity_profile"
        "societies"
    )

    local schema
    for schema in "${schemas[@]}"; do
        echo "[Reset:app] Dropping schema '$schema'..."
        execute_postgres_sql "$POSTGRES_DB" "DROP SCHEMA IF EXISTS \"$schema\" CASCADE;"
    done

    echo "[Reset:app] Dropping and recreating public schema..."
    execute_postgres_sql "$POSTGRES_DB" \
        "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public AUTHORIZATION pg_database_owner;"

    echo "[Reset:app] Re-applying EF migrations for EF-backed contexts..."
    dotnet ef database update \
        --project src/BindingChaos.IdentityProfile \
        --context IdentityProfileDbContext

    echo "[Reset:app] Done. Marten schemas and development seed data will be recreated when CorePlatform API starts in Development."
}
