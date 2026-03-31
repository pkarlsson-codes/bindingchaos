# Sourced by reset-area.sh — defines reset_app_data.
# Requires ensure_postgres and execute_postgres_sql from reset-shared.sh.

reset_app_data() {
    echo "[Reset:app] Resetting application data schemas..."
    ensure_postgres

    local schemas=(
        "bindingchaos_events"
        "stigmergy"
        "community_discourse"
        "tagging"
        "documents"
        "identity_profile"
        "societies"
        "signal_processing"
    )

    local schema
    for schema in "${schemas[@]}"; do
        echo "[Reset:app] Dropping schema '$schema'..."
        execute_postgres_sql "$POSTGRES_DB" "DROP SCHEMA IF EXISTS \"$schema\" CASCADE;"
    done

    echo "[Reset:app] Dropping and recreating public schema..."
    execute_postgres_sql "$POSTGRES_DB" \
        "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public AUTHORIZATION pg_database_owner;"

    echo "[Reset:app] Enabling pgvector extension..."
    execute_postgres_sql "$POSTGRES_DB" "CREATE EXTENSION IF NOT EXISTS vector;"

    echo "[Reset:app] Creating signal_processing schema and embeddings table..."
    execute_postgres_sql "$POSTGRES_DB" \
        "CREATE SCHEMA signal_processing; CREATE TABLE signal_processing.signal_embeddings (signal_id TEXT PRIMARY KEY, embedding vector(384), signal_text TEXT NOT NULL);"

    echo "[Reset:app] Re-applying EF migrations for EF-backed contexts..."
    dotnet ef database update \
        --project src/BindingChaos.IdentityProfile \
        --context IdentityProfileDbContext

    echo "[Reset:app] Done. Marten schemas and development seed data will be recreated when CorePlatform API starts in Development."
}
